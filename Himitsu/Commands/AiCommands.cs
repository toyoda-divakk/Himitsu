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
        /// AI関数にメッセージを送信し、応答を表示します。
        /// </summary>
        /// <param name="name">-n, 使用するAI, ai listで一覧取得。</param>
        /// <param name="message">-m, 送信メッセージ</param>
        [Command("ai")]
        public async Task MessageAsync([FromServices] HelperService helper, string name, string message)
        {
            // データの送信
            var res = await helper.PostAsync(helper.GetUrl(name), message); // 関数名の指定に変更すること
            Console.WriteLine(res);
        }

        /// <summary>
        /// AIのpromptyファイルを指定してメッセージを送信し、応答を表示します。
        /// </summary>
        /// <param name="name">-n, プロンプトファイル名、拡張子無し、なければドンペン</param>
        /// <param name="message">-m, 送信メッセージ</param>
        [Command("ai prompty")]
        public async Task PromptyMessageAsync([FromServices] HelperService helper, string? name, string message)
        {
            name ??= "AiDonpen";

            // データの送信
            var res = await helper.PostAsync($"{helper.GetUrl("Ai")}&name={name}", message);
            Console.WriteLine(res);
        }

        /// <summary>
        /// aiコマンドで使用できるname一覧を取得します。
        /// </summary>
        [Command("ai list")]
        public async Task AiListAsync([FromServices] HelperService helper)
        {
            // データの取得
            var res = await helper.GetAsync(helper.GetUrl("AiList"));
            Console.WriteLine(res);
        }

        // Himitsu ai file -n AiDonpen < 入力ファイル.txt > 出力ファイル.txt
        /// <summary>
        /// AIにテキストファイルを入力し、応答を表示します。 
        /// </summary>
        /// <param name="name">-n, 使用するAI名、なければドンペン</param>
        [Command("ai file")]
        public async Task FileInputToAi([FromServices] HelperService helper, string? name)
        {
            var inputData = Console.In.ReadToEnd();
            name ??= "AiDonpen";

            // データの送信
            var res = await helper.PostAsync(helper.GetUrl(name), inputData);
            Console.WriteLine(res);
        }


        ///// <summary>
        /////プロンプトファイルを指定してAIとの会話を開始する。
        ///// </summary>
        ///// <param name="promptfile">-p, プロンプトファイル名</param>
        //[Command("ai prompt")]
        //public void Prompt([FromServices] HelperService helper, string promptfile)
        //{
        //    Console.WriteLine(promptfile);
        //}

        ///// <summary>
        ///// 会話の履歴を一回リセットする。
        ///// </summary>
        ///// <param name="helper"></param>
        //[Command("ai reset")]
        //public void Reset([FromServices] HelperService helper)
        //{
        //    Console.WriteLine("Reset");
        //}

        ///// <summary>
        ///// 指定されたプロンプトファイルに会話を保存する。
        ///// </summary>
        ///// <param name="helper"></param>
        //[Command("ai save")]
        //public void Save([FromServices] HelperService helper)
        //{
        //    Console.WriteLine("Save");
        //}

        ///// <summary>
        ///// 利用可能なプロンプトファイルの一覧を表示する。
        ///// </summary>
        ///// <param name="helper"></param>
        //[Command("ai list")]
        //public void List([FromServices] HelperService helper)
        //{
        //    Console.WriteLine("List");
        //}



    }
}