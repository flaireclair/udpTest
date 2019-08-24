using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;

public class ClientSocket : MonoBehaviour
{
  public string ipOrHost = "127.0.0.1";
  public int port = 11888;
    NetworkStream stream = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  private NetworkStream GetNetworkStream()
  {
    if (stream != null && stream.CanRead)
    {
      return stream;
    }

    TcpClient tcp = new TcpClient(ipOrHost, port);
    Debug.Log("success conn server");

    return tcp.GetStream();
  }
}
