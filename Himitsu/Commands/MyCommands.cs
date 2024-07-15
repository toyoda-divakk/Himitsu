using BliFunc.Models;
using ConsoleAppFramework;
using System.Text;

namespace Himitsu.Commands
{

    /// <summary>
    /// 練習用。
    /// 本番では消す。
    /// </summary>
    public class MyCommands // IDisposable, IAsyncDisposableがついていたら、コマンド実行後にDisposeされる。
    {
        /// <summary>デフォルトのコマンド：オウム返し</summary>
        /// <param name="msg">-m, Message to show.</param>
        [Command("")]
        public void Root(string msg) => Console.WriteLine(msg);     // ※Command属性を使用して名前を任意に変更することもできる。メソッド名が長くなり過ぎた時なんかに。

        ///// <summary>
        ///// 引数のURLに対してHttpGetを行い、結果を表示する。
        ///// </summary>
        ///// <param name="url">-u, URL</param>
        ///// <returns></returns>
        //public async Task Get(string url)
        //{
        //    using var client = new HttpClient();
        //    var res = await client.GetAsync(url);
        //    Console.WriteLine(await res.Content.ReadAsStringAsync());
        //}

        ///// <summary>
        ///// 環境変数から"BliFuncKey"の値を取得する。
        ///// Azure関数アプリのmaster（ホスト）キー。
        ///// </summary>
        //private string Key => Environment.GetEnvironmentVariable("BliFuncKey") ?? "";
        //private string EndPoint => Environment.GetEnvironmentVariable("BliFuncEndpoint") ?? "";
        ///// <summary>
        ///// Function1のテスト
        ///// </summary>
        ///// <returns>Function1の実行結果</returns>
        //public async Task GetTest() => await Get($"{EndPoint}Function1?code={Key}");  // Himitsu get-test


    }
}