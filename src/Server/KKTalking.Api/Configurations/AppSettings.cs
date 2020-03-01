using KKTalking.Configurations;



namespace KKTalking.Api.Configurations
{
    /// <summary>
    /// アプリケーション設定を表します。
    /// </summary>
    public sealed class AppSettings
    {
#pragma warning disable CS8618
        /// <summary>
        /// Azure WebJobs が利用する Storage への接続文字列を取得します。
        /// </summary>
        public string AzureWebJobsStorage { get; private set; }


        /// <summary>
        /// Azure Cognitive Service の構成を取得します。
        /// </summary>
        public CognitiveSearchConfiguration CognitiveSearch { get; private set; }


        /// <summary>
        /// Web Proxy の構成を取得します。
        /// </summary>
        public WebProxyConfiguration WebProxy { get; private set; }
#pragma warning restore CS8618
    }
}
