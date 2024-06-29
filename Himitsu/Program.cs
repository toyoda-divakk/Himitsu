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

// 属性によるバリデーション
// あと、正常終了の終了コードは0、異常終了の終了コードは1になります。
// ConsoleApp.Run(args, ([EmailAddress] string firstArg, [Range(0, 2)] int secondArg) => { });

// ログ
// ConsoleApp.Log

// ---------- ---------- ---------- フィルタ ---------- ---------- ----------

// フィルタクラスを作ると、[ConsoleAppFilter<フィルタクラス>]をClassやMethodに付けることで、実行の前後に処理をフックできる。
// class LogRunningTimeFilter(ConsoleAppFilter next) : ConsoleAppFilter(next)


// 後の処理(Nextに値を渡す方法)

//internal class AuthenticationFilter(ConsoleAppFilter next) : ConsoleAppFilter(next)
//{
//    public override async Task InvokeAsync(ConsoleAppContext context, CancellationToken cancellationToken)
//    {
//        var requestId = Guid.NewGuid();
//        var userId = await GetUserIdAsync();

//        // setup new state to context
//        var authedContext = context with { State = new ApplicationContext(requestId, userId) };
//        await Next.InvokeAsync(authedContext, cancellationToken);
//    }

//    // get user-id from DB/auth saas/others
//    async Task<int> GetUserIdAsync()
//    {
//        await Task.Delay(TimeSpan.FromSeconds(1));
//        return 1999;
//    }
//}

//record class ApplicationContext(Guid RequiestId, int UserId);

// ※ConsoleAppContext contextを後処理に引数に加えることで受け取れる。string[] argsでも良い。
// プロジェクト間でのフィルターの共有する場合は、ConsoleAppFramework.AbstractionsをNugetする。

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

// void以外の戻り値は終了コード。int, Task<int>が使える。

// ---------- ---------- ---------- DI ---------- ---------- ----------
// DIを使う場合は、Microsoft.Extensions.DependencyInjectionとかでやる。

//var services = new ServiceCollection();
//services.AddTransient<MyService>();

//using var serviceProvider = services.BuildServiceProvider();

//// Any DI library can be used as long as it can create an IServiceProvider
//ConsoleApp.ServiceProvider = serviceProvider;

//// When passing to a lambda expression/method, using [FromServices] indicates that it is passed via DI, not as a parameter
//ConsoleApp.Run(args, ([FromServices] MyService service, int x, int y) => Console.WriteLine(x + y));


// ---------- ---------- ---------- デフォルトのログ機能 ---------- ---------- ----------
//using var serviceProvider = services.BuildServiceProvider(); // using for cleanup(important)
//ConsoleApp.ServiceProvider = serviceProvider;

//// setup ConsoleApp system logger
//var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
//ConsoleApp.Log = msg => logger.LogInformation(msg);
//ConsoleApp.LogError = msg => logger.LogError(msg);

// ---------- ---------- ---------- appsettings.json ---------- ---------- ----------
// Microsoft.Extensions.Configuration.Json

//{
//    "Position": {
//        "Title": "Editor",
//        "Name": "Joe Smith"
//    },
//  "MyKey": "My appsettings.json Value",
//  "AllowedHosts": "*"
//}

//// Package Import: Microsoft.Extensions.Configuration.Json
//var configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json")
//    .Build();

//// Bind to services( Package Import: Microsoft.Extensions.Options.ConfigurationExtensions )
//var services = new ServiceCollection();
//services.Configure<PositionOptions>(configuration.GetSection("Position"));

// public class MyCommand(IOptions<PositionOptions> options)
// options.Value.Title





