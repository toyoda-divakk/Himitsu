using ConsoleAppFramework;
// ビルド後イベントを設定しているので、ビルドしたら"E:\Program Files\Himitsu"に自動でコピーします。
// 環境変数のPathに"E:\Program Files\Himitsu"を追加する。

//// ---------- ---------- 1つだけ実装する場合。 ---------- ----------
// 使い方：コマンドプロンプトで、cdでディレクトリを移動して、以下を実行
// Himitsu --foo 10 --bar 20

// ----------

//ConsoleApp.Run(args, (int foo, int bar) => Console.WriteLine($"Sum: {foo + bar}"));

// ----------

//// ラムダ式を使用して非同期メソッドを定義する場合
//await ConsoleApp.RunAsync(args, async (int foo, int bar, CancellationToken cancellationToken) =>
//{
//    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
//    Console.WriteLine($"Sum: {foo + bar}");
//});

// 引数が渡されない場合は、デフォルトではヘルプ表示が呼び出されます。

// ----------

//// ドキュメントコメントをヘルプに表示する場合。
//ConsoleApp.Run(args, Commands.Hello);
//static class Commands
//{
//    /// <summary>
//    /// Display Hello.
//    /// </summary>
//    /// <param name="message">-m, Message to show.</param>
//    public static void Hello(string message) => Console.Write($"Hello, {message}");
//}

// ---------- ---------- ---------- ここから複数コマンドを実装する場合 ---------- ---------- ----------

//var app = ConsoleApp.Create();

//app.Add("", (string msg) => Console.WriteLine(msg));
//app.Add("echo", (string msg) => Console.WriteLine(msg));
//app.Add("sum", (int x, int y) => Console.WriteLine(x + y));

//// Himitsu --msg aaaa
//// Himitsu echo --msg aaaa
//// Himitsu sum --x 1 --y 2
//app.Run(args);

// ---------- 結構ちゃんとした書き方
var app = ConsoleApp.Create();
app.Add<MyCommands>();
app.Run(args);

public class MyCommands
{
    /// <summary>Root command test.</summary>
    /// <param name="msg">-m, Message to show.</param>
    [Command("")]
    public void Root(string msg) => Console.WriteLine(msg);

    /// <summary>Display message.</summary>
    /// <param name="msg">Message to show.</param>
    public void Echo(string msg) => Console.WriteLine(msg);

    /// <summary>Sum parameters.</summary>
    /// <param name="x">left value.</param>
    /// <param name="y">right value.</param>
    public void Sum(int x, int y) => Console.WriteLine(x + y);
}

