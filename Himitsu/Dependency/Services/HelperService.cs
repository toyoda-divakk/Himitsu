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
        /// <summary>
        /// HttpPostを行い、結果を表示する。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="record">送信する情報</param>
        /// <returns></returns>
        public async Task<string> PostAsync<T>(string url, T record)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(record);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync(url, content);
            return await res.Content.ReadAsStringAsync();
        }
    }
}
