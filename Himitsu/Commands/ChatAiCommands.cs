using BliFunc.Models;
using ConsoleAppFramework;
using Himitsu.Dependency.Services;
using System.Text;

namespace Himitsu.Commands
{
    // 会話履歴などはここで持っておく。永久保存はCosmosDBで。
    // Function Callingによって更新などを行う場合、ChatGPTで生成した更新内容をいったんここで表示してHelperServiceで確認を行う。OKの場合はAzureFunctionの更新関数に更新内容を送信する。という流れ。

    // TODO:とりあえずシンプルに会話できるやつを作るか
    /// <summary>
    /// AI関係
    /// チャットによってAIとやり取りするコマンド
    /// </summary>
    public class ChatAiCommands 
    {
        // 会話用プロンプトファイルを指定して、新しく会話を開始する。

        // 前回の会話をDBから読み込んで、続きから会話を開始する。

        // 会話を1回分入力して、応答を表示する。

        // 会話の要約をDBの情報テーブルに格納する。

        // TODO:1つコマンドを作成したらProgram.csに登録すること。


        ///// <summary>
        ///// 今回の会話をリセットする。
        ///// </summary>
        ///// <param name="helper"></param>
        //[Command("chat reset")]
        //public void Reset([FromServices] HelperService helper)
        //{
        //    Console.WriteLine("Reset");
        //}

        ///// <summary>
        ///// 指定されたDBに会話を永続化する。
        ///// </summary>
        ///// <param name="helper"></param>
        //[Command("chat save")]
        //public void Save([FromServices] HelperService helper)
        //{
        //    Console.WriteLine("Save");
        //}

        ///// <summary>
        ///// 利用可能なプロンプトファイルの一覧を表示する。
        ///// </summary>
        ///// <param name="helper"></param>
        //[Command("chat list")]
        //public void List([FromServices] HelperService helper)
        //{
        //    Console.WriteLine("List");
        //}

    }
}