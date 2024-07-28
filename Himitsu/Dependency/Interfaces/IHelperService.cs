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

        /// <summary>
        /// 環境変数から"BliFuncKey"の値を取得する。
        /// Azure関数アプリのmaster（ホスト）キー。
        /// </summary>
        string Key { get; }
        string EndPoint { get; }

        /// <summary>
        /// HelperServiceのURLを取得する。
        /// </summary>
        /// <param name="function">関数名</param>
        /// <returns>URL</returns>
        string GetUrl(string function);
    }
}
