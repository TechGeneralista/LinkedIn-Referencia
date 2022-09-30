using Common;
using Common.Interfaces;
using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using Common.Settings;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Common.Communication.SimpleTCP.Server.MultiClient
{
    public class TCPServerMultiClientDC
    {
        public LanguageDC LanguageDC { get; }
        public IReadOnlyPropertyArray<string> AvailableIPs { get; } = new PropertyArray<string>();
        public IProperty<string> SelectedIP { get; }
        public IProperty<int> Port { get; }
        public IReadOnlyPropertyArray<Socket> Clients { get; } = new PropertyArray<Socket>();


        readonly LogDC logDC;
        readonly ICanCommunication canCommunication;
        IPEndPoint localEndPoint;
        Socket listener;


        public TCPServerMultiClientDC(LanguageDC languageDC, ISettingsCollection settingsCollection, LogDC logDC, ICanCommunication canCommunication)
        {
            LanguageDC = languageDC;
            this.logDC = logDC;
            this.canCommunication = canCommunication;

            if (canCommunication.IsNull())
                throw new ArgumentNullException();

            string[] availableIps = Utils.GetLocalIPAddresses();

            AvailableIPs.ToSettable().AddRange(availableIps); 
            SelectedIP = new StorableProperty<string>(availableIps[0], settingsCollection, nameof(TCPServerMultiClientDC), nameof(SelectedIP));
            Port = new StorableProperty<int>(2000, settingsCollection, nameof(TCPServerMultiClientDC), nameof(Port));

            Start();
        }

        private void Start()
        {
            if (AvailableIPs.Value.Length == 0)
            {
                SelectedIP.Value = "";
                return;
            }

            try
            {
                Initialize();
                logDC.NewMessage(LogTypes.Successful, LanguageDC.TCPServerInitialize, string.Format("{0}:{1}",SelectedIP.Value, Port.Value));
                Task.Run(() => AcceptMethod());
            }
            catch { }
        }

        private void Stop()
        {
            foreach (Socket socket in Clients.Value)
            {
                socket.Disconnect(false);
                socket.Dispose();
            }

            Clients.ToSettable().Clear();
            listener.Dispose();
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        private void AcceptMethod()
        {
            try
            {
                while (true)
                {
                    Socket newClient = listener.Accept();
                    Clients.ToSettable().Add(newClient);
                    Task.Run(() => ClientMethod(newClient));
                }
            }
            catch { }
        }

        private void ClientMethod(Socket client)
        {
            byte[] rxBuffer = new byte[1024];
            int receivedLength = 0;

            try
            {
                while (true)
                {
                    receivedLength = client.Receive(rxBuffer);

                    if (receivedLength == 0)
                        break;

                    string receivedData = Encoding.ASCII.GetString(rxBuffer, 0, receivedLength).Replace("\0", string.Empty);
                    string responseData = canCommunication.Communication(receivedData);

                    if (!string.IsNullOrEmpty(responseData))
                        Send(client, responseData);
                }
            }
            catch { }

            client.Dispose();
            Clients.ToSettable().Remove(client);
        }

        private void Initialize()
        {
            IPAddress ipAddress = IPAddress.Parse(SelectedIP.Value);
            localEndPoint = new IPEndPoint(ipAddress, Port.Value);
            listener = new Socket(localEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(1);
        }

        public void Send(Socket client, string data) => client.Send(Encoding.ASCII.GetBytes(data));
    }
}