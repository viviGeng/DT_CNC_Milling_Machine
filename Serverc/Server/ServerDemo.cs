using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using LitJson;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Timers.Time;

public class ServerDemo
{
    Socket socket;
    System.Timers.Timer timer;
    private ConcurrentDictionary<string,Socket> ClientConnected;
    private ConcurrentDictionary<string,int> HeartBeat;
    private ConcurrentDictionary<string,string[]> transferInfo;
    private string ip;
    private int port;
    //private byte[] buffer;
    private byte[] buffer = new byte[1024];
    private FileStream fs= new FileStream("/home/grx/Serverc/Serverc/Server/data.txt",FileMode.Open,FileAccess.Write);

    //private Socket client;
    public ServerDemo(string _ip, int _port)
    {
        this.ip = _ip;
        this.port = _port;
    }
    ~ServerDemo()
    {
        fs.Flush();
        fs.Close();
    }

    public void StartConnetct()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(ip), port);
            Console.WriteLine("ip"+ip);
            Console.WriteLine("port"+port);
            socket.Bind(iep);
            socket.Listen(1000);
            Console.WriteLine("New Serve: is opened successfully!");
            Console.WriteLine("Listen to the connection!");
            ClientConnected = new ConcurrentDictionary<string,Socket>();
            HeartBeat = new ConcurrentDictionary<string,int>();
            transferInfo = new ConcurrentDictionary<string,string[]>();
            string[] tmp = new string[]{"VR","AR"};
            transferInfo.Add("Mach3",tmp);
            string[] tmp = new string[]{"Mach3"};
            transferInfo.Add("VR",tmp);
            timer = new System.Timers.Timer();
            timer.Interval = 2000;
            timer.Elapsed += delegate{
                foreach (KeyValuePair<string, int> kvp in HeartBeat){
                    if(kvp.Value > 3){
                        HeartBeat.Remove(kvp.Key);
                        ClientConnected.Remove(kvp.Key);
                    }
                    else{
                        kvp.Value = kvp.Value + 1;
                    }
                }
            };
            timer.Start();
            socket.BeginAccept(new AsyncCallback(AcceptCallback),socket);
        }
        catch (Exception e)
        {
            Console.WriteLine("Server open failed!");
            Console.WriteLine("Error: " + e.ToString());
        } 
    }
    public void AcceptCallback(IAsyncResult ar)
    {
        Socket socket = (Socket) ar.AsyncState;
        Socket c = socket.EndAccept(ar);
        Console.WriteLine("One Client connected");
        Receive(c);
        socket.BeginAccept(new AsyncCallback(AcceptCallback),socket);
    }
    public void Receive(Socket c)
    {
        try
        {
            c.BeginReceive(buffer, 0, 1024, 0, new AsyncCallback(BegRece), c);
        }
        catch (Exception e)
        {
            Console.WriteLine("Receive Error: " + e.ToString());
        }
    }

    private async void BegRece(IAsyncResult ar)
    {
	    if(ar == null)
	    {
		    Console.WriteLine("Receive Null");
	    }
        Socket so = (Socket)ar.AsyncState;
        Console.WriteLine("Receive info from client");
	    int length = so.EndReceive(ar);
        Console.WriteLine("Info Length: "+length.ToString());

        try
       {
            int usd = 0;
            //粘包问题
            while (true)
            {
                int jdLength = BitConverter.ToInt32(buffer, usd);
                Console.WriteLine("jdLength is "+ jdLength.ToString());
		        if(jdLength > length)
                {
                    break;
                }
                //byte[] jdByte = new byte[jdLength];
                usd += 4;
                if(jdLength != 0){
                    string content = Encoding.UTF8.GetString(buffer, usd, jdLength);
                    JsonData jd = JsonMapper.ToObject<JsonData>(content);
                    string sender = (string) jd["sender"];
                    string note = (string) jd["note"];                
                    switch (note)
                    {
                        case "connect":
                            try{
                                ClientConnected.TryAdd(sender,so);
                                HeartBeat.TryAdd(sender,0);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("Error is : "+e);
                            }
                        case "heartbeat":
                            HeartBeat[sender] = 0;
                        case "transmit":
                        	byte[] jsByte = Encoding.UTF8.GetBytes(jd.ToJson());
			                int head = jsByte.Length;
			                byte[] ss = BitConverter.GetBytes(head);
			                byte[] s1 = new byte[ss.Length+jsByte.Length];
			                Buffer.BlockCopy(ss,0,s1,0,4);
			                Buffer.BlockCopy(jsByte,0,s1,4,jsByte.Length);
			                fs.Write(s1,0,s1.Length);
                            for(int i = 0 ; i < transferInfo["sender"].size() ; i++){
                                string terminal = transferInfo["sender"][i];
                                if(ClientConnected.ContainsKey(terminal)){
                                    ClientConnected[terminal].Send(s1);
                                }
                            }
                    }
                    if (!ClientConnected.ContainsKey(sender))
                    {
                        Console.WriteLine(sender+ " send test message and connected.");
                        try{
                            ClientConnected.TryAdd(sender,so);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("Error is : "+e);
                        }
                    }
                    else
                    {
                        if (note == "quit"){
                            Socket st;
                            if(ClientConnected.TryRemove(sender,out st)){
                                Console.WriteLine("Remove "+sender);
                            }
                        }
                    }   
		        }	
                usd += jdLength;
                if (usd == length)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error!!! "+ e.ToString());
        }
	    so.BeginReceive(buffer, 0, 1024, 0, new AsyncCallback(BegRece), so);	
    }
}
