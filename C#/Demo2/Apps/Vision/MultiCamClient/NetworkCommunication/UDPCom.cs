using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace MultiCamApp.NetworkCommunication
{
    public class UDPCom
    {
        public event Action<IPEndPoint, byte[]> DataReceived;

        public UDPCom(int receiverPort)
        {
            Task.Run(() => ReceiverMethod(receiverPort));
        }

        private void ReceiverMethod(int receiverPort)
        {
            UdpClient udpServer = new UdpClient(receiverPort);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                byte[] receivedBytes = udpServer.Receive(ref ipEndPoint);

                if(receivedBytes.IsEOMAtEnd())
                {
                    receivedBytes = receivedBytes.RemoveEOMFromEnd();

                    if (receivedBytes.Length > 0)
                        DataReceived?.Invoke(ipEndPoint, receivedBytes);
                }
            }
        }

        public void SendBroadcastData(byte[] data, int udpPort)
        {
            data = data.AddEOMToEnd();

            using (UdpClient udpClient = new UdpClient())
            {
                foreach (IPAddress ipAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        byte[] addressBytes = ipAddress.GetAddressBytes();
                        addressBytes[3] = 255;
                        udpClient.Send(data, data.Length, new IPEndPoint(new IPAddress(addressBytes), udpPort));
                    }
                }
            }
        }
    }
}
