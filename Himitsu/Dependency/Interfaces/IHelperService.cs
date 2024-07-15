using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Himitsu.Dependency.Interfaces
{
    public interface IHelperService
    {
        /// <summary>
        /// HttpPostを行い、結果を表示する。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="record">送信する情報</param>
        /// <returns></returns>
        Task<string> PostAsync<T>(string url, T record);
    }
}
