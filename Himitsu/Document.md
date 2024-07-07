## イメージ
Himitsu SayHello
現在のAIに挨拶と自分の機能を紹介してもらう。（Functionをたたく）

Himitsu Setup
接続テスト。環境変数の確認をして、取れなかったらappsettingsの確認。だめだったら設定手順を表示。

Himitsu TestFunction
指定した名前のAzure Functionを呼び出し、その結果を出力表示する。


# どうやって効率よく作るか？
まず最低限のインターフェースを作って、そこから膨らませる。

持ってる環境をAIプロンプトに説明して、実装手順を洗い出してもらう。
CosmosDBに現在の開発環境の情報をkey-valueで登録しておいて、読んでもらう。

# やることリスト
AzureFunctionからAIに接続。応答はstringで返してコンソール出力する。  
1問1答形式のpromptyを読み込ませて、回答させる。（最初はprompty固定で）
会話ログ付きの応答に挑戦。最初はprompt無し。
DBに会話ログの記録。

ついでに、やることリストもDBに登録して、AIに読めるようにしておこう。

# 説明書
システム環境変数に以下を追加します。
- "BliFuncKey"  
Azure関数アプリのホストキーを登録してください。
- "BliFuncEndpoint"  
Azure関数アプリのエンドポイントを登録してください。
- "Path"  
"E:\Program Files\Himitsu"を追加してください。  

ビルドしたら、ビルドスクリプトにより"E:\Program Files\Himitsu"に自動でコピーします。
そしたら、コマンドプロンプトから"Himitsu"と打つと、プログラムが起動します。


# 業務用の改善
とくになし

# 引っかかったところ
## Postするメソッドが、応答が来る前に終了してしまう
メソッドがasyncの時は戻り値をvoidにしてしまうと待機せずにメソッドが終了してしまうので、Taskにすること。

