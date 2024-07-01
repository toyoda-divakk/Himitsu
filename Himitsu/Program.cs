using ConsoleAppFramework;
using Himitsu.Commands;
// ビルド後イベントを設定しているので、ビルドしたら"E:\Program Files\Himitsu"に自動でコピーします。
// 環境変数のPathに"E:\Program Files\Himitsu"を追加する。

// システム環境設定に"BliFuncKey"

var app = ConsoleApp.Create();
app.Add<MyCommands>();
app.Add<Works>();
app.Run(args);

public class MyCommands // IDisposable, IAsyncDisposableがついていたら、コマンド実行後にDisposeされる。
{
    /// <summary>Root command test.</summary>
    /// <param name="msg">-m, Message to show.</param>
    public void Root(string msg) => Console.WriteLine(msg);     // ※Command属性を使用して名前を任意に変更することもできる。メソッド名が長くなり過ぎた時なんかに。

    /// <summary>Display message.</summary>
    /// <param name="msg">Message to show.</param>
    public void Echo(string msg) => Console.WriteLine(msg);

    /// <summary>Sum parameters.</summary>
    /// <param name="x">left value.</param>
    /// <param name="y">right value.</param>
    public void Sum(int x, int y) => Console.WriteLine(x + y);

    // 引数のURLに対してHttpGetを行い、結果を表示する。
    public async Task Get(string url)
    {
        using var client = new HttpClient();
        var res = await client.GetAsync(url);
        Console.WriteLine(await res.Content.ReadAsStringAsync());
    }
    /// <summary>
    /// Function1のテスト
    /// </summary>
    /// <returns>Function1の実行結果</returns>
    public async Task GetTest() => await Get($"https://blifunc.azurewebsites.net/api/Function1?code={GetKey()}");  // Himitsu get-test

    // 環境変数から"BliFuncKey"の値を取得する。Azure関数アプリのmaster（ホスト）キー。
    private string GetKey() => Environment.GetEnvironmentVariable("BliFuncKey") ?? "";    // TODO:あとで、無かった場合はappSettings.jsonから取得するように変更する。それで無かった場合はエラーメッセージを出す。
}




