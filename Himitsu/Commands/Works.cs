using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppFramework;

namespace Himitsu.Commands
{
    // TrackRecord: AzureFunctionで1日の工数実績を送信する。issue番号、時間、日付（オプション）、コメント（オプション）を指定する。
    // Aggregation: AzureFunctionで指定した期間の工数実績を集計する。


    /// <summary>
    /// 仕事を手伝ってくれるコマンド
    /// </summary>
    public class Works
    {
        /// <summary>Sum parameters.</summary>
        /// <param name="x">left value.</param>
        /// <param name="y">right value.</param>
        public void Dummy(int x, int y) => Console.WriteLine(x + y);
    }

}
