using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serverc
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = "";
            int port = 0000;
            // ServerDemo sd = new ServerDemo("0.0.0.0",30000);
            ServerDemo sd = new ServerDemo(ip,port);
            sd.StartConnetct();
            Console.ReadKey();
        }
    }
}
