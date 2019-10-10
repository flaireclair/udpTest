using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Text;

namespace Script.SocketServer
{
    public class ClientSocket : MonoBehaviour
    {
        public int port = 11188;
        private static NetworkStream stream = null;
        public static DATA data;
        public static TcpClient tcp;
        public string ipaddr;
        // Start is calle1d before the first frame update
        void Start()
        {
            tcp = new TcpClient(ipaddr, port);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public static void Socket()
        {
            Thread thread = new Thread(() =>
            {
            stream = tcp.GetStream();
            try
            {
                byte[] bytes = new byte[tcp.ReceiveBufferSize];
                stream.Read(bytes, 0, bytes.Length);
                Debug.Log(Encoding.UTF8.GetString(bytes));
                }
                catch (Exception) { }
                stream.Close();
            });
            thread.Start();
        }

        public void SendMessage(byte[] sendByte)
        {
            if (stream != null)
            {
                Thread sendthread = new Thread(() => { stream.Write(sendByte, 0, sendByte.Length); });
                sendthread.Start();
            }
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