namespace KKTalking.Externals.Instagram.Entities
{
    /// <summary>
    /// サイズを表します。
    /// </summary>
    public readonly struct Size
    {
        #region プロパティ
        /// <summary>
        /// 幅を取得します。
        /// </summary>
        public int Width { get; }

        
        /// <summary>
        /// 高さを取得します。
        /// </summary>
        public int Height { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal Size(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
        #endregion
    }
}
