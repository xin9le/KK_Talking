using System.Net;



namespace KKTalking.Configurations
{
    /// <summary>
    /// <see cref="NetworkCredential"/> の構成情報を表します。
    /// </summary>
    public sealed class NetworkCredentialConfiguration
    {
        #region プロパティ
#pragma warning disable CS8618
        /// <summary>
        /// ユーザー名を取得します。
        /// </summary>
        public string UserName { get; private set; }


        /// <summary>
        /// パスワードを取得します。
        /// </summary>
        public string Password { get; private set; }
#pragma warning restore CS8618
        #endregion
    }
}
