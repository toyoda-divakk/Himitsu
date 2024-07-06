using BliFunc.Models;
using ConsoleAppFramework;
using System.Text;
using Newtonsoft.Json;

namespace Himitsu.Commands
{
    public class WorkCommands // IDisposable, IAsyncDisposableがついていたら、コマンド実行後にDisposeされる。
    {
        #region private
        /// <summary>
        /// 環境変数から"BliFuncKey"の値を取得する。
        /// Azure関数アプリのmaster（ホスト）キー。
        /// </summary>
        private static string Key => Environment.GetEnvironmentVariable("BliFuncKey") ?? "";
        private static string EndPoint => Environment.GetEnvironmentVariable("BliFuncEndpoint") ?? "";
        private string GetUrl(string function) => $"{EndPoint}{function}?code={Key}";

        /// <summary>
        /// HttpPostを行い、結果を表示する。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="record">工数情報</param>
        /// <returns></returns>
        private async Task<string> PostAsync(string url, WorkRecord record)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(record);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync(url, content);
            return await res.Content.ReadAsStringAsync();
        }
        #endregion

        /// <summary>
        /// 今日の工数を登録します
        /// </summary>
        /// <param name="name">-n, issue番号か名前</param>
        /// <param name="hours">-h, 工数</param>
        /// <param name="date">-d, 日付：yyMMdd</param>
        [Command("rec")]
        public async Task RecordWorkAsync(string name, double hours = 7.75, string? date = null)        // asyncの時はvoidにしてしまうと待機せずにメソッドが終了してしまい、Postが終わる前にプログラムが終了してしまう。
        {
            // データの準備
            DateTime? dt = date == null ? null : DateTime.ParseExact(date, "yyMMdd", null);
            var workRecord = new WorkRecord(name, hours, dt);

            // データの送信
            var res = await PostAsync(GetUrl("RecordWork"), workRecord);
            Console.WriteLine(res);
        }

    }
}