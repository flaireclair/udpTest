using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO;
using System.Text;
using UnityEngine;

public class TCPServer: MonoBehaviour
{
    //ListenするIPアドレス
    string ipString = "127.0.0.1";

    //ホスト名からIPアドレスを取得する時は、次のようにする
    //string host = "localhost";
    //System.Net.IPAddress ipAdd =
    //    System.Net.Dns.GetHostEntry(host).AddressList[0];
    //.NET Framework 1.1以前では、以下のようにする
    //System.Net.IPAddress ipAdd =
    //    System.Net.Dns.Resolve(host).AddressList[0];

    //Listenするポート番号
    int port = 11188;
    private static readonly List<TcpClient> clients = new List<TcpClient>();

    private TcpListener listener;


    // Start is called before the first frame update
    void Start()
    {
        // 接続中のIPアドレスを取得
        var ipAddress = "0.0.0.0";
        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddress = ip.Address.ToString();
                        Debug.Log(ipAddress);
                    }
                }
            }
        }

        //System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(ipString);
        //TcpListenerオブジェクトを作成する
        listener = new TcpListener(IPAddress.Any, port);

        //Listenを開始する
        listener.Start();
        listener.BeginAcceptSocket(DoAcceptTcpClientCallback, listener);
        //Console.WriteLine("Listenを開始しました({0}:{1})。",
        //    ((System.Net.IPEndPoint)listener.LocalEndpoint).Address,
        //    ((System.Net.IPEndPoint)listener.LocalEndpoint).Port);
        Debug.Log("Listenを開始しました( " +
            ((IPEndPoint)listener.LocalEndpoint).Address + " : " +
            ((IPEndPoint)listener.LocalEndpoint).Port + ")。");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // クライアントからの接続処理
    private void DoAcceptTcpClientCallback(IAsyncResult ar)
    {
        var listener = (TcpListener)ar.AsyncState;
        var client = listener.EndAcceptTcpClient(ar);
        clients.Add(client);
        Debug.Log("Connect: " + client.Client.RemoteEndPoint);

        // 接続が確立したら次の人を受け付ける
        listener.BeginAcceptSocket(DoAcceptTcpClientCallback, listener);

        // 今接続した人とのネットワークストリームを取得
        var stream = client.GetStream();
        var reader = new StreamReader(stream, Encoding.UTF8);

        // 接続が切れるまで送受信を繰り返す
        while (client.Connected)
        {
            while (!reader.EndOfStream)
            {
                // 一行分の文字列を受け取る
                byte[] bytes = new byte[client.ReceiveBufferSize];
                stream.Read(bytes, 0, bytes.Length);
                stream.Write(bytes, 0, bytes.Length);
                OnMessage(bytes);
            }

            // クライアントの接続が切れたら
            if (client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0))
            {
                Debug.Log("Disconnect: " + client.Client.RemoteEndPoint);
                client.Close();
                clients.Remove(client);
                break;
            }
        }
    }

    // メッセージ受信
    public void OnMessage(byte[] bytes)
    {
        Debug.Log(bytes);
        DATA tran = new DATA(bytes);
        ChangeParams1.gameObject.transform.position = tran.ToVector3();
    }

    // クライアントにメッセージ送信
    public void Send(byte[] sendByte)
    {
        if (clients.Count == 0)
        {
            return;
        }
        // 全員に同じメッセージを送る
        foreach (var client in clients)
        {
            try
            {
                var stream = client.GetStream();
                stream.Write(sendByte, 0, sendByte.Length);
            }
            catch
            {
                clients.Remove(client);
            }
        }
    }

    // 終了処理
    protected virtual void OnApplicationQuit()
    {
        if (listener != null) listener.Stop();
        if (clients != null) foreach (var client in clients) client.Close();
    }
}
