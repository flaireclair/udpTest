using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Script.SocketServer
{
    public class ClientSocket : SocketServer
    {
        public int port = 11188;
        private NetworkStream stream = null;
        public static DATA data;
        public static TcpListener tcpserver;
        public static TcpClient tcp;
        private readonly List<TcpClient> clients = new List<TcpClient>();
        private string ipAddress;
        // Start is called before the first frame update
        void Start()
        {
            // 接続中のIPアドレスを取得
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
            // 指定したポートを開く
            Listen("192.168.11.31", port);
            Debug.Log("now Listen Start! : " + ipAddress);  
        }

        // Update is called once per frame
        void Update()
        {

        }

        

        public static void Socket()
        {
            Thread thread = new Thread(() =>
            {
                NetworkStream stream = tcp.GetStream(); 
                try
                {
                    byte[] bytes = new byte[tcp.ReceiveBufferSize];
                    stream.Read(bytes, 0, bytes.Length);
                    Debug.Log(Encoding.UTF8.GetString(bytes));
                    stream.Write(bytes, 0, bytes.Length);
                    data = new DATA(bytes);
                }
                catch (Exception) { }
                if(data.ToVector3() == new Vector3(0,0,0)) stream.Close();
            });
            thread.Start();
        }

        /*private NetworkStream GetNetworkStream()
        {
          if (stream != null && stream.CanRead)
          {
            return stream;
          }

          TcpClient tcp = new TcpClient(ipOrHost, port);
          Debug.Log("success conn server");

          return tcp.GetStream();
        }*/
    }
}