using BliFunc.Models;
using ConsoleAppFramework;
using Himitsu.Dependency.Services;
using System.Text;

namespace Himitsu.Commands
{

    /// <summary>
    /// AI関係
    /// </summary>
    public class AiCommands 
    {
        /// <summary>
        /// AIにメッセージを送信し、応答を表示します。
        /// </summary>
        /// <param name="message">-m, 送信メッセージ</param>
        [Command("ai")]
        public void Message([FromServices] HelperService helper, string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        ///プロンプトファイルを指定してAIとの会話を開始する。
        /// </summary>
        /// <param name="promptfile">-p, プロンプトファイル名</param>
        [Command("ai prompt")]
        public void Prompt([FromServices] HelperService helper, string promptfile)
        {
            Console.WriteLine(promptfile);
        }

        /// <summary>
        /// 会話の履歴を一回リセットする。
        /// </summary>
        /// <param name="helper"></param>
        [Command("ai reset")]
        public void Reset([FromServices] HelperService helper)
        {
            Console.WriteLine("Reset");
        }

        /// <summary>
        /// 指定されたプロンプトファイルに会話を保存する。
        /// </summary>
        /// <param name="helper"></param>
        [Command("ai save")]
        public void Save([FromServices] HelperService helper)
        {
            Console.WriteLine("Save");
        }

        /// <summary>
        /// 利用可能なプロンプトファイルの一覧を表示する。
        /// </summary>
        /// <param name="helper"></param>
        [Command("ai list")]
        public void List([FromServices] HelperService helper)
        {
            Console.WriteLine("List");
        }
    }
}