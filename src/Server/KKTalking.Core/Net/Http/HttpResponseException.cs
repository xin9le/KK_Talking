using System;
using System.Net.Http;



namespace KKTalking.Net.Http
{
    /// <summary>
    /// HTTP 応答の例外を表します。
    /// </summary>
    public class HttpResponseException : Exception
    {
        #region プロパティ
        /// <summary>
        /// HTTP 応答を取得します。
        /// </summary>
        public HttpResponseMessage Response { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="response"></param>
        internal HttpResponseException(HttpResponseMessage response)
            : base()
            => this.Response = response;


        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="response"></param>
        /// <param name="message"></param>
        internal HttpResponseException(HttpResponseMessage response, string message)
            : base(message)
            => this.Response = response;


        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="response"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        internal HttpResponseException(HttpResponseMessage response, string message, Exception innerException)
            : base(message, innerException)
            => this.Response = response;
        #endregion
    }
}
