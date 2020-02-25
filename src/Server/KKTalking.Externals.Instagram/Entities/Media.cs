using System.Collections.Generic;



namespace KKTalking.Externals.Instagram.Entities
{
    /// <summary>
    /// メディア情報を表します。
    /// </summary>
    public sealed class Media
    {
        #region プロパティ
        /// <summary>
        /// ID を取得します。
        /// </summary>
        public string Id { get; }


        /// <summary>
        /// ショートコード (= 短い ID) を取得します。
        /// </summary>
        public string ShortCode { get; }


        /// <summary>
        /// コンテンツのサイズを取得します。
        /// </summary>
        public Size Size { get; }


        /// <summary>
        /// 画像の URL を取得します。
        /// </summary>
        public string ImageUrl { get; }


        /// <summary>
        /// 動画の URL を取得します。
        /// </summary>
        public string VideoUrl { get; }


        /// <summary>
        /// 動画の閲覧数を取得します。
        /// </summary>
        public int VideoViewCount { get; }


        /// <summary>
        /// 動画コンテンツかどうかを取得します。
        /// </summary>
        public bool IsVideo { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="media"></param>
        internal Media(IReadOnlyDictionary<string, object> media)
        {
            var dim = (IReadOnlyDictionary<string, object>)media["dimensions"];

            this.Id = (string)media["id"];
            this.ShortCode = (string)media["shortcode"];
            this.Size = new Size
            (
                (int)(double)dim["width"],
                (int)(double)dim["height"]
            );
            this.ImageUrl = (string)media["display_url"];
            this.VideoUrl = (string)media.GetValueOrDefault("video_url");
            this.VideoViewCount = (int)(double)media.GetValueOrDefault("video_view_count", 0.0);
            this.IsVideo = (bool)media["is_video"]; 
        }
        #endregion
    }
}
