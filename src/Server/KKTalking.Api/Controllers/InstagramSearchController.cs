using System;
using System.Threading;
using System.Threading.Tasks;
using KKTalking.Domain.Instagram.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;



namespace KKTalking.Api.Controllers
{
    /// <summary>
    /// Instagram の投稿検索関連の要求を処理します。
    /// </summary>
    public sealed class InstagramSearchController
    {
        #region プロパティ
        /// <summary>
        /// Instagram の投稿検索サービスを取得します。
        /// </summary>
        private SearchService SearchService { get; }


        /// <summary>
        /// ロガーを取得します。
        /// </summary>
        private ILogger Logger { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="searchService"></param>
        /// <param name="logger"></param>
        public InstagramSearchController(SearchService searchService, ILogger<InstagramSearchController> logger)
        {
            this.SearchService = searchService;
            this.Logger = logger;
        }
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
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "search/build/{shortCode}")] HttpRequest request,
            [FromRoute] string shortCode,
            CancellationToken cancellationToken)
        {
            try
            {
                var metadata = await this.SearchService.BuildAsync(shortCode, cancellationToken).ConfigureAwait(false);
                return new OkObjectResult(metadata);
            }
            catch (Exception ex)
            {
                var message = $"検索データの構築中にエラーが発生しました | ShortCode : {shortCode}";
                this.Logger.LogError(ex, message);
                throw;
            }
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
            try
            {
                await this.SearchService.BuildAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var message = $"検索データの定期構築中にエラーが発生しました";
                this.Logger.LogError(ex, message);
                throw;
            }
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "search")] HttpRequest request,
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
