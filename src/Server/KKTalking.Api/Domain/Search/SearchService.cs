using System.Threading;
using System.Threading.Tasks;
using KKTalking.Externals.Instagram.Services;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
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
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="scrapingService"></param>
        /// <param name="configuration"></param>
        public SearchService(ScrapingService scrapingService, IConfigurationRoot config)
        {
            this.ScrapingService = scrapingService;
            var account = CloudStorageAccount.Parse(config["AzureWebJobsStorage"]);
            this.BlobClient = account.CreateCloudBlobClient();
        }   
        #endregion


        /// <summary>
        /// 検索データを構築します。
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask BuildAsync(CancellationToken cancellationToken = default)
        {
            //--- 取得
            const bool recursive = false;
            var posts = await this.ScrapingService.ExtractPostSlimsAsync(AccountName, recursive, cancellationToken).ConfigureAwait(false);

            //--- キャプションを解析して Blob Storage にアップロード
            var container = this.BlobClient.GetContainerReference("instagram-post-search-metadata");
            foreach (var x in posts)
            {
                try
                {
                    var result = CaptionParser.Parse(x.Caption);
                    var metadata = new SearchMetadata(x, result);
                    var fileName = $"KK{metadata.Number}.json";
                    var json = JsonSerializer.Serialize(metadata);
                    var blob = container.GetBlockBlobReference(fileName);
                    await blob.UploadFromByteArrayAsync(json, 0, json.Length, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    // エラーは一旦無視
                }
            }
        }
    }
}
