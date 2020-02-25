using System.Threading;
using Microsoft.Azure.WebJobs;



namespace KKTalking.Api.Controllers
{
    /// <summary>
    /// システム全体に関する要求を処理する機能を提供します。
    /// </summary>
    public sealed class SystemController
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        public SystemController()
        { }
        #endregion


        /// <summary>
        /// 定期的なハートビートを行い Hot Standby 状態を維持します。
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Disable("Disable_AlwaysOn")]
        [FunctionName("Timer_AlwaysOn")]
        public void AlwaysOn(
            [TimerTrigger("%CRON_AlwaysOn%")]TimerInfo timer,
            CancellationToken cancellationToken)
        {}
    }
}
