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
        [Command("work rec")]
        public async Task RecordWorkAsync(string name, double hours = 7.75, string? date = null)        // asyncの時はvoidにしてしまうと待機せずにメソッドが終了してしまい、Postが終わる前にプログラムが終了してしまう。
        {
            // データの準備
            DateTime? dt = date == null ? null : DateTime.ParseExact(date, "yyMMdd", null);
            var workRecord = new WorkRecord(name, hours, dt);

            // データの送信
            var res = await PostAsync(GetUrl("RecordWork"), workRecord);
            Console.WriteLine(res);
        }

        /// <summary>
        /// 指定した月の工数を取得して、書類フォーマットで出力します。
        /// </summary>
        /// <param name="month">-m, 年月yyyyMM</param>
        /// <returns></returns>
        [Command("work get")]
        public async Task GetWorkAsync(string? month = null)
        {
            var partitionKey = month ?? DateTime.Now.ToString("yyyyMM");
            using var client = new HttpClient();
            var res = await client.GetAsync($"{GetUrl("GetWorkRecords")}&partitionKey={partitionKey}");
            var json = await res.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<List<WorkRecord>>(json);
            if (records == null || records.Count == 0)
            {
                Console.WriteLine("工数の取得ができませんでした。");
                return;
            }
            foreach (var record in records)
            {
                Console.WriteLine(record.ToSheetFormat());
            }
        }

        /// <summary>
        /// 指定した月の工数を削除します
        /// </summary>
        /// <param name="month">-m, 年月yyyyMM</param>
        /// <returns></returns>
        [Command("work del")]
        public async Task DeleteWorkAsync(string? month = null)
        {
            var partitionKey = month ?? DateTime.Now.ToString("yyyyMM");
            var url = $"{GetUrl("DeleteWorkRecords")}&partitionKey={partitionKey}";
            using var client = new HttpClient();
            var res = await client.DeleteAsync(url);
            Console.WriteLine(await res.Content.ReadAsStringAsync());
        }
    }
}