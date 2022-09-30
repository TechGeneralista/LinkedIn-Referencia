using SmartVisionClientApp.Common;
using SmartVisionClientApp.DTOs;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


namespace SmartVisionClientApp.Communication
{
    public class TCPClient
    {
        Socket tcpClient;
        byte[] rxBuffer = new byte[1024 * 1024 * 10]; //10MB
        readonly byte eot = 4;
        readonly object lockObject = new object();


    public TCPClient(IPAddress serverAddr, int serverPort)
        {
            tcpClient = new Socket(serverAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            tcpClient.Connect(new IPEndPoint(serverAddr, serverPort));
        }

        void Send<T>(T obj) where T : class
        {
            string jsonStr = JsonSerializer.Serialize(obj) + (char)4;
            byte[] dataToSend = Encoding.UTF8.GetBytes(jsonStr);
            tcpClient.Send(dataToSend);
        }

        T Receive<T>() where T : class
        {
            byte[] buffer = new byte[1024 * 1024];
            int receivedLength = 0;
            bool receiving = true;

            do
            {
                int currentLength = tcpClient.Receive(buffer);
                Array.Copy(buffer, 0, rxBuffer, receivedLength, currentLength);
                Array.Clear(buffer, 0, currentLength);
                receivedLength += currentLength;

                if (rxBuffer[receivedLength - 1] == eot)
                {
                    rxBuffer[Array.IndexOf(rxBuffer, eot)] = 0;
                    receivedLength -= 1;
                    receiving = false;
                }

            } while (receiving);

            string jsonStr = Encoding.UTF8.GetString(rxBuffer.Take(receivedLength).ToArray());
            Array.Clear(rxBuffer, 0, receivedLength);
            T obj = JsonSerializer.Deserialize<T>(jsonStr);
            IHasQueryInfo hasQueryInfo = (IHasQueryInfo)obj;

            if(hasQueryInfo.QueryInfo.QueryStatus.ToUpper() == "OK")
            return obj;

            Utils.ShowError("QueryStatus -> ERROR");
            return default;
        }

        internal T2 SendAndReceive<T1, T2>(T1 objToSend) 
            where T1 : class 
            where T2 : class
        {
            lock(lockObject)
            {
                Send(objToSend);
                return Receive<T2>();
            }
        }
    }
}
