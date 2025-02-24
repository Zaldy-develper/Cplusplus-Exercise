以下はネットワーキング演習研究の現在の進捗です。

■ 仕様の達成:
1. 受信情報の入力をチェックする : OK  
2. 受信処理を開始する : OK  
3. 接続要求を受け入れる : OK  
4. 送信後の受信処理 : NG  
  1. 問題: 受信したデータが不完全でした。  
  2. 例: テキストファイルを送ると、最後の行が受信されませんでした。  

```text
Source.txt:
  メッセージの1行目。
  メッセージの2行目。
  メッセージの3行目。
Received.txt:
  メッセージの1行目。
  メッセージの2行目。
```

■ Tera Termとの通信:
状態 (OK / NG) : 項目  
1. OK : 作成したアプリケーションで受信を始める。  
2. OK : TeraTermを起動して、対象のIPアドレスとポートに接続する。  
3. NG : TeraTermでデータを送る。  
  　問題: TeraTermクライアントから受信したデータが不完全でした。  
4. OK : TeraTermの接続が切断される。


Here is the current progress of Networking Exercise Research:

Specification Achievement:
1. Check the input of receive information. : OK
2. Start the receive process. : OK
3. Accepting a connection request. : OK
4. Receive processing after transmission. :  NG
	1. Problem Encountered: Incomplete Data Received.
	2. Example: Send a text.file but the last line of the contents are not received.
			
```text
Source.txt:
		line 1 of message.
		line 2 of message.
		line 3 of message.	
Received.txt:
		line 1 of message.
		line 2 of message.
```

Communication with Tera Term:
Status (OK / NG) : Item
	1. OK           :  Start receiving with the created application.
	2. OK           :  Start TeraTerm and connect to the target IP address and port.
	3. NG           :  Send data using TeraTerm.
					1. Issue: Incomplete data received from Tera Term Client.
	4.  OK          :  TeraTerm connection will be terminated.




