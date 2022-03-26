using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LitJson;

public class Client
{
    private static Socket client;

    public static Thread thread;
    public static ThreadStart ts;

    public static byte[] readBuffer;
    public static int maxbuffer = 10240;
    public static int alread = 0;
    public static int usedByte = 0;
    public static bool isRun = false;

    public static Queue<JsonData> command;
    //IPEndPoint iep;
    public Client()
    {

    }
    public static void StartThread()
    {
        try
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("10.11.13.30"), 30000);
            client.Connect(iep);
        }
        catch (Exception e)
        {
        }
        Client.command = new Queue<JsonData>();
        Client.isRun = true;
        Client.readBuffer = new byte[Client.maxbuffer];
        Client.ts = new ThreadStart(run);
        thread = new Thread(ts);
        thread.IsBackground = true;
        thread.Start();
    }

    public static void run()
    {
        if (Client.isRun == false)
        {
            return;
        }

        try
        {
            client.BeginReceive(Client.readBuffer, 0, Client.maxbuffer, 0, new AsyncCallback(Receive), client);
        }
        catch (Exception e)
        {
        }
        while (Client.isRun)
        {

        }
    }

    private static void Receive(IAsyncResult ar)
    {
        Socket socket = (Socket)ar.AsyncState;
        int length = 0;
        try
        {
            length = socket.EndReceive(ar);
        }
        catch (Exception e)
        {
        }
        try
        {
            dispose();
        }
        catch (Exception e)
        {
            //Debug.Log("Exception: " + e.ToString());
        }
        Client.client.BeginReceive(Client.readBuffer, Client.alread, Client.maxbuffer, 0, new AsyncCallback(Receive), Client.client);
    }

    private static void dispose()
    {
        while (true)
        {
            byte[] head = new byte[4];
            Buffer.BlockCopy(Client.readBuffer, Client.usedByte, head, 0, 4);
            int length = BitConverter.ToInt32(head, 0);
            if (Client.alread >= Client.usedByte + 4 + length)
            {
                byte[] tmp = new byte[length];
                Buffer.BlockCopy(Client.readBuffer, Client.usedByte + 4, tmp, 0, length);
                string content = Encoding.UTF8.GetString(tmp);
                JsonData jd = JsonMapper.ToObject<JsonData>(content);
                if (jd != null)
                {
                    try
                    {
                        DisposePacket(jd);
                    }
                    catch (Exception e)
                    {
                        //Debug.Log("DisposePacket Error" + e.ToString());
                    }

                }
                Client.usedByte = Client.usedByte + 4 + length;
                if (Client.alread >= Client.usedByte + 4)
                {
                    continue;
                }
                else
                {
                    if (Client.alread <= Client.usedByte)
                    {
                        Client.readBuffer = new byte[Client.maxbuffer];
                    }
                    else
                    {
                        byte[] t = new byte[Client.alread - Client.usedByte];
                        Buffer.BlockCopy(Client.readBuffer, Client.usedByte, t, 0, Client.alread - Client.usedByte);
                        Client.readBuffer = new byte[Client.maxbuffer];
                        Buffer.BlockCopy(t, 0, Client.readBuffer, 0, Client.alread - Client.usedByte);
                    }
                    Client.alread = Client.alread - Client.usedByte;
                    Client.usedByte = 0;
                    break;
                }
            }
        }
    }

    private static void DisposePacket(JsonData jd)
    {
        command.Enqueue(jd);
        string sender = (string)jd["sender"];
    }

    public static void SendMessage(JsonData _jd)
    {
        byte[] content = Encoding.UTF8.GetBytes(_jd.ToJson());
        int length = content.Length;
        byte[] head = new byte[4];
        head = BitConverter.GetBytes(length);
        byte[] ss = new byte[length + 4];
        Buffer.BlockCopy(head, 0, ss, 0, 4);
        Buffer.BlockCopy(content, 0, ss, 4, length);
        try
        {
            Client.client.Send(ss);
        }
        catch (Exception e)
        {
        }
    }

    public static void Close()
    {
        Client.isRun = false;
        Client.thread.Abort();
        Client.client.Close();
    }
}

