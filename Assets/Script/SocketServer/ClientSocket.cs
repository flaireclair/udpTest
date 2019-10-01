using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Net;
using System.Text;

namespace Script.SocketServer
{
    public class ClientSocket : MonoBehaviour
    {
        public int port = 11188;
        private NetworkStream stream = null;
        public static DATA data;
        public static TcpListener tcpserver;
        public static TcpClient tcp;
        // Start is called before the first frame update
        void Start()
        {
            tcpserver = new TcpListener(IPAddress.Any, port);
            tcpserver.Start();
            tcp = tcpserver.AcceptTcpClient();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public static void Socket(int port)
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
                stream.Close();
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