using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KKTalking.Externals.Instagram.Models;
using KKTalking.Net.Http;
using Utf8Json;



namespace KKTalking.Externals.Instagram.Services
{
    /// <summary>
    /// Instagram のサイトをスクレイピングする機能を提供します。
    /// </summary>
    public sealed class ScrapingService
    {
        #region 定数
        /// <summary>
        /// ルート URL を表します。
        /// この値は定数です。
        /// </summary>
        private const string RootUrl = "https://www.instagram.com";
        #endregion


        #region プロパティ
        /// <summary>
        /// HTTP 通信クライアントを取得します。
        /// </summary>
        private HttpClient HttpClient { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="client"></param>
        public ScrapingService(HttpClient client)
            => this.HttpClient = client;
        #endregion


        /// <summary>
        /// アカウント情報を抽出します。
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<Account> ExtractAccountAsync(string accountName, CancellationToken cancellationToken = default)
        {
            var user = await this.GetRawUserData(accountName, cancellationToken).ConfigureAwait(false);
            var now = DateTimeOffset.UtcNow;
            return new Account(user, now);
        }


        /// <summary>
        /// 軽量な投稿情報の一覧を抽出します。
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="recursive">無限スクロール部分を再帰的に処理するかどうか</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<IReadOnlyList<PostSlim>> ExtractPostSlimsAsync(string accountName, bool recursive = false, CancellationToken cancellationToken = default)
        {
            var user = await this.GetRawUserData(accountName, cancellationToken).ConfigureAwait(false);
            var userId = (string)user["id"];
            dynamic timeline = user["edge_owner_to_timeline_media"];
            var count
                = recursive
                ? (int)timeline["count"]
                : ((List<dynamic>)timeline["edges"]).Count;

            var buffer = new List<PostSlim>(count);
            var heavyLoading = false;  // 連打を緩くする
            await EnumerateAsync(this.HttpClient, buffer, userId, timeline, recursive, heavyLoading, cancellationToken).ConfigureAwait(false);
            return buffer;

            #region ローカル関数
            async static Task EnumerateAsync(HttpClient client, List<PostSlim> buffer, string userId, dynamic timeline, bool recursive, bool heavyLoading, CancellationToken cancellationToken)
            {
                //--- 要素を格納
                var now = DateTimeOffset.UtcNow;
                IEnumerable<dynamic> edges = timeline["edges"];
                foreach (var x in edges)
                {
                    IReadOnlyDictionary<string, object> node = x["node"];
                    var post = new PostSlim(node, now);
                    buffer.Add(post);
                }

                //--- 無限スクロールを再帰的に呼び出すかどうか
                if (!recursive)
                    return;

                //--- 続きがあれば読み込む
                var pageInfo = timeline["page_info"];
                var hasNextPage = (bool)pageInfo["has_next_page"];
                if (hasNextPage)
                {
                    //--- 連打防止のために気持ちちょっとだけ待つ
                    if (!heavyLoading)
                        await Task.Delay(3000).ConfigureAwait(false);

                    //--- 読み込み
                    const string queryId = "e769aa130647d2354c40ea6a439bfc08";
                    var data = new
                    {
                        id = userId,
                        first = 12,  // 最大 50 件っぽい
                        after = (string)pageInfo["end_cursor"],
                    };
                    var variables = JsonSerializer.ToJsonString(data);
                    var url = Uri.EscapeUriString($"{RootUrl}/graphql/query/?query_hash={queryId}&variables={variables}");
                    var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
                    response.ThrowIfError();
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var obj = JsonSerializer.Deserialize<dynamic>(json);
                    var next = obj["data"]["user"]["edge_owner_to_timeline_media"];
                    await EnumerateAsync(client, buffer, userId, next, recursive, heavyLoading, cancellationToken).ConfigureAwait(false);
                }
            }
            #endregion
        }


        /// <summary>
        /// 投稿情報を抽出します。
        /// </summary>
        /// <param name="shortCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<Post> ExtractPostAsync(string shortCode, CancellationToken cancellationToken = default)
        {
            var url = $"{RootUrl}/p/{shortCode}/";
            var html = await this.GetHtmlAsync(url, cancellationToken).ConfigureAwait(false);
            var sharedData = this.ExtractSharedData(html);
            var json = JsonSerializer.Deserialize<dynamic>(sharedData);
            var post = (IReadOnlyDictionary<string, object>)json["entry_data"]["PostPage"][0]["graphql"]["shortcode_media"];
            var now = DateTimeOffset.UtcNow;
            return new Post(post, now);
        }


        #region 補助
        /// <summary>
        /// 指定されたアカウントの生ユーザー情報を取得します。
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        private async ValueTask<IReadOnlyDictionary<string, object>> GetRawUserData(string accountName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(accountName))
                throw new ArgumentException(nameof(accountName));

            //--- SharedData を取得
            var url = $"{RootUrl}/{accountName}/";
            var html = await this.GetHtmlAsync(url, cancellationToken).ConfigureAwait(false);
            var sharedData = this.ExtractSharedData(html);

            //--- ユーザー情報を取得
            var obj = JsonSerializer.Deserialize<dynamic>(sharedData);
            var user = obj["entry_data"]["ProfilePage"][0]["graphql"]["user"];
            return (IReadOnlyDictionary<string, object>)user;
        }


        /// <summary>
        /// 指定された URL の HTML を取得します。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async ValueTask<HtmlDocument> GetHtmlAsync(string url, CancellationToken cancellationToken)
        {
            var response = await this.HttpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.ThrowIfError();

            var html = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var document = new HtmlDocument();
            document.Load(html, Encoding.UTF8);
            return document;
        }


        /// <summary>
        /// 指定された HTML ドキュメントから window._sharedData の JSON を取得します。
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string ExtractSharedData(HtmlDocument html)
        {
            const string key = "window._sharedData =";
            return html.DocumentNode
                .SelectNodes("//script[@type=\"text/javascript\"]")
                .Select(x => x.InnerText.Trim())
                .Where(x => x.StartsWith(key))
                .First()
                .AsSpan()
                .TrimStart(key)
                .TrimEnd(';')
                .ToString();
        }
        #endregion
    }
}
