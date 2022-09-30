using System;
using System.Net;
using System.Net.Sockets;


namespace MultiCamApp.NetworkCommunication
{
    public class TCPClient
    {
        IPEndPoint ipEndPoint;
        Socket socket;
        byte[] receiverBuffer = new byte[10485760];
        byte[] packetBuffer = new byte[32768];


        public TCPClient(IPEndPoint ipEndPoint)
        {
            this.ipEndPoint = ipEndPoint;
            socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            socket.Connect(ipEndPoint);
        }

        internal void Send(byte[] data)
        {
            socket.Send(data);
        }

        internal void WaitForEOM()
        {
            
        }
    }
}
