using System.Collections.Generic;



namespace KKTalking.Api.Domain.Search
{
    /// <summary>
    /// 検索用のメタデータを表します。
    /// </summary>
    public sealed class SearchMetadata
    {
        #region プロパティ
        /// <summary>
        /// 投稿番号を取得します。
        /// </summary>
        public int Number { get; }


        /// <summary>
        /// 投稿のトピックを取得します。
        /// </summary>
        public TranslationPair Topic { get; }


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
        /// <param name="topic"></param>
        /// <param name="tips"></param>
        /// <param name="conversation"></param>
        internal SearchMetadata(int number, in TranslationPair topic, IReadOnlyList<TranslationPair> tips, string conversation)
        {
            this.Number = number;
            this.Topic = topic;
            this.Tips = tips;
            this.Conversation = conversation;
        }
        #endregion
    }
}
