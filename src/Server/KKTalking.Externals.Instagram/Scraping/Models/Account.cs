using System;
using System.Collections.Generic;



namespace KKTalking.Externals.Instagram.Scraping.Models
{
    /// <summary>
    /// アカウント情報を表します。
    /// </summary>
    public sealed class Account
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
        /// 名前を取得します。
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// アカウント名を取得します。
        /// </summary>
        public string UserName { get; }


        /// <summary>
        /// 自己紹介を取得します。
        /// </summary>
        public string Biography { get; }


        /// <summary>
        /// プロフィールアイコンの URL を取得します。
        /// </summary>
        public Uri IconUrl { get; }


        /// <summary>
        /// 投稿数を取得します。
        /// </summary>
        public int PostCount { get; }


        /// <summary>
        /// フォロー数を取得します。
        /// </summary>
        public int FollowCount { get; }


        /// <summary>
        /// フォロワー数を取得します。
        /// </summary>
        public int FollowerCount { get; }


        /// <summary>
        /// Web サイトの URL を取得します。
        /// </summary>
        public Uri? WebsiteUrl { get; }


        /// <summary>
        /// 非公開アカウントかどうかを取得します。
        /// </summary>
        public bool IsPrivate { get; }


        /// <summary>
        /// 認証済みアカウントかどうかを取得します。
        /// </summary>
        public bool IsVerified { get; }


        /// <summary>
        /// ビジネスアカウントかどうかを取得します。
        /// </summary>
        public bool IsBusiness { get; }


        /// <summary>
        /// 最近登録されたアカウントかどうかを取得します。
        /// </summary>
        public bool IsJoinedRecently { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="snapshotAt"></param>
        internal Account(IReadOnlyDictionary<string, object> user, DateTimeOffset snapshotAt)
        {
            dynamic obj = user;
            var webSiteUrl = (string)user.GetValueOrDefault("external_url");

            this.SnapshotAt = snapshotAt;
            this.Id = (string)user["id"];
            this.Name = (string)user["full_name"];
            this.UserName = (string)user["username"];
            this.Biography = (string)user["biography"];
            this.IconUrl = new Uri((string)user["profile_pic_url_hd"]);
            this.PostCount = (int)obj["edge_owner_to_timeline_media"]["count"];
            this.FollowCount = (int)obj["edge_follow"]["count"];
            this.FollowerCount = (int)obj["edge_followed_by"]["count"];
            this.WebsiteUrl = (webSiteUrl == null) ? null : new Uri(webSiteUrl);
            this.IsPrivate = (bool)user["is_private"];
            this.IsVerified = (bool)user["is_verified"];
            this.IsBusiness = (bool)user["is_business_account"];
            this.IsJoinedRecently = (bool)user["is_joined_recently"];
        }
        #endregion
    }
}
