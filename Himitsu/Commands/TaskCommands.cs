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
        /// <summary>デフォルトのコマンド：タスク一覧</summary>
        [Command("")]
        public async Task Root([FromServices] HelperService helper) => await GetTaskAsync(helper);

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
            var res = await helper.PostAsync(helper.GetUrl("RecordTask"), record);
            Console.WriteLine(res);
        }

        /// <summary>
        /// 指定したカテゴリのタスクを取得して、出力します。
        /// </summary>
        /// <param name="category">-c, 分類（半角英字で書くこと）</param>
        /// <returns></returns>
        [Command("task get")]
        public async Task GetTaskAsync([FromServices] HelperService helper, string category = "default")
        {
            using var client = new HttpClient();
            var res = await client.GetAsync($"{helper.GetUrl("GetTasks")}&partitionKey={category}");
            var json = await res.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<List<TodoTask>>(json);
            if (records == null || records.Count == 0)
            {
                Console.WriteLine("タスクの取得ができませんでした。");
                return;
            }
            Console.WriteLine($"{category}のタスク一覧");
            var i = 0;
            foreach (var record in records)
            {
                Console.WriteLine($"{i}:{record.Description}");
                i++;
            }
        }


        /// <summary>
        /// 指定した分類のタスクを全て削除します
        /// </summary>
        /// <param name="category">-c, 分類（半角英字で書くこと）</param>
        /// <returns></returns>
        [Command("task del all")]
        public async Task DeleteTaskAsync([FromServices] HelperService helper, string category = "default")
        {
            var url = $"{helper.GetUrl("DeleteTasks")}&partitionKey={category}";
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
        public async Task GetTaskAllAsync([FromServices] HelperService helper, string category = "default")
        {
            using var client = new HttpClient();
            var res = await client.GetAsync($"{helper.GetUrl("GetTasks")}&partitionKey={category}");
            var json = await res.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<List<TodoTask>>(json);
            if (records == null || records.Count == 0)
            {
                Console.WriteLine("タスクの取得ができませんでした。");
                return;
            }
            Console.WriteLine($"{category}のタスク一覧");
            var i = 0;
            foreach (var record in records)
            {
                Console.WriteLine($"{i}:{record}");
                i++;
            }
        }

        // privateにする
        /// <summary>
        /// 最後に登録したタスクを削除します
        /// </summary>
        /// <param name="category">-c, 分類（半角英字で書くこと）</param>
        /// <returns></returns>
        private async Task DeleteLastTaskAsync(HelperService helper, string category = "default")
        {
            // 最後のレコードを取得
            using var client = new HttpClient();
            var res = await client.GetAsync($"{helper.GetUrl("GetTasks")}&partitionKey={category}");
            var json = await res.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<List<TodoTask>>(json);
            if (records == null || records.Count == 0)
            {
                Console.WriteLine("タスクの取得ができませんでした。");
                return;
            }
            var last = records.Last();

            // IDを指定して削除
            var url = $"{helper.GetUrl("DeleteTask")}&partitionKey={category}&id={last.Id}";
            res = await client.DeleteAsync(url);
            Console.WriteLine(await res.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// 指定したカテゴリの何番目のタスクを削除します、指定が無ければdefaultカテゴリの最後のタスクを削除します
        /// </summary>
        /// <param name="index">-i, 0番目から指定, 無ければ最後のタスクを削除</param>
        /// <param name="category">-c, 分類（半角英字で書くこと）</param>
        /// <returns></returns>
        [Command("task del")]
        public async Task DeleteTaskAsync([FromServices] HelperService helper, int index = -1, string category = "default")
        {
            if (index < 0)
            {
                await DeleteLastTaskAsync(helper, category);
                return;
            }

            // IDを指定して削除
            using var client = new HttpClient();
            var url = $"{helper.GetUrl("DeleteByIndex")}&partitionKey={category}&index={index}";
            var res = await client.DeleteAsync(url);
            Console.WriteLine(await res.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// 登録されているタスクのカテゴリ一覧を取得します
        /// </summary>
        /// <returns></returns>
        [Command("task category")]
        public async Task GetTaskCategoriesAsync([FromServices] HelperService helper)
        {
            using var client = new HttpClient();
            var res = await client.GetAsync($"{helper.GetUrl("GetTaskCategories")}");
            var json = await res.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<List<string>>(json);
            if (records == null || records.Count == 0)
            {
                Console.WriteLine("タスクの取得ができませんでした。");
                return;
            }

            foreach (var record in records)
            {
                Console.WriteLine(record);
            }
        }
    }
}