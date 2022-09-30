using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace IOBoardServer
{
    enum ReceiveProcess { Name, Data }

    public class TCPServer
    {
        public event Action<Socket, string, string> DataReceived;


        IPAddress serverIPAddress;
        int serverPort;
        IPEndPoint localEndPoint;
        ILog log;


        public TCPServer(IPAddress serverIPAddress, int serverPort, ILog log)
        {
            this.serverIPAddress = serverIPAddress;
            this.serverPort = serverPort;
            this.log = log;

            localEndPoint = new IPEndPoint(serverIPAddress, serverPort);
        }

        internal void Start()
        {
            Socket listener = new Socket(serverIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(1);
            log.NewMessage(LogLevels.Information, nameof(TCPServer) + " -> Started: " + localEndPoint.ToString());

            while (true)
            {
                Socket clientSocket = listener.Accept();
                Task.Run(() => ClientMethod(clientSocket));
            }
        }

        private void ClientMethod(Socket clientSocket)
        {
            ReceiveProcess receiveProcess = ReceiveProcess.Name;
            byte[] rxBuffer = new byte[1024];
            clientSocket.ReceiveTimeout = 1 * (1000*60); // 1 min
            //clientSocket.ReceiveTimeout = 10 * 1000; // 10 sec
            log.NewMessage(LogLevels.Information, nameof(TCPServer) + " -> Client connected: " + clientSocket.RemoteEndPoint.ToString());
            int receivedLength = 0;
            string clientName = "";

            try
            {
                while (true)
                {
                    try
                    {
                        receivedLength = clientSocket.Receive(rxBuffer);

                        if (receivedLength == 0)
                            break;
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(SocketException))
                        {
                            SocketException socketEx = (SocketException)ex;

                            if (socketEx.SocketErrorCode == SocketError.TimedOut)
                                ex = new Exception("Timeout");
                        }

                        break;
                    }

                    string receivedData = Encoding.UTF8.GetString(rxBuffer, 0, receivedLength).Replace("\0", string.Empty);

                    if (receiveProcess == ReceiveProcess.Name)
                    {
                        if (!string.IsNullOrEmpty(receivedData) && !string.IsNullOrWhiteSpace(receivedData))
                        {
                            receiveProcess = ReceiveProcess.Data;
                            clientName = receivedData;
                            SendOk(clientSocket);
                            log.NewMessage(LogLevels.Information, nameof(TCPServer) + " -> " + clientSocket.RemoteEndPoint.ToString() + " name of client received: " + clientName);
                        }
                        else
                            throw new Exception("Client name is empty or white space");
                    }

                    else if (receiveProcess == ReceiveProcess.Data)
                    {
                        if (receivedData == ".") //keep alive 1s interval
                            SendOk(clientSocket);
                        else
                            DataReceived?.Invoke(clientSocket, clientName, receivedData);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(clientName) && !string.IsNullOrWhiteSpace(clientName))
                    log.NewMessage(LogLevels.Error, nameof(TCPServer) + " -> " + clientName + ": " + ex.Message);
                else
                    log.NewMessage(LogLevels.Error, nameof(TCPServer) + " -> " + clientSocket.RemoteEndPoint.ToString() + ": " + ex.Message);
            }

            clientSocket.Disconnect(true);

            if (!string.IsNullOrEmpty(clientName) && !string.IsNullOrWhiteSpace(clientName))
                log.NewMessage(LogLevels.Information, nameof(TCPServer) + " -> Client disconnected: " + clientName);
            else
                log.NewMessage(LogLevels.Information, nameof(TCPServer) + " -> Client disconnected: " + clientSocket.RemoteEndPoint.ToString());
        }

        public void Send(Socket client, string data) => client.Send(Encoding.UTF8.GetBytes(data));

        private static void SendOk(Socket clientSocket) => clientSocket.Send(Encoding.UTF8.GetBytes("ok"));
    }
}