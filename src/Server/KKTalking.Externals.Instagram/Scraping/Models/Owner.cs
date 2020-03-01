using System;
using System.Collections.Generic;



namespace KKTalking.Externals.Instagram.Scraping.Models
{
    /// <summary>
    /// オーナー情報を表します。
    /// </summary>
    public sealed class Owner
    {
        #region プロパティ
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
        /// プロフィールアイコンの URL を取得します。
        /// </summary>
        public Uri IconUrl { get; }


        /// <summary>
        /// 非公開アカウントかどうかを取得します。
        /// </summary>
        public bool IsPrivate { get; }


        /// <summary>
        /// 認証済みアカウントかどうかを取得します。
        /// </summary>
        public bool IsVerified { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="owner"></param>
        internal Owner(IReadOnlyDictionary<string, object> owner)
        {
            this.Id = (string)owner["id"];
            this.Name = (string)owner["full_name"];
            this.UserName = (string)owner["username"];
            this.IconUrl = new Uri((string)owner["profile_pic_url"]);
            this.IsPrivate = (bool)owner["is_private"];
            this.IsVerified = (bool)owner["is_verified"];
        }
        #endregion
    }



    /// <summary>
    /// 軽量なオーナー情報を表します。
    /// </summary>
    public readonly struct OwnerSlim
    {
        #region プロパティ
        /// <summary>
        /// ID を取得します。
        /// </summary>
        public string Id { get; }


        /// <summary>
        /// アカウント名を取得します。
        /// </summary>
        public string UserName { get; }

        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        internal OwnerSlim(string id, string userName)
        {
            this.Id = id;
            this.UserName = userName;
        }


        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="owner"></param>
        internal OwnerSlim(IReadOnlyDictionary<string, object> owner)
        {
            this.Id = (string)owner["id"];
            this.UserName = (string)owner["username"];
        }
        #endregion
    }
}
