using MultiCamApp.DTOs;
using MultiCamApp.NetworkCommunication;
using System;
using System.Net;
using System.Threading;
using System.Windows.Media.Imaging;


namespace MultiCamApp.MultiCam
{
    public class MultiCamDevice : ObservableProperty
    {
        public string Name { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public string SerialNo { get; private set; }
        public string Version { get; private set; }
        public bool IsConnected { get; private set; }

        public BitmapSource Snapshoot
        {
            get => snapshoot;
            private set => SetField(value, ref snapshoot);
        }


        TCPClient tcpClient;
        const int tcpCommunicationPort = 24000;
        BitmapSource snapshoot;

        public MultiCamDevice(DeviceInfoDTO0 deviceInfoDTO)
        {
            Version = deviceInfoDTO.Version[0] + "." + deviceInfoDTO.Version[1];
            IPAddress = new IPAddress(deviceInfoDTO.IPAddress);
            tcpClient = new TCPClient(new IPEndPoint(IPAddress, tcpCommunicationPort));
            tcpClient.Connect();

            CommandDTO commandDTO = new CommandDTO();
            commandDTO.Command = "GetDeviceInfo1".StringToDTOMember(32);

            //zek következnek, a Jetsonon a TCP szerver következik
            tcpClient.Send(commandDTO.ToBytes());
            tcpClient.WaitForEOM();
        }

        /*public void GetSnapshoot()
        {
            tcpClient.Connect(ipEndPoint);

            CommandDTO commandDTO = new CommandDTO();
            commandDTO.Command = "GetImageResolution".StringToDTOMember();
            tcpClient.Send(commandDTO.ToBytes());
            Receive();

            tcpClient.Close();

            Console.WriteLine(System.Text.Encoding.ASCII.GetString(receiverBuffer).Replace("\0", string.Empty));
        }*/

        /*private void Receive()
        {
            Array.Clear(receiverBuffer, 0, receiverBuffer.Length);
            int offset = 0;

            while(true)
            {
                int receivedBytes = tcpClient.Receive(packetBuffer);

                if (packetBuffer[0] == '<' && packetBuffer[1] == 'E' && packetBuffer[2] == 'O' && packetBuffer[3] == 'T' && packetBuffer[4] == '>')
                    break;

                Array.Copy(packetBuffer, 0, receiverBuffer, offset, receivedBytes);
                offset += receivedBytes;
            }
        }*/
    }
}
