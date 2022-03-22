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

public class ServerDemo
{
    Socket socket;
    //private List<Socket> client;
    private ConcurrentDictionary<string,Socket> ClientConnected;
    private bool StartCommunication = false;
    private string ip;
    private int port;
    private int maxBuffer = 1024;
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
        //byte[] buffer = new byte[maxBuffer];
        try
        {
            c.BeginReceive(buffer, 0, 1024, 0, new AsyncCallback(BegRece), c);
        }
        catch (Exception e)
        {
            Console.WriteLine("Receive Error: " + e.ToString());
        }
    }

    private void BegRece(IAsyncResult ar)
    {
	if(ar == null)
	{
		Console.WriteLine("Null!!!!!");
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
		        int k =0;
		        foreach (KeyValuePair<string, Socket> kvp in ClientConnected)
                        {
			    k = k+1;
                            Console.WriteLine("Key = {0}", kvp.Key);
                        }
		        Console.WriteLine("Dictionary Length: "+ k);	
			byte[] jsByte = Encoding.UTF8.GetBytes(jd.ToJson());
			int head = jsByte.Length;
			byte[] ss = BitConverter.GetBytes(head);
			byte[] s1 = new byte[ss.Length+jsByte.Length];
			Buffer.BlockCopy(ss,0,s1,0,4);
			Buffer.BlockCopy(jsByte,0,s1,4,jsByte.Length);
			fs.Write(s1,0,s1.Length);
            ClientConnected[sender].Send(s1);
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
