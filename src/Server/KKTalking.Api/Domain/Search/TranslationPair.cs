namespace KKTalking.Api.Domain.Search
{
    /// <summary>
    /// 翻訳ペアを表します。
    /// </summary>
    public readonly struct TranslationPair
    {
        #region プロパティ
        /// <summary>
        /// 英語を取得します。
        /// </summary>
        public string English { get; }


        /// <summary>
        /// 日本語を取得します。
        /// </summary>
        public string Japanese { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="english"></param>
        /// <param name="japanese"></param>
        internal TranslationPair(string english, string japanese)
        {
            this.English = english;
            this.Japanese = japanese;
        }
        #endregion
    }
}
