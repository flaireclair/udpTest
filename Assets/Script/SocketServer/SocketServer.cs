using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/*
 * SocketServer.cs
 * ソケット通信（サーバ）
 * Unityアプリ内にサーバを立ててメッセージの送受信を行う
 */
namespace Script.SocketServer
{
	public class SocketServer : MonoBehaviour {
		private TcpListener listener;
		private static readonly List<TcpClient> _clients = new List<TcpClient>();

		// ソケット接続準備、待機
		protected void Listen(string host, int port){
			Debug.Log("ipAddress:"+host+" port:"+port);
			var ip = IPAddress.Parse(host);
		    listener = new TcpListener(ip, port);
			listener.Start();
			listener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, listener);
		}
		
		// クライアントからの接続処理
		private void DoAcceptTcpClientCallback(IAsyncResult ar) {

            // 今接続した人とのネットワークストリームを取得
            using (TcpClient client = listener.EndAcceptTcpClient(ar))
            using (NetworkStream stream = client.GetStream())
            {
                Debug.Log("Connect: " + client.Client.RemoteEndPoint);
                string responce = string.Empty;

                using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
                {
                    // 一行分の文字列を受け取る
                    byte[] bytes = new byte[12];
                    int resSize = 0;
                    do
                    {
                        int numBytesRead = 0;
                        // 接続が切れるまで送受信を繰り返す
                        Debug.Log("a");
                        Debug.Log(client.Connected);
                        Debug.Log("Byte Length : " + bytes.Length);
                        int n = reader.Read(bytes, numBytesRead, bytes.Length);
                        numBytesRead += n;
                        resSize = numBytesRead;
                        // クライアントの接続が切れたら
                        if (numBytesRead == 0 || (client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0)))
                        {
                            Debug.Log("Disconnect1: " + client.Client.RemoteEndPoint);
                            client.Close();
                            _clients.Remove(client);
                            break;
                        }
                        Debug.Log("b");
                        Debug.Log(resSize);
                        OnMessage(bytes, writer);
                    } while (stream.DataAvailable || bytes[resSize - 1] != '\n');
                }
            }
        }


		// メッセージ受信
		public virtual void OnMessage(byte[] msg, BinaryWriter writer)
        {
			Debug.Log(BitConverter.ToString(msg));

            Debug.Log(Encoding.UTF8.GetString(msg));
            ClientSocket.data = new DATA(msg);

            Vector3 response = ClientSocket.data.ToVector3();

            // クライアントに受領メッセージを返す
            SendMessageToClient(("Accept: x:" + response.x + ", y: " + response.y + ", z:" + response.z + "\n"), writer);
        }

		// クライアントにメッセージ送信
		public static void SendMessageToClient(string msg, BinaryWriter writer)
        {
			if (_clients.Count == 0){
				return;
			}

            var sendByte = Encoding.UTF8.GetBytes(msg);

            // 全員に同じメッセージを送る
            foreach (var client in _clients){
				try
                {
					writer.Write(sendByte, 0, 2);
                    Debug.Log("Send!");
				}
                catch
                {
					_clients.Remove(client);
				}
			}
		}

		// 終了処理
		protected virtual void OnApplicationQuit() {
			if (listener == null){
				return;
			}

			if (_clients.Count != 0){
				foreach(var client in _clients){
					client.Close();
				}
			}
			listener.Stop();
		}

	}
}