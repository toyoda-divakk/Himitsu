using ConsoleAppFramework;
// ビルド後イベントを設定しているので、ビルドしたら"E:\Program Files\Himitsu"に自動でコピーします。
// 環境変数のPathに"E:\Program Files\Himitsu"を追加する。

var app = ConsoleApp.Create();
app.Add<MyCommands>();
app.Run(args);

public class MyCommands // IDisposable, IAsyncDisposableがついていたら、コマンド実行後にDisposeされる。
{
    /// <summary>Root command test.</summary>
    /// <param name="msg">-m, Message to show.</param>
    [Command("")]
    public void Root(string msg) => Console.WriteLine(msg);     // ※Command属性を使用して名前を任意に変更することもできる。メソッド名が長くなり過ぎた時なんかに。

    /// <summary>Display message.</summary>
    /// <param name="msg">Message to show.</param>
    public void Echo(string msg) => Console.WriteLine(msg);

    /// <summary>Sum parameters.</summary>
    /// <param name="x">left value.</param>
    /// <param name="y">right value.</param>
    public void Sum(int x, int y) => Console.WriteLine(x + y);
}




