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
			TcpListener listener = (TcpListener)ar.AsyncState;
			TcpClient client = listener.EndAcceptTcpClient(ar);
			_clients.Add(client);
			Debug.Log("Connect: " + client.Client.RemoteEndPoint);

			// 接続が確立したら次の人を受け付ける
			listener.BeginAcceptSocket(DoAcceptTcpClientCallback, listener);

			// 今接続した人とのネットワークストリームを取得
			NetworkStream stream = client.GetStream();
			StreamReader reader = new StreamReader(stream,Encoding.UTF8);

            // 一行分の文字列を受け取る
            byte[] bytes = new byte[12];
            int resSize = 0;

            // 接続が切れるまで送受信を繰り返す
            do
            {
                Debug.Log("a");
                Debug.Log(client.Connected);
                Debug.Log("Byte Length : " + bytes.Length);
                resSize = stream.Read(bytes, 0, bytes.Length);
                Debug.Log("b");
                Debug.Log(resSize);
                OnMessage(bytes, stream);

                // クライアントの接続が切れたら
                if (resSize == 0 || (client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0)))
                {
                    Debug.Log("Disconnect: " + client.Client.RemoteEndPoint);
                    client.Close();
                    _clients.Remove(client);
                    break;
                }
            } while (stream.DataAvailable || bytes[resSize - 1] != '\n');

        }


		// メッセージ受信
		public virtual void OnMessage(byte[] msg, NetworkStream stream){
			Debug.Log(BitConverter.ToString(msg));
		}

		// クライアントにメッセージ送信
		public static void SendMessageToClient(string msg, NetworkStream stream)
    {
			if (_clients.Count == 0){
				return;
			}

            var sendByte = Encoding.UTF8.GetBytes(msg);

            // 全員に同じメッセージを送る
            foreach (var client in _clients){
				try
                {
					stream.Write(sendByte, 0, sendByte.Length);
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