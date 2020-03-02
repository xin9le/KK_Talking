using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KKTalking.Externals.Instagram.Scraping;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Utf8Json;
using ValueTaskSupplement;



namespace KKTalking.Domain.Instagram.Search
{
    /// <summary>
    /// Instagram の投稿検索に関する処理を提供します。
    /// </summary>
    public sealed class SearchService
    {
        #region 定数
        /// <summary>
        /// Instagram のアカウント名を表します。
        /// </summary>
        private const string AccountName = "kk_talking";


        /// <summary>
        /// 検索メタデータを格納している Blob Storage のコンテナ名を表します。
        /// </summary>
        private const string SearchMetadataContainerName = "instagram-post-search-metadata";


        /// <summary>
        /// 投稿のサムネイル画像を格納している Blob Storage のコンテナ名を表します。
        /// </summary>
        private const string PostThumbnailContainerName = "instagram-post-thumbnail";


        /// <summary>
        /// 検索インデックス名を表します。
        /// </summary>
        private const string SearchIndexName = "instagram-post-index";


        /// <summary>
        /// トピック検索の対象フィールドを表します。
        /// </summary>
        private static string[] TopicSearchFields { get; } = new[] { "Topics/English", "Topics/Japanese" };


        /// <summary>
        /// Tips 検索の対象フィールドを表します。
        /// </summary>
        private static string[] TipsSearchFields { get; } = new[] { "Tips/English", "Tips/Japanese" };


        /// <summary>
        /// 取得対象フィールドを表します。
        /// </summary>
        private static string[] SelectFields { get; }
            = new[]
            {
                "ShortCode",
                "PublishdAt",
                "Number",
                "Topics/English",
                "Topics/Japanese",
                "Tips/English",
                "Tips/Japanese",
            };
        #endregion


        #region プロパティ
        /// <summary>
        /// Instagram のスクレイピング機能を取得します。
        /// </summary>
        private ScrapingService ScrapingService { get; }


        /// <summary>
        /// Blob Storage へのアクセス機能を取得します。
        /// </summary>
        private CloudBlobClient BlobClient { get; }


        /// <summary>
        /// Cognitive Search へのアクセス機能を取得します。
        /// </summary>
        private SearchServiceClient SearchClient { get; }


        /// <summary>
        /// HTTP アクセスするための機能を取得します。
        /// </summary>
        private HttpClient HttpClient
            => this.SearchClient.HttpClient;  // これは OK


        /// <summary>
        /// ロガーを取得します。
        /// </summary>
        private ILogger Logger { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="scrapingService"></param>
        /// <param name="blobClient"></param>
        /// <param name="searchClient"></param>
        /// <param name="logger"></param>
        public SearchService(ScrapingService scrapingService, CloudBlobClient blobClient, SearchServiceClient searchClient, ILogger<SearchService> logger)
        {
            this.ScrapingService = scrapingService;
            this.BlobClient = blobClient;
            this.SearchClient = searchClient;
            this.Logger = logger;
        }
        #endregion


        /// <summary>
        /// 検索データを構築します。
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask BuildAsync(CancellationToken cancellationToken = default)
        {
            var posts = await this.ScrapingService.ExtractPostSlimsAsync(AccountName, recursive: false, cancellationToken).ConfigureAwait(false);
            foreach (var x in posts)
            {
                try
                {
                    var result = CaptionParser.Parse(x.Caption);
                    var metadata = new SearchMetadata(x, result);
                    var t1 = this.CopyThumbnailAsync(x.ImageUrl, metadata.Number, overwrite: false, cancellationToken);
                    var t2 = this.UploadMetadataAsync(metadata, cancellationToken);
                    await ValueTaskEx.WhenAll(t1, t2).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var message = $"検索データの構築中にエラーが発生しました | ShortCode : {x.ShortCode}";
                    this.Logger.LogError(ex, message);
                    throw;
                }
            }
        }


        /// <summary>
        /// 検索データを構築します。
        /// </summary>
        /// <param name="shortCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<SearchMetadata> BuildAsync(string shortCode, CancellationToken cancellationToken = default)
        {
            var post = await this.ScrapingService.ExtractPostAsync(shortCode, cancellationToken).ConfigureAwait(false);
            if (post.Owner.UserName != AccountName)
            {
                var message = $"{AccountName} 以外の投稿データを取得しようとしています | AccountName : {post.Owner.UserName}";
                throw new InvalidOperationException(message);
            }

            var result = CaptionParser.Parse(post.Caption);
            var metadata = new SearchMetadata(post, result);
            var t1 = this.CopyThumbnailAsync(post.Medias[0].ImageUrl, metadata.Number, overwrite: true, cancellationToken);
            var t2 = this.UploadMetadataAsync(metadata, cancellationToken);
            await ValueTaskEx.WhenAll(t1, t2).ConfigureAwait(false);
            return metadata;
        }


        /// <summary>
        /// 指定されたキーワードで投稿の検索を行います。
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="top"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<SearchResult> SearchAsync(string searchText, int? top = default, CancellationToken cancellationToken = default)
        {
            //--- 全文検索
            var indexClient = this.SearchClient.Indexes.GetClient(SearchIndexName);
            var t1 = indexClient.Documents.SearchAsync<SearchMetadata>
            (
                searchText,
                searchParameters: new SearchParameters
                {
                    SearchMode = SearchMode.All,
                    SearchFields = TopicSearchFields,  // Topic だけで検索
                    Select = SelectFields,
                    Top = top,
                },
                cancellationToken: cancellationToken
            );
            var t2 = indexClient.Documents.SearchAsync<SearchMetadata>
            (
                searchText,
                searchParameters: new SearchParameters
                {
                    SearchMode = SearchMode.All,
                    SearchFields = TipsSearchFields,  // Tips だけで検索
                    Select = SelectFields,
                    Top = top,
                },
                cancellationToken: cancellationToken
            );
            var metadatas
                = (await Task.WhenAll(t1, t2).ConfigureAwait(false))
                .SelectMany(x => x.Results)
                .Select(x => x.Document)
                .Distinct(SearchMetadata.Comparer)  // Topic / Tips それぞれの検索結果から重複を削除
                .ToArray();

            //--- 形式を調整
            var container = this.BlobClient.GetContainerReference(PostThumbnailContainerName);
            var thumbnailEndpoint = container.Uri.ToString();  // CDN 通すときはここを調整
            return new SearchResult(thumbnailEndpoint, metadatas);
        }


        #region 補助
        /// <summary>
        /// 検索メタデータをアップロードします。
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private ValueTask UploadMetadataAsync(SearchMetadata metadata, CancellationToken cancellationToken = default)
        {
            var fileName = $"KK{metadata.Number}.json";
            var json = JsonSerializer.Serialize(metadata);
            var container = this.BlobClient.GetContainerReference(SearchMetadataContainerName);
            var blob = container.GetBlockBlobReference(fileName);
            return blob.UploadFromByteArrayAsync(json, 0, json.Length, cancellationToken).AsValueTask();
        }


        /// <summary>
        /// 指定されたサムネイル画像をコピーします。
        /// </summary>
        /// <param name="thumbnailUrl"></param>
        /// <param name="number"></param>
        /// <param name="overwrite"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async ValueTask CopyThumbnailAsync(string thumbnailUrl, int number, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            var fileName = $"KK{number}.jpg";
            var container = this.BlobClient.GetContainerReference(PostThumbnailContainerName);
            var blob = container.GetBlockBlobReference(fileName);
            var exists = await blob.ExistsAsync(cancellationToken).ConfigureAwait(false);
            if (!exists || overwrite)
            {
                //--- コピー
                var response = await this.HttpClient.GetAsync(thumbnailUrl, cancellationToken).ConfigureAwait(false);
                var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                await blob.UploadFromStreamAsync(stream).ConfigureAwait(false);
            }
        }
        #endregion
    }
}
