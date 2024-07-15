using BliFunc.Models;
using ConsoleAppFramework;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using Himitsu.Dependency.Services;

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
        private static string GetUrl(string function) => $"{EndPoint}{function}?code={Key}";
        private static readonly double DailyStandup = 0.25;

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
        private static List<WorkRecord> AggregateWorkRecords(List<WorkRecord> records)
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
        public async Task RecordWorkAsync([FromServices] HelperService helper, string name, double hours = 7.75, string? date = null)
        {
            // データの準備
            DateTime? dt = date == null ? null : DateTime.ParseExact(date, "yyMMdd", null);
            var workRecord = new WorkRecord(name, hours, dt);

            // データの送信
            var res = await helper.PostAsync(GetUrl("RecordWork"), workRecord);
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
            var workDays = GetWorkDays(records);
            if (!test)
            {
                records = AggregateWorkRecords(records);
            }
            var standup = (workDays - GetDailyStandupSkipDays(records)) * DailyStandup;
            Console.WriteLine($"日次\tCRX Daily Standup\t{standup}h");

            foreach (var record in records)
            {
                Console.WriteLine(record.ToSheetFormat());
            }
            Console.WriteLine($"\t合計作業時間(概算)\t{records.Sum(x => x.Hours) + standup}h");
        }

        /// <summary>
        /// 作業が8時間以上の日はDaily Standupの工数を0とするため、その日数を算出して補正する工数を求める
        /// </summary>
        /// <param name="records"></param>
        /// <returns>Daily Standupの工数を0にする日数</returns>
        private static int GetDailyStandupSkipDays(List<WorkRecord> records)
        {
            var skip = 0;
            var dailyHours = records.GroupBy(x => x.Date).Select(x => x.Sum(y => y.Hours));
            foreach (var hours in dailyHours)
            {
                if (hours >= 8)
                {
                    skip++;
                }
            }
            return skip;
        }

        /// <summary>
        /// 指定した月の工数を全て削除します
        /// </summary>
        /// <param name="month">-m, 年月yyyyMM</param>
        /// <returns></returns>
        [Command("work del all")]
        public async Task DeleteWorkAsync(string? month = null)
        {
            var partitionKey = month ?? DateTime.Now.ToString("yyyyMM");
            var url = $"{GetUrl("DeleteWorkRecords")}&partitionKey={partitionKey}";
            using var client = new HttpClient();
            var res = await client.DeleteAsync(url);
            Console.WriteLine(await res.Content.ReadAsStringAsync());
        }


        /// <summary>
        /// 指定した月の工数を取得して、全フィールドを一覧で出力します。
        /// </summary>
        /// <param name="month">-m, 年月yyyyMM</param>
        /// <returns></returns>
        [Command("work get all")]
        public async Task GetWorkAllAsync(string? month = null)
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
                Console.WriteLine(record.ToString());
            }
        }

        /// <summary>
        /// 最後に登録した工数を削除します
        /// </summary>
        /// <param name="month">-m, 年月yyyyMM</param>
        /// <returns></returns>
        [Command("work del last")]
        public async Task DeleteLastWorkAsync(string? month = null)
        {
            var partitionKey = month ?? DateTime.Now.ToString("yyyyMM");
            
            // 最後のレコードを取得
            using var client = new HttpClient();
            var res = await client.GetAsync($"{GetUrl("GetWorkRecords")}&partitionKey={partitionKey}");
            var json = await res.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<List<WorkRecord>>(json);
            if (records == null || records.Count == 0)
            {
                Console.WriteLine("工数の取得ができませんでした。");
                return;
            }
            var last = records.Last();

            // IDを指定して削除
            var url = $"{GetUrl("DeleteWorkRecord")}&partitionKey={partitionKey}&id={last.Id}";
            res = await client.DeleteAsync(url);
            Console.WriteLine(await res.Content.ReadAsStringAsync());
        }
    }
}