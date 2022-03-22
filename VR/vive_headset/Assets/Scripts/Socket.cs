using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;
using UnityEngine.UI;

public class Socket : MonoBehaviour
{
    TcpClient commandSocket;
    NetworkStream commandStream;
    StreamWriter commandWriter;
    string host = "localhost";
    Int32 port = 8080;
    public Text indicator;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            commandSocket = new TcpClient(host, port);
            commandStream = commandSocket.GetStream();
            commandWriter = new StreamWriter(commandStream);
            indicator.text = "correct";
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
            indicator.text = "wrong";
        }
    }

    // Update is called once per frame
    void Update()
    {
        commandWriter.Write("test");
        commandWriter.Flush();
    }
}
