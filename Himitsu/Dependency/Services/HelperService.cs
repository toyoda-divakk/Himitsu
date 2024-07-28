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

        public string Key => Environment.GetEnvironmentVariable("BliFuncKey") ?? "";
        public string EndPoint => Environment.GetEnvironmentVariable("BliFuncEndpoint") ?? "";
        public string GetUrl(string function) => $"{EndPoint}{function}?code={Key}";

    }
}
