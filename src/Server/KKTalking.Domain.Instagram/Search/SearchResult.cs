using System.Collections.Generic;



namespace KKTalking.Domain.Instagram.Search
{
    /// <summary>
    /// 検索結果を表します。
    /// </summary>
    public sealed class SearchResult
    {
        #region プロパティ
#pragma warning disable CS8618
        /// <summary>
        /// サムネイル画像のエンドポイント URL を取得します。
        /// </summary>
        public string ThumbnailEndpoint { get; }


        /// <summary>
        /// コンテンツを取得します。
        /// </summary>
        public IReadOnlyList<SearchMetadata> Contents { get; }
#pragma warning restore CS8618
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="thumnailEndpoint"></param>
        /// <param name="contents"></param>
        internal SearchResult(string thumnailEndpoint, IReadOnlyList<SearchMetadata> contents)
        {
            this.ThumbnailEndpoint = thumnailEndpoint;
            this.Contents = contents;
        }
        #endregion
    }
}
