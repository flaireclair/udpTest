using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Threading;

namespace Script.SocketServer
{
    public class ClientSocket : MonoBehaviour
    {
        public string ipOrHost = "127.0.0.1";
        public int port = 11188;
        private NetworkStream stream = null;
        public static DATA data;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Socket(string ip, int port)
        {
            Thread thread = new Thread(() =>
            {
                TcpClient tcp = new TcpClient(ip, port);
                stream = tcp.GetStream();
                try
                {
                    byte[] bytes = new byte[tcp.ReceiveBufferSize];
                    stream.Read(bytes, 0, bytes.Length);
                    data = new DATA(bytes);
                }
                catch (Exception) { }
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