using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace Common.Communication.TCP5
{
    public enum TCPServerStatus { Stopped, Listening, ErrorOccurred }

    public class TCPServerDC
    {
        public event Action<TCPClientHandler> NewClientConnected;
        public event Action<string, int> Started;

        public TCPServerStatus Status { get; private set; } = TCPServerStatus.Stopped;
        public Exception Error { get; private set; }
        public string IP { get; }
        public int Port { get; }


        readonly int packetBufferSize, dataBufferSize, receiverTimeout;
        readonly byte[] dataEnd;
        readonly string arraySeparator;
        IPEndPoint localEndPoint;
        Socket serverSocket;


        public TCPServerDC(string ip, int port, int packetBufferSize, int dataBufferSize, int receiverTimeout, string dataEnd, string arraySeparator)
        {
            if (string.IsNullOrEmpty(ip))
                ip = Utils.GetLocalIPAddresses()[0];

            IP = ip;
            Port = port;
            this.packetBufferSize = packetBufferSize;
            this.dataBufferSize = dataBufferSize;
            this.receiverTimeout = receiverTimeout;
            this.dataEnd = dataEnd.ToUTF8Bytes();
            this.arraySeparator = arraySeparator;
        }

        public Task StartAsync()
            => Task.Run(() => Start());

        public void Start()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(IP);
                localEndPoint = new IPEndPoint(ipAddress, Port);
                serverSocket = new Socket(localEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(localEndPoint);
                serverSocket.Listen(1);
                Status = TCPServerStatus.Listening;
                Started?.Invoke(IP, Port);

                while (true)
                {
                    Socket clientSocket = serverSocket.Accept();

                    Task.Run(() => 
                    {
                        TCPClientHandler client = new TCPClientHandler(clientSocket, packetBufferSize, dataBufferSize, receiverTimeout, dataEnd, arraySeparator);
                        NewClientConnected?.Invoke(client);
                        client.Close();
                    });
                }
            }

            catch (Exception ex)
            {
                Status = TCPServerStatus.ErrorOccurred;
                Error = ex;
            }
        }
    }
}
