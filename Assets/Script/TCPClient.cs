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
    private DATA data;

    [SerializeField]
    private GameObject obj;
    private Transform tran;
    private Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        tran = obj.transform;
        Debug.Log(tran.position);
        pos = tran.position;
        //TcpClientを作成し、サーバーと接続する
        tcp = new TcpClient(ipaddr, port);
        var ip = IPAddress.Parse(ipaddr);
        tcp.BeginConnect(ip, port, DoAcceptTcpClientCallback, tcp);
        Debug.Log("サーバー( " + ((IPEndPoint)tcp.Client.RemoteEndPoint).Address + " : " +
            ((IPEndPoint)tcp.Client.RemoteEndPoint).Port + " )と接続しました( " +
            ((IPEndPoint)tcp.Client.LocalEndPoint).Address + " : " +
            ((IPEndPoint)tcp.Client.LocalEndPoint).Port + " )。");
        //DoAcceptTcpClientCallback();

        /*try
        {
            clientReceiveThread = new Thread(new ThreadStart(DoAcceptTcpClientCallback));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }*/
    }

    private void FixedUpdate()
    {
        pos = tran.position;
    }

    // 接続処理
    private void DoAcceptTcpClientCallback(IAsyncResult ar)
    {
        Debug.Log(1);
        while(true)
        {
            
            Debug.Log(2);
            // 一行分の文字列を受け取る
            byte[] bytes = new byte[tcp.SendBufferSize];
            Debug.Log(3);
            //var reader = new StreamReader(ns, Encoding.UTF8);
            Debug.Log(6);
            Debug.Log(pos);
            data = new DATA(pos);
            Debug.Log(7);
            bytes = data.ToByte();
            Debug.Log(BitConverter.ToString(bytes));
            Debug.Log("Byte Length : " + bytes.Length);
            int resSize = 0;
            //ns = tcp.GetStream();


            // Read incomming stream into byte arrary. 					
            do
            {
                Debug.Log(4);
                ns.Write(bytes, 0, bytes.Length);
                Debug.Log(8);
                resSize =  ns.Read(bytes, 0, bytes.Length);
                Debug.Log("server message received as: " + bytes);
            } while (ns.DataAvailable);

            Debug.Log(5);

            // 接続が切れたら
            if (resSize == 0 || (tcp.Client.Poll(1000, SelectMode.SelectRead) && (tcp.Client.Available == 0)))
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
