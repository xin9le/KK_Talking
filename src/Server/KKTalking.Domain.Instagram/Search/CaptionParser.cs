using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace KKTalking.Domain.Instagram.Search
{
    /// <summary>
    /// キャプション情報を解析する機構を提供します。
    /// </summary>
    public sealed class CaptionParser
    {
        #region 型
        /// <summary>
        /// セクション情報を表します。
        /// </summary>
        private enum CaptionSection
        {
            /// <summary>
            /// 前置き
            /// </summary>
            Prologue = 0,

            /// <summary>
            /// トピック
            /// </summary>
            Topic,

            /// <summary>
            /// Tips
            /// </summary>
            Tips,

            /// <summary>
            /// 会話
            /// </summary>
            Conversation,

            /// <summary>
            /// 結び
            /// </summary>
            Epilogue,
        }
        #endregion


        #region プロパティ
        /// <summary>
        /// 行単位に分解されたキャプション情報を取得します。
        /// </summary>
        private IReadOnlyList<string> Lines { get; }


        /// <summary>
        /// 現在指している行番号を取得または設定します。
        /// </summary>
        private int LineNumber { get; set; }


        /// <summary>
        /// 現在のセクションを取得または設定します。
        /// </summary>
        private CaptionSection Section { get; set; }


        /// <summary>
        /// 連続している空行数を取得または設定します。
        /// </summary>
        private int EmptyLineCounter { get; set; }
        

        /// <summary>
        /// 現在の行を取得します。
        /// </summary>
        private string CurrentLine
            => this.Lines[this.LineNumber];


        /// <summary>
        /// 現在の行が空行かどうかを取得します。
        /// </summary>
        private bool IsEmptyLine
            => string.IsNullOrWhiteSpace(this.CurrentLine) || this.CurrentLine.StartsWith(".");


        /// <summary>
        /// トピック部分のヘッダーかどうかを取得します。
        /// </summary>
        private bool IsTopicHeader
            => this.CurrentLine.StartsWith("⚜️Topic");


        /// <summary>
        /// Tips 部分のヘッダーかどうかを取得します。
        /// </summary>
        private bool IsTipsHeader
            => this.CurrentLine.StartsWith("🍀Tips");


        /// <summary>
        /// 会話部分のヘッダーかどうかを取得します。
        /// </summary>
        private bool IsConversationHeader
            => this.CurrentLine.StartsWith("----- Conversation -----");


        /// <summary>
        /// ハッシュタグ行かどうかを取得します。
        /// </summary>
        private bool IsHashTagLine
            => this.CurrentLine.Contains("#kktalking");
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="caption"></param>
        private CaptionParser(string caption)
            => this.Lines
                = caption
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
        #endregion


        public static int GetNumber(ReadOnlySpan<char> caption)
        {
            caption = caption.Trim();
            var start = caption.IndexOf("[KK") + 3;
            var end = caption.IndexOf("]");
            var number = caption[start..end];
            return int.Parse(number);
        }


        /// <summary>
        /// 解析を行います。
        /// </summary>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static CaptionParseResult Parse(string caption)
            => new CaptionParser(caption).Parse();


        /// <summary>
        /// 解析を行います。
        /// </summary>
        /// <returns></returns>
        private CaptionParseResult Parse()
        {
            var number = 0;
            var topics = new List<TranslationPair>();
            var tips = new List<TranslationPair>();
            var conversationBuilder = new StringBuilder();

            while (this.LineNumber < this.Lines.Count)
            {
                //--- セクション更新
                UpdateSection();

                //--- 空行を数える
                this.EmptyLineCounter = this.IsEmptyLine ? (this.EmptyLineCounter + 1) : 0;

                //--- セクション固有の処理
                if (this.Section == CaptionSection.Prologue)
                {
                    if (this.LineNumber == 0)
                    {
                        const string prefix = "[KK";
                        var line = this.CurrentLine.AsSpan().Trim();
                        if (line.StartsWith(prefix))
                        {
                            var end = line.IndexOf(']');
                            var value = line[prefix.Length..end].Trim();
                            number = int.Parse(value);
                        }
                    }
                }
                else if (this.Section == CaptionSection.Topic)
                {
                    if (this.IsTopicHeader || this.IsEmptyLine)
                    {
                        // skip
                    }
                    else if (this.CurrentLine.StartsWith("- "))
                    {
                        var splited = this.CurrentLine.TrimStart('-').Split('/');
                        var pair = CreateTranslationPair(splited);
                        topics.Add(pair);
                    }
                    else
                    {
                        //--- 前の行がヘッダーかどうかを判定
                        var backup = this.LineNumber;
                        this.LineNumber--;  // ひとつ前に戻す
                        var previousLineIsTopicHeacer = this.IsTopicHeader;
                        this.LineNumber = backup;  // 元に戻す

                        //--- ヘッダーだったら読み込む
                        if (previousLineIsTopicHeacer)
                        {
                            var splited = this.CurrentLine.Split('/');
                            var pair = CreateTranslationPair(splited);
                            topics.Add(pair);
                        }                        
                    }
                }
                else if (this.Section == CaptionSection.Tips)
                {
                    if (this.IsTipsHeader || this.IsEmptyLine)
                    {
                        // skip
                    }
                    else if (this.CurrentLine.StartsWith("- "))
                    {
                        //--- 基本的には「/」区切りで解析
                        var trimmed = this.CurrentLine.TrimStart('-');
                        var splitBySlash = trimmed.Split('/');
                        if (splitBySlash.Length < 2)
                        {
                            //--- 一部の例外のために「=」区切りにもチャレンジ
                            var splitByEqual = trimmed.Split('=');
                            var pair = CreateTranslationPair(splitByEqual);
                            tips.Add(pair);
                        }
                        else
                        {
                            var pair = CreateTranslationPair(splitBySlash);
                            tips.Add(pair);
                        }
                    }
                }
                else if (this.Section == CaptionSection.Conversation)
                {
                    if (this.IsConversationHeader)
                    {
                        // skip
                    }
                    else if (this.IsEmptyLine)
                    {
                        conversationBuilder.AppendLine();
                    }
                    else
                    {
                        conversationBuilder.AppendLine(this.CurrentLine);
                    }
                }
                else if (this.Section == CaptionSection.Epilogue)
                { }

                //--- 次の行へ
                this.LineNumber++;
            }

            var conversation = conversationBuilder.ToString().TrimEnd();
            return new CaptionParseResult(number, topics, tips, conversation);

            #region ローカル関数
            void UpdateSection()
            {
                if (this.LineNumber == 0)
                {
                    this.Section = CaptionSection.Prologue;
                }
                else if (this.Section == CaptionSection.Conversation)  // 会話セクション
                {
                    if (2 <= this.EmptyLineCounter || this.IsHashTagLine)
                        this.Section = CaptionSection.Epilogue;
                }
                else if (1 <= this.EmptyLineCounter)  // 空行が 1 行以上空いた
                {
                    this.Section = this.Section switch
                    {
                        CaptionSection.Prologue => this.IsTopicHeader ? CaptionSection.Topic : this.Section,
                        CaptionSection.Topic => this.IsTipsHeader ? CaptionSection.Tips : this.Section,
                        CaptionSection.Tips => this.IsConversationHeader ? CaptionSection.Conversation : this.Section,
                        _ => this.Section,
                    };
                }
            }


            static TranslationPair CreateTranslationPair(IReadOnlyList<string> splited)
            {
                var english = splited[0].Trim();
                var japaneseSpan = splited[1].AsSpan();
                var endIndex = japaneseSpan.IndexOf('(');  // 丸カッコ以降は無視する
                var japanese
                    = endIndex < 0
                    ? japaneseSpan[0..].Trim().ToString()
                    : japaneseSpan[0..endIndex].Trim().ToString();
                return new TranslationPair(english, japanese);
            }
            #endregion
        }
    }
}
