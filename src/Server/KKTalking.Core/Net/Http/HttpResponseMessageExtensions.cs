using System.Net.Http;



namespace KKTalking.Net.Http
{
    /// <summary>
    /// <see cref="HttpResponseMessage"/> の拡張機能を提供します。
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// <see cref="HttpResponseMessage.IsSuccessStatusCode"/> が false を返す場合に例外をスローします。
        /// </summary>
        /// <param name="response"></param>
        public static void ThrowIfError(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;

            var message = $"HTTP response was failed | StatudCode : {response.StatusCode}";
            throw new HttpResponseException(response, message);
        }


        /// <summary>
        /// <see cref="HttpResponseMessage.IsSuccessStatusCode"/> が false を返す場合に例外をスローします。
        /// </summary>
        /// <param name="response"></param>
        /// <param name="message"></param>
        public static void ThrowIfError(this HttpResponseMessage response, string message)
        {
            if (!response.IsSuccessStatusCode)
                throw new HttpResponseException(response, message);
        }
    }
}
