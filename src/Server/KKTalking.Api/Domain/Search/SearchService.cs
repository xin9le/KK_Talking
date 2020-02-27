using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KKTalking.Externals.Instagram.Services;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Utf8Json;



namespace KKTalking.Api.Domain.Search
{
    /// <summary>
    /// 投稿の検索に関する処理を提供します。
    /// </summary>
    public sealed class SearchService
    {
        #region 定数
        /// <summary>
        /// Instagram のアカウント名を表します。
        /// </summary>
        private const string AccountName = "kk_talking";
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
        /// ロガーを取得します。
        /// </summary>
        private ILogger Logger { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="scrapingService"></param>
        /// <param name="accountProvider"></param>
        /// <param name="searchClient"></param>
        /// <param name="logger"></param>
        public SearchService(ScrapingService scrapingService, StorageAccountProvider accountProvider, SearchServiceClient searchClient, ILogger<SearchService> logger)
        {
            this.ScrapingService = scrapingService;
            this.BlobClient = accountProvider.AzureWebJobs.CreateCloudBlobClient();
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
            const bool recursive = false;
            var posts = await this.ScrapingService.ExtractPostSlimsAsync(AccountName, recursive, cancellationToken).ConfigureAwait(false);

            foreach (var x in posts)
            {
                try
                {
                    var result = CaptionParser.Parse(x.Caption);
                    var metadata = new SearchMetadata(x, result);
                    await this.UploadAsync(metadata, cancellationToken).ConfigureAwait(false);
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
            await this.UploadAsync(metadata, cancellationToken).ConfigureAwait(false);
            return metadata;
        }


        /// <summary>
        /// 指定されたキーワードで投稿の検索を行います。
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="top"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<SearchMetadata[]> SearchAsync(string searchText, int? top = default, CancellationToken cancellationToken = default)
        {
            const string indexName = "instagram-post-index";
            var indexClient = this.SearchClient.Indexes.GetClient(indexName);
            var result
                = await indexClient.Documents
                .SearchAsync<SearchMetadata>
                (
                    searchText,
                    searchParameters: new SearchParameters
                    {
                        SearchMode = SearchMode.All,
                        //HighlightFields = new []
                        //{
                        //    "Conversation",
                        //    "Topics/English",
                        //    "Topics/Japanese",
                        //    "Tips/English",
                        //    "Tips/Japanese",
                        //},
                        Top = top,
                    },
                    cancellationToken: cancellationToken
                )
                .ConfigureAwait(false);
            return result.Results.Select(x => x.Document).ToArray();
        }


        #region 補助
        /// <summary>
        /// 検索メタデータをアップロードします。
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private ValueTask UploadAsync(SearchMetadata metadata, CancellationToken cancellationToken = default)
        {
            var fileName = $"KK{metadata.Number}.json";
            var json = JsonSerializer.Serialize(metadata);
            var container = this.BlobClient.GetContainerReference("instagram-post-search-metadata");
            var blob = container.GetBlockBlobReference(fileName);
            var task = blob.UploadFromByteArrayAsync(json, 0, json.Length, cancellationToken);
            return new ValueTask(task);
        }
        #endregion
    }
}
