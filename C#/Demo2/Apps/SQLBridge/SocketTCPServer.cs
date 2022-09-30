using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace SQLBridgeApp
{
    internal class SocketTCPServer
    {
        public event Action<Socket, string> CreateConnectionRequest;
        public event Action<Socket, string> TestConnectionRequest;
        public event Action<Socket, string> QueryExecuteRequest;


        IPEndPoint localEndPoint;
        Socket listener;


        public SocketTCPServer(IPAddress ipAddress, int portNumber)
        {
            localEndPoint = new IPEndPoint(ipAddress, portNumber);
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            while (true)
            {
                Socket client = listener.Accept();
                Task.Run(() => ClientMethod(client));
            }
        }

        private void ClientMethod(Socket client)
        {
            Debug.WriteLine(client.RemoteEndPoint.ToString() + " connected");

            client.ReceiveTimeout = -1;
            byte[] receiveBuffer = new byte[1024];
            int receivedBytes = 0;
            string receivedData;

            while (true)
            {
                try
                {
                    receivedBytes = client.Receive(receiveBuffer);

                    if (receivedBytes == 0)
                        break;

                    receivedData = Encoding.ASCII.GetString(receiveBuffer, 0, receivedBytes);
                    receivedData = receivedData.Replace("\r\n", string.Empty);

                    Debug.WriteLine(client.RemoteEndPoint.ToString() + " received: " + receivedData);

                    if (receivedData.Contains(Constants.CreateConnection))
                        CreateConnectionRequest?.Invoke(client, receivedData.Replace(Constants.CreateConnection + Constants.MessageSeparator, string.Empty));

                    if (receivedData.Contains(Constants.TestConnection))
                        TestConnectionRequest?.Invoke(client, receivedData.Replace(Constants.TestConnection + Constants.MessageSeparator, string.Empty));

                    if (receivedData.Contains(Constants.QueryExecute))
                        QueryExecuteRequest?.Invoke(client, receivedData.Replace(Constants.QueryExecute + Constants.MessageSeparator, string.Empty));
                }

                catch(Exception ex)
                {
                    SocketException socketException = ex as SocketException;

                    if(!(socketException != null && (socketException.SocketErrorCode == SocketError.TimedOut || socketException.SocketErrorCode == SocketError.ConnectionReset)))
                        MessageBox.Show(ex.Message, Constants.Error + ':', MessageBoxButton.OK, MessageBoxImage.Error);

                    break;
                }
            }

            Debug.WriteLine(client.RemoteEndPoint.ToString() + " disconnected");
            client.Close();
        }

        public void SendMessage(Socket client, string message)
        {
            byte[] txData = Encoding.ASCII.GetBytes(message);
            client.Send(txData);
            Debug.WriteLine(client.RemoteEndPoint.ToString() + " transmitted: " + message);
        }
    }
}