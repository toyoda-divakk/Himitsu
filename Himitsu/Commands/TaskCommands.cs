using BliFunc.Models;
using ConsoleAppFramework;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using Himitsu.Dependency.Services;

namespace Himitsu.Commands
{
    public class TaskCommands
    {
        // TODO:このprivateは共通なのでHelperServiceに入れたら？
        #region private
        /// <summary>
        /// 環境変数から"BliFuncKey"の値を取得する。
        /// Azure関数アプリのmaster（ホスト）キー。
        /// </summary>
        private static string Key => Environment.GetEnvironmentVariable("BliFuncKey") ?? "";
        private static string EndPoint => Environment.GetEnvironmentVariable("BliFuncEndpoint") ?? "";
        private static string GetUrl(string function) => $"{EndPoint}{function}?code={Key}";

        #endregion

        /// <summary>
        /// タスクを登録します
        /// </summary>
        /// <param name="description">-d, 説明</param>
        /// <param name="category">-c, 分類（半角英字で書くこと）</param>
        [Command("task rec")]
        public async Task RecordTaskAsync([FromServices] HelperService helper, string description, string category = "default")
        {
            // データの準備
            var record = new TodoTask(category, description);

            // データの送信
            var res = await helper.PostAsync(GetUrl("RecordTask"), record);
            Console.WriteLine(res);
        }

        /// <summary>
        /// 指定したカテゴリのタスクを取得して、出力します。
        /// </summary>
        /// <param name="category">-c, 分類（半角英字で書くこと）</param>
        /// <returns></returns>
        [Command("task get")]
        public async Task GetTaskAsync(string category = "default")
        {
            using var client = new HttpClient();
            var res = await client.GetAsync($"{GetUrl("GetTaskRecords")}&partitionKey={category}");
            var json = await res.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<List<TodoTask>>(json);
            if (records == null || records.Count == 0)
            {
                Console.WriteLine("タスクの取得ができませんでした。");
                return;
            }
            Console.WriteLine($"タスク一覧");
            foreach (var record in records)
            {
                Console.WriteLine(record);
            }
        }


        /// <summary>
        /// 指定した分類のタスクを全て削除します
        /// </summary>
        /// <param name="category">-c, 分類（半角英字で書くこと）</param>
        /// <returns></returns>
        [Command("task del all")]
        public async Task DeleteTaskAsync(string category = "default")
        {
            var url = $"{GetUrl("DeleteTasks")}&partitionKey={category}";
            using var client = new HttpClient();
            var res = await client.DeleteAsync(url);
            Console.WriteLine(await res.Content.ReadAsStringAsync());
        }


        /// <summary>
        /// 指定した分類のタスクを取得して、全フィールドを一覧で出力します。
        /// </summary>
        /// <param name="category">-c, 分類（半角英字で書くこと）</param>
        /// <returns></returns>
        [Command("task get all")]
        public async Task GetTaskAllAsync(string category = "default")
        {
            using var client = new HttpClient();
            var res = await client.GetAsync($"{GetUrl("GetTasks")}&partitionKey={category}");
            var json = await res.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<List<TodoTask>>(json);
            if (records == null || records.Count == 0)
            {
                Console.WriteLine("タスクの取得ができませんでした。");
                return;
            }
            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }

        /// <summary>
        /// 最後に登録したタスクを削除します
        /// </summary>
        /// <param name="category">-c, 分類（半角英字で書くこと）</param>
        /// <returns></returns>
        [Command("task del last")]
        public async Task DeleteLastTaskAsync(string category = "default")
        {
            // 最後のレコードを取得
            using var client = new HttpClient();
            var res = await client.GetAsync($"{GetUrl("GetTasks")}&partitionKey={category}");
            var json = await res.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<List<TodoTask>>(json);
            if (records == null || records.Count == 0)
            {
                Console.WriteLine("タスクの取得ができませんでした。");
                return;
            }
            var last = records.Last();

            // IDを指定して削除
            var url = $"{GetUrl("DeleteTask")}&partitionKey={category}&id={last.Id}";
            res = await client.DeleteAsync(url);
            Console.WriteLine(await res.Content.ReadAsStringAsync());
        }
    }
}