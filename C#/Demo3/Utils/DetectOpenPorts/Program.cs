using System;
using System.Net.Sockets;
using System.Threading;


namespace DetectOpenPorts
{
    class Program
    {
        static string hostName = "portquiz.net";

        static void Main(string[] args)
        {
            Console.WriteLine("Started");

            for (int i = 1; i < 65535; i++)
            {
                TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork);

                if (tcpClient.ConnectAsync(hostName, i).Wait(50))
                {
                    Console.WriteLine(string.Format("port {0}: Ok", i));
                    tcpClient.Close();
                }

                tcpClient.Dispose();
            }

            Console.WriteLine("Finished");

            while(true)
                Thread.Sleep(1000);
        }
    }
}
