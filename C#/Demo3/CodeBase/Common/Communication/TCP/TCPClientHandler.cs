using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;


namespace Common.Communication.TCP5
{
    public class TCPClientHandler
    {
        public string IP { get; }
        public int Port { get; }


        readonly Socket clientSocket;
        readonly int packetBufferSize;
        readonly byte[] packetBuffer, dataBuffer, dataEnd;
        readonly string arraySeparator;


        public TCPClientHandler(Socket clientSocket, int packetBufferSize, int dataBufferSize, int receiveTimeout, byte[] dataEnd, string arraySeparator)
        {
            this.clientSocket = clientSocket;
            this.packetBufferSize = packetBufferSize;
            this.dataEnd = dataEnd;
            this.arraySeparator = arraySeparator;

            clientSocket.SendBufferSize = packetBufferSize;
            clientSocket.ReceiveBufferSize = packetBufferSize;
            clientSocket.ReceiveTimeout = receiveTimeout;
            packetBuffer = new byte[packetBufferSize];
            dataBuffer = new byte[dataBufferSize];

            IPEndPoint ipEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;

            if (ipEndPoint.IsNotNull())
            {
                IP = ipEndPoint.Address.ToString();
                Port = ipEndPoint.Port;
            }
        }

        internal void Close()
        {
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                clientSocket.Close();
            }
        }

        public string[] ReceiveTextArray()
        {
            string text = ReceiveText();

            if (string.IsNullOrEmpty(text))
                return new string[0];

            return text.Split(new string[] { arraySeparator }, StringSplitOptions.None);
        }

        public string ReceiveText()
        {
            byte[] data = ReceiveData();

            if (data.Length == 0)
                return string.Empty;

            return data.ToUTF8String();
        }

        public byte[] ReceiveData()
        {
            int receivedLength;
            int writeIndex = 0;

            while (true)
            {
                try
                {
                    receivedLength = clientSocket.Receive(packetBuffer);

                    if (receivedLength == 0)
                        break;

                    Array.Copy(packetBuffer, 0, dataBuffer, writeIndex, receivedLength);
                    writeIndex += receivedLength;

                    if (dataBuffer.EndWith(writeIndex, dataEnd))
                        return dataBuffer.Take(writeIndex - dataEnd.Length).ToArray();
                }
                catch
                {
                    break;
                }
            }

            return new byte[0];
        }

        public void SendTextArray(params string[] texts)
            => SendText(Utils.ConcatWithSeparator(arraySeparator, texts));

        public void SendText(string text)
            => SendData(text.ToUTF8Bytes());

        public void SendData(byte[] data)
        {
            int dataLeft = data.Length;
            int currentSendIndex = 0;
            bool errorOccurred = false;

            while (dataLeft > 0)
            {
                int dataToSend = dataLeft;

                if (dataToSend > packetBufferSize)
                    dataToSend = packetBufferSize;

                try
                {
                    clientSocket.Send(data.Skip(currentSendIndex).Take(dataToSend).ToArray());
                }
                catch
                {
                    errorOccurred = true;
                    break;
                }

                currentSendIndex += dataToSend;
                dataLeft -= dataToSend;
            }

            if (!errorOccurred)
            {
                try
                {
                    clientSocket.Send(dataEnd);
                }
                catch { }
            }
        }
    }
}