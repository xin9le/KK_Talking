namespace KKTalking.Configurations
{
    /// <summary>
    /// Azure Cognitive Search の構成を表します。
    /// </summary>
    public sealed class CognitiveSearchConfiguration
    {
#pragma warning disable CS8618
        /// <summary>
        /// サービス名を取得します。
        /// </summary>
        public string ServiceName { get; private set; }


        /// <summary>
        /// API キーを取得します。
        /// </summary>
        public string ApiKey { get; private set; }
#pragma warning restore CS8618
    }
}
