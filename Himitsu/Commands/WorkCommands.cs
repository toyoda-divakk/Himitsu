using BliFunc.Models;
using ConsoleAppFramework;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

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

        /// <summary>
        /// 日付だけでDistinctを取って、稼働日数を算出する。
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        private static int GetWorkDays(List<WorkRecord> records) => records.Select(x => x.Date).Select(y => y.Day).Distinct().Count();

        /// <summary>
        /// 工数データの中で、連続した日付で同じタスクを行った場合、期間をまとめて表示します。
        /// </summary>
        /// <param name="records">月次工数データ</param>
        /// <returns>集約済み工数データ</returns>
        private List<WorkRecord> AggregateWorkRecords(List<WorkRecord> records)
        {
            if (records.Count == 0)
            {
                return records;
            }
            records.Sort((x, y) => x.TaskName.CompareTo(y.TaskName));
            records.Sort((x, y) => x.Date.CompareTo(y.Date));

            var result = new List<WorkRecord>();
            var temp = records.First();
            var days = 1;
            for (int i = 1; i < records.Count; i++)
            {
                var next = records[i];
                if (temp.TaskName == next.TaskName && temp.Date.Day == next.Date.Day)
                {
                    temp.EndDate = next.Date;
                    temp.Hours += next.Hours;
                }
                else if (temp.TaskName == next.TaskName && temp.Date.Day + days == next.Date.Day)
                {
                    temp.EndDate = next.Date;
                    temp.Hours += next.Hours;
                    days++;
                }
                else
                {
                    result.Add(temp);
                    temp = records[i];
                    days = 1;
                }
            }
            result.Add(temp);
            return result;
        }

        #endregion

        /// <summary>
        /// 今日の工数を登録します
        /// </summary>
        /// <param name="name">-n, issue番号か名前</param>
        /// <param name="hours">-h, 工数</param>
        /// <param name="date">-d, 日付：yyMMdd</param>
        [Command("work rec")]
        public async Task RecordWorkAsync(string name, double hours = 7.75, string? date = null)
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
        /// <param name="test">-t, 纏めないで出力</param>
        /// <returns></returns>
        [Command("work get")]
        public async Task GetWorkAsync(string? month = null, bool test = false)
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
            var standup = GetWorkDays(records) * 0.25;
            Console.WriteLine($"日次\tCRX Daily Standup\t{standup}h");

            if (!test)
            {
                records = AggregateWorkRecords(records);
            }
            foreach (var record in records)
            {
                Console.WriteLine(record.ToSheetFormat());
            }
            Console.WriteLine($"\t合計作業時間(概算)\t{records.Sum(x => x.Hours) + standup}h");
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