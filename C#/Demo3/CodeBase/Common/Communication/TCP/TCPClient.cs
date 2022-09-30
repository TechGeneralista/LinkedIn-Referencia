using Common.NotifyProperty;
using System;
using System.Linq;
using System.Net.Sockets;


namespace Common.Communication.TCP5
{
    public enum TCPClientStatus { Disconnected, Connected, ErrorOccurred }

    public class TCPClient
    {
        public IReadOnlyProperty<TCPClientStatus> Status { get; } = new Property<TCPClientStatus>();
        public Exception Error { get; private set; }
        public string IP { get; }
        public int Port { get; }


        readonly string host, arraySeparator;
        readonly int port, packetBufferSize, receiveTimeout;
        Socket clientSocket;
        readonly byte[] packetBuffer, dataBuffer, dataEnd;


        public TCPClient(string host, int port, int packetBufferSize, int dataBufferSize, int receiveTimeout, string dataEnd, string arraySeparator)
        {
            this.packetBufferSize = packetBufferSize;
            this.host = host;
            this.port = port;
            this.receiveTimeout = receiveTimeout;
            this.dataEnd = dataEnd.ToUTF8Bytes();
            this.arraySeparator = arraySeparator;

            packetBuffer = new byte[packetBufferSize];
            dataBuffer = new byte[dataBufferSize];
        }

        public void Connect()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.SendBufferSize = packetBufferSize;
                clientSocket.ReceiveBufferSize = packetBufferSize;
                clientSocket.ReceiveTimeout = receiveTimeout;
                clientSocket.Connect(host, port);
                Error = null;
                Status.ToSettable().Value = TCPClientStatus.Connected;
            }
            catch (Exception ex)
            {
                Error = ex;
                Status.ToSettable().Value = TCPClientStatus.ErrorOccurred;
                clientSocket.Close();
            }
        }

        public void Disconnect()
        {
            try
            {
                clientSocket.Close();
            }
            catch(Exception ex)
            {
                Error = ex;
                Status.ToSettable().Value = TCPClientStatus.ErrorOccurred;
            }
        }

        public void SendTextArray(params string[] texts)
            => SendText(Utils.ConcatWithSeparator(Constants.DivisionSign, texts));

        public void SendText(string text)
            => SendData(text.ToUTF8Bytes());

        public void SendData(byte[] data)
        {
            int dataLeft = data.Length;
            int currentSendIndex = 0;

            while (dataLeft > 0)
            {
                int dataToSend = dataLeft;

                if (dataToSend > packetBufferSize)
                    dataToSend = packetBufferSize;

                try
                {
                    clientSocket.Send(data.Skip(currentSendIndex).Take(dataToSend).ToArray());
                }
                catch (Exception ex)
                {
                    Error = ex;
                    Status.ToSettable().Value = TCPClientStatus.ErrorOccurred;
                    break;
                }

                currentSendIndex += dataToSend;
                dataLeft -= dataToSend;
            }

            if (Error.IsNull())
            {
                try
                {
                    clientSocket.Send(dataEnd);
                }
                catch (Exception ex)
                {
                    Error = ex;
                    Status.ToSettable().Value = TCPClientStatus.ErrorOccurred;
                }
            }
        }

        public string[] ReceiveTextArray()
        {
            string text = ReceiveText();

            if (string.IsNullOrEmpty(text))
                return new string[0];

            return text.Split(Constants.DivisionSign);
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
                catch (Exception ex)
                {
                    Error = ex;
                    Status.ToSettable().Value = TCPClientStatus.ErrorOccurred;
                    break;
                }
            }

            return new byte[0];
        }
    }
}
