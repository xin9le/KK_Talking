using System.Collections.Generic;



namespace KKTalking.Domain.Instagram.Search
{
    /// <summary>
    /// キャプションの解析結果を表します。
    /// </summary>
    public sealed class CaptionParseResult
    {
        #region プロパティ
        /// <summary>
        /// 投稿番号を取得します。
        /// </summary>
        public int Number { get; }


        /// <summary>
        /// 投稿のトピックを取得します。
        /// </summary>
        public IReadOnlyList<TranslationPair> Topics { get; }


        /// <summary>
        /// 投稿に関連する Tips を取得します。
        /// </summary>
        public IReadOnlyList<TranslationPair> Tips { get; }


        /// <summary>
        /// 投稿の会話文を取得します。
        /// </summary>
        public string Conversation { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="number"></param>
        /// <param name="topics"></param>
        /// <param name="tips"></param>
        /// <param name="conversation"></param>
        internal CaptionParseResult(int number, in IReadOnlyList<TranslationPair> topics, IReadOnlyList<TranslationPair> tips, string conversation)
        {
            this.Number = number;
            this.Topics = topics;
            this.Tips = tips;
            this.Conversation = conversation;
        }
        #endregion
    }
}
