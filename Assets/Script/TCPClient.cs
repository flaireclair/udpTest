using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;

public class TCPClient : MonoBehaviour
{
    public string ipaddr = "127.0.0.1";
    public int port = 11188;
    private NetworkStream ns;
    private Thread clientReceiveThread;
    private TcpClient tcp;

    // Start is called before the first frame update
    void Start()
    {
        //TcpClientを作成し、サーバーと接続する
        tcp = new TcpClient(ipaddr, port);
        Debug.Log("サーバー( " + ((IPEndPoint)tcp.Client.RemoteEndPoint).Address + " : " +
            ((IPEndPoint)tcp.Client.RemoteEndPoint).Port + " )と接続しました( " +
            ((IPEndPoint)tcp.Client.LocalEndPoint).Address + " : " +
            ((IPEndPoint)tcp.Client.LocalEndPoint).Port + " )。");

        

        //NetworkStreamを取得する
        ns = tcp.GetStream();

        //読み取り、書き込みのタイムアウトを10秒にする
        //デフォルトはInfiniteで、タイムアウトしない
        //(.NET Framework 2.0以上が必要)
        ns.ReadTimeout = 10000;
        ns.WriteTimeout = 10000;

        try
        {
            clientReceiveThread = new Thread(new ThreadStart(DoAcceptTcpClientCallback));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    // 接続処理
    private void DoAcceptTcpClientCallback()
    {
        while(true)
        {
            // 一行分の文字列を受け取る
            byte[] bytes = new byte[tcp.ReceiveBufferSize];

            var reader = new StreamReader(ns, Encoding.UTF8);

            // Read incomming stream into byte arrary. 					
            while (!reader.EndOfStream)
            {
                ns.Read(bytes, 0, bytes.Length);
                ns.Write(bytes, 0, bytes.Length);
                Debug.Log("server message received as: " + bytes);
            }

            // 接続が切れたら
            if (tcp.Client.Poll(1000, SelectMode.SelectRead) && (tcp.Client.Available == 0))
            {
                Debug.Log("Disconnect: " + tcp.Client.RemoteEndPoint);
                tcp.Close();
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
