namespace KKTalking.Configurations
{
    /// <summary>
    /// Web Proxy の構成情報を表します。
    /// </summary>
    public sealed class WebProxyConfiguration
    {
        #region プロパティ
#pragma warning disable CS8618
        /// <summary>
        /// 有効化するかどうかを取得します。
        /// </summary>
        public bool IsEnabled { get; private set; }


        /// <summary>
        /// ホストの End Point を取得します。
        /// </summary>
        public string Host { get; private set; }


        /// <summary>
        /// ポート番号を取得します。
        /// </summary>
        public int Port { get; private set; }


        /// <summary>
        /// 資格情報を取得します。
        /// </summary>
        public NetworkCredentialConfiguration? Credentials { get; private set; }
#pragma warning restore CS8618
        #endregion
    }
}
