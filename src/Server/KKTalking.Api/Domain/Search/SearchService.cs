using System.Threading;
using System.Threading.Tasks;
using KKTalking.Externals.Instagram.Services;



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
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="scrapingService"></param>
        public SearchService(ScrapingService scrapingService)
            => this.ScrapingService = scrapingService;
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

            // todo: caption を parse
        }
    }
}
