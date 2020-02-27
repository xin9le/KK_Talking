using System;
using System.Collections.Generic;
using KKTalking.Externals.Instagram.Models;



namespace KKTalking.Api.Domain.Search
{
    /// <summary>
    /// 検索用のメタデータを表します。
    /// </summary>
    public sealed class SearchMetadata
    {
        #region プロパティ
        /// <summary>
        /// 短い ID を取得します。
        /// </summary>
        public string ShortCode { get; }


        /// <summary>
        /// 画像 URL を取得します。
        /// </summary>
        public string ImageUrl { get; }


        /// <summary>
        /// 投稿日時を取得します。
        /// </summary>
        public DateTimeOffset PublishdAt { get; }


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
        /// <param name="post"></param>
        /// <param name="result"></param>
        internal SearchMetadata(PostSlim post, CaptionParseResult result)
        {
            this.ShortCode = post.ShortCode;
            this.ImageUrl = post.ImageUrl;
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
            this.ImageUrl = post.Medias[0].ImageUrl;
            this.PublishdAt = post.PublishdAt;
            this.Number = result.Number;
            this.Topics = result.Topics;
            this.Tips = result.Tips;
            this.Conversation = result.Conversation;
        }
        #endregion
    }
}
