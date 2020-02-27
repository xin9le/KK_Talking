using System.Threading;
using System.Threading.Tasks;
using KKTalking.Api.Domain.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;



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
        /// <param name="request"></param>
        /// <param name="shortCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Disable("Disable:BuildSearchData")]
        [FunctionName("Http_BuildSearchData")]
        public async Task<IActionResult> Build(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "search/build/{shortCode}")] HttpRequest request,
            [FromRoute] string shortCode,
            CancellationToken cancellationToken)
        {
            var metadata = await this.SearchService.BuildAsync(shortCode, cancellationToken).ConfigureAwait(false);
            return new OkObjectResult(metadata);
        }


        /// <summary>
        /// 検索データを構築します。
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Disable("Disable:BuildMultiSearchData")]
        [FunctionName("Timer_BuildMultiSearchData")]
        public async Task BuildMulti(
            [TimerTrigger("%Cron:BuildMultiSearchData%")]TimerInfo timer,
            CancellationToken cancellationToken)
        {
            await this.SearchService.BuildAsync(cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// キーワード検索します。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Disable("Disable:KeywordSearch")]
        [FunctionName("Http_KeywordSearch")]
        public async Task<IActionResult> KeywordSearch(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "search")] HttpRequest request,
            CancellationToken cancellationToken)
        {
            if (!request.Query.TryGetValue("q", out var searchText))
                return new BadRequestObjectResult("Can't find search parameter.");

            int? top = null;  // 一旦既定値にしておく
            var result = await this.SearchService.SearchAsync(searchText, top, cancellationToken).ConfigureAwait(false);
            return new OkObjectResult(result);
        }
    }
}
