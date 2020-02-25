using System;
using System.Collections.Generic;
using System.Linq;



namespace KKTalking.Externals.Instagram.Models
{
    /// <summary>
    /// 投稿情報を表します。
    /// </summary>
    public sealed class Post
    {
        #region プロパティ
        /// <summary>
        /// スナップショットした日時を取得します。
        /// </summary>
        public DateTimeOffset SnapshotAt { get; }


        /// <summary>
        /// ID を取得します。
        /// </summary>
        public string Id { get; }


        /// <summary>
        /// ショートコード (= 短い ID) を取得します。
        /// </summary>
        public string ShortCode { get; }


        /// <summary>
        /// 投稿日時を取得します。
        /// </summary>
        public DateTimeOffset PublishdAt { get; }


        /// <summary>
        /// キャプションを取得します。
        /// </summary>
        public string Caption { get; }


        /// <summary>
        /// キャプションが編集されているかどうかを取得します。
        /// </summary>
        public bool IsCaptionEdited { get; }


        /// <summary>
        /// いいね数を取得します。
        /// </summary>
        public int LikeCount { get; }


        /// <summary>
        /// コメント数を取得します。
        /// </summary>
        public int CommentCount { get; }


        /// <summary>
        /// コメント投稿が許可されているかどうかを取得します。
        /// </summary>
        public bool IsCommentDisabled { get; }


        /// <summary>
        /// 広告かどうかを取得します。
        /// </summary>
        public bool IsAd { get; }


        /// <summary>
        /// オーナー情報を取得します。
        /// </summary>
        public Owner Owner { get; }


        /// <summary>
        /// 位置情報を取得します。
        /// </summary>
        public Location? Location { get; }


        /// <summary>
        /// メディアコンテンツの一覧を取得します。
        /// </summary>
        public IReadOnlyList<Media> Medias { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="post"></param>
        /// <param name="snapshotAt"></param>
        internal Post(IReadOnlyDictionary<string, object> post, DateTimeOffset snapshotAt)
        {
            dynamic obj = post;
            var publishedAt = (long)(double)post["taken_at_timestamp"];
            var owner = (IReadOnlyDictionary<string, object>)post["owner"];
            var location = (IReadOnlyDictionary<string, object>)post["location"];

            this.SnapshotAt = snapshotAt;
            this.Id = (string)post["id"];
            this.ShortCode = (string)post["shortcode"];
            this.PublishdAt = DateTimeOffset.FromUnixTimeSeconds(publishedAt);
            this.Caption = (obj["edge_media_to_caption"]["edges"] as List<dynamic>).FirstOrDefault()?["node"]["text"] ?? string.Empty;
            this.IsCaptionEdited = (bool)post["caption_is_edited"];
            this.LikeCount = (int)obj["edge_media_preview_like"]["count"];
            this.CommentCount = (int)obj["edge_media_preview_comment"]["count"];
            this.IsCommentDisabled = (bool)post["comments_disabled"];
            this.IsAd = (bool)post["is_ad"];
            this.Owner = new Owner(owner);
            this.Location = (location == null) ? (Location?)null : new Location(location);
            this.Medias = ToMedias(post);

            #region ローカル関数
            static IReadOnlyList<Media> ToMedias(IReadOnlyDictionary<string, object> post)
            {
                dynamic sidecar = post.GetValueOrDefault("edge_sidecar_to_children");
                if (sidecar == null)
                {
                    var media = new Media(post);
                    return new[] { media };
                }
                else
                {
                    IEnumerable<dynamic> edges = sidecar["edges"];
                    return edges
                        .Select(x =>
                        {
                            IReadOnlyDictionary<string, object> node = x["node"];
                            return new Media(node);
                        })
                        .ToArray();
                }
            }
            #endregion
        }
        #endregion
    }



    /// <summary>
    /// 軽量な投稿情報を表します。
    /// </summary>
    /// <remarks>主にサムネイル表示などに利用します</remarks>
    public sealed class PostSlim
    {
        #region プロパティ
        /// <summary>
        /// スナップショットした日時を取得します。
        /// </summary>
        public DateTimeOffset SnapshotAt { get; }


        /// <summary>
        /// ID を取得します。
        /// </summary>
        public string Id { get; }


        /// <summary>
        /// ショートコード (= 短い ID) を取得します。
        /// </summary>
        public string ShortCode { get; }


        /// <summary>
        /// 投稿日時を取得します。
        /// </summary>
        public DateTimeOffset PublishdAt { get; }


        /// <summary>
        /// キャプションを取得します。
        /// </summary>
        public string Caption { get; }


        /// <summary>
        /// いいね数を取得します。
        /// </summary>
        public int LikeCount { get; }


        /// <summary>
        /// コメント数を取得します。
        /// </summary>
        public int CommentCount { get; }


        /// <summary>
        /// コメント投稿が許可されているかどうかを取得します。
        /// </summary>
        public bool IsCommentDisabled { get; }


        /// <summary>
        /// コンテンツのサイズを取得します。
        /// </summary>
        public Size Size { get; }


        /// <summary>
        /// 画像の URL を取得します。
        /// </summary>
        public string ImageUrl { get; }


        /// <summary>
        /// 動画コンテンツかどうかを取得します。
        /// </summary>
        public bool IsVideo { get; }


        /// <summary>
        /// オーナーを取得します。
        /// </summary>
        public OwnerSlim Owner { get; }


        /// <summary>
        /// 位置情報を取得します。
        /// </summary>
        public Location? Location { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="post"></param>
        /// <param name="snapshotAt"></param>
        internal PostSlim(IReadOnlyDictionary<string, object> post, DateTimeOffset snapshotAt)
        {
            dynamic obj = post;
            var publishedAt = (long)(double)post["taken_at_timestamp"];
            var dim = (IReadOnlyDictionary<string, object>)post["dimensions"];
            var owner = (IReadOnlyDictionary<string, object>)post["owner"];
            var location = (IReadOnlyDictionary<string, object>)post["location"];

            this.SnapshotAt = snapshotAt;
            this.Id = (string)post["id"];
            this.ShortCode = (string)post["shortcode"];
            this.PublishdAt = DateTimeOffset.FromUnixTimeSeconds(publishedAt);
            this.Caption = (obj["edge_media_to_caption"]["edges"] as List<dynamic>).FirstOrDefault()?["node"]["text"] ?? string.Empty;
            this.LikeCount = (int)obj["edge_media_preview_like"]["count"];
            this.CommentCount = (int)obj["edge_media_to_comment"]["count"];
            this.IsCommentDisabled = (bool)post["comments_disabled"];
            this.Size = new Size
            (
                (int)(double)dim["width"],
                (int)(double)dim["height"]
            );
            this.ImageUrl = (string)post["display_url"];
            this.IsVideo = (bool)post["is_video"];
            this.Owner = new OwnerSlim(owner);
            this.Location = (location == null) ? (Location?)null : new Location(location);
        }
        #endregion
    }
}
