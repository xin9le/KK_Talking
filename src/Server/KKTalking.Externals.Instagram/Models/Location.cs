using System.Collections.Generic;



namespace KKTalking.Externals.Instagram.Models
{
    /// <summary>
    /// 位置情報を表します。
    /// </summary>
    public readonly struct Location
    {
        #region プロパティ
        /// <summary>
        /// ID を取得します。
        /// </summary>
        public string Id { get; }


        /// <summary>
        /// 名称を取得します。
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// 公開ページがあるかどうかを取得します。
        /// </summary>
        public bool HasPublicPage { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="hasPublicPage"></param>
        internal Location(string id, string name, bool hasPublicPage)
        {
            this.Id = id;
            this.Name = name;
            this.HasPublicPage = hasPublicPage;
        }


        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="source"></param>
        internal Location(IReadOnlyDictionary<string, object> source)
        {
            this.Id = (string)source["id"];
            this.Name = (string)source["name"];
            this.HasPublicPage = (bool)source["has_public_page"];
        }
        #endregion
    }
}
