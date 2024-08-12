using Himitsu.Dependency.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Himitsu.Dependency.Services
{
    public class HelperService : IHelperService
    {
        public async Task<string> PostAsync<T>(string url, T record)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(record);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync(url, content);
            return await res.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAsync(string url)
        {
            using var client = new HttpClient();
            var res = await client.GetAsync(url);
            return await res.Content.ReadAsStringAsync();
        }

        public string Key => Environment.GetEnvironmentVariable("BliFuncKey") ?? "";
        public string EndPoint => Environment.GetEnvironmentVariable("BliFuncEndpoint") ?? "";
        public string GetUrl(string function) => $"{EndPoint}{function}?code={Key}";

        /// <summary>
        /// 質問を表示し、回答を(y/n)で取得する。
        /// </summary>
        /// <param name="message">質問</param>
        /// <returns>yならばtrue</returns>
        private bool ConfirmAction(string message)
        {
            while (true)
            {
                Console.Write($"{message} (y/n): ");
                string answer = Console.ReadLine()!.ToLower();

                if (answer.ToLower() == "y")
                {
                    return true;
                }
                else if (answer.ToLower() == "n")
                {
                    return false;
                }

                Console.WriteLine("無効な入力です！ yかnを入力してください。");
            }
        }
    }
}
