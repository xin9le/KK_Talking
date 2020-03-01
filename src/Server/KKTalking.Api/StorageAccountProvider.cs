using Microsoft.Azure.Storage;



namespace KKTalking.Api.Domain
{
    /// <summary>
    /// ストレージアカウントの取得機能を提供します。
    /// </summary>
    public sealed class StorageAccountProvider
    {
        #region プロパティ
        /// <summary>
        /// Azure WebJobs 用ストレージのアカウント情報を取得します。
        /// </summary>
        public CloudStorageAccount AzureWebJobs { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="appSettings"></param>
        public StorageAccountProvider(AppSettings appSettings)
            => this.AzureWebJobs = CloudStorageAccount.Parse(appSettings.AzureWebJobsStorage);
        #endregion
    }
}
