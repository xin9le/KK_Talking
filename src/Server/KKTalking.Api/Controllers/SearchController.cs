using System.Threading;
using System.Threading.Tasks;
using KKTalking.Api.Domain.Search;
using Microsoft.Azure.WebJobs;



namespace KKTalking.Api.Controllers
{
    /// <summary>
    /// 検索関連の要求を処理します。
    /// </summary>
    public sealed class SearchController
    {
        #region プロパティ
        /// <summary>
        /// 検索サービスを取得します。
        /// </summary>
        private SearchService SearchService { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="searchService"></param>
        public SearchController(SearchService searchService)
            => this.SearchService = searchService;
        #endregion


        /// <summary>
        /// 検索データを構築します。
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Disable("Disable_BuildSearchData")]
        [FunctionName("Timer_BuildSearchData")]
        public async Task Build(
            [TimerTrigger("%CRON_BuildSearchData%")]TimerInfo timer,
            CancellationToken cancellationToken)
        {
            await this.SearchService.BuildAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
