using System;
using System.Collections.Generic;
using KKTalking.Externals.Instagram.Scraping.Models;



namespace KKTalking.Domain.Instagram.Search
{
    /// <summary>
    /// 検索用のメタデータを表します。
    /// </summary>
    public sealed class SearchMetadata
    {
#pragma warning disable CS8618
        #region プロパティ
        /// <summary>
        /// 短い ID を取得または設定します。
        /// </summary>
        public string ShortCode { get; set; }


        /// <summary>
        /// 投稿日時を取得または設定します。
        /// </summary>
        public DateTimeOffset PublishdAt { get; set; }


        /// <summary>
        /// 投稿番号を取得または設定します。
        /// </summary>
        public int Number { get; set; }


        /// <summary>
        /// 投稿のトピックを取得または設定します。
        /// </summary>
        public IReadOnlyList<TranslationPair> Topics { get; set; }


        /// <summary>
        /// 投稿に関連する Tips を取得または設定します。
        /// </summary>
        public IReadOnlyList<TranslationPair> Tips { get; set; }


        /// <summary>
        /// 投稿の会話文を取得または設定します。
        /// </summary>
        public string Conversation { get; set; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        public SearchMetadata()
        { }


        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="post"></param>
        /// <param name="result"></param>
        internal SearchMetadata(PostSlim post, CaptionParseResult result)
        {
            this.ShortCode = post.ShortCode;
            this.PublishdAt = post.PublishdAt;
            this.Number = result.Number;
            this.Topics = result.Topics;
            this.Tips = result.Tips;
            this.Conversation = result.Conversation;
        }


        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="post"></param>
        /// <param name="result"></param>
        internal SearchMetadata(Post post, CaptionParseResult result)
        {
            this.ShortCode = post.ShortCode;
            this.PublishdAt = post.PublishdAt;
            this.Number = result.Number;
            this.Topics = result.Topics;
            this.Tips = result.Tips;
            this.Conversation = result.Conversation;
        }
        #endregion
#pragma warning restore CS8618


        #region IEqualityComparer
        /// <summary>
        /// 等値比較機能を取得します。
        /// </summary>
        public static IEqualityComparer<SearchMetadata> Comparer { get; } = new EqualityComparer();


        /// <summary>
        /// 等値比較機能を提供します。
        /// </summary>
        private sealed class EqualityComparer : IEqualityComparer<SearchMetadata>
        {
            public bool Equals(SearchMetadata x, SearchMetadata y)
                => x.Number == y.Number;


            public int GetHashCode(SearchMetadata obj)
                => obj.Number.GetHashCode();
        }
        #endregion
    }
}
