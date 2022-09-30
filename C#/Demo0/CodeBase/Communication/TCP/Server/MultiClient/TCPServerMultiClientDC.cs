using AppLog;
using Common;
using Common.NotifyProperty;
using Common.Settings;
using Language;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Communication.TCP.Server.MultiClient
{
    public class TCPServerMultiClientDC
    {
        public event Action<Socket, string> DataReceived;


        public LanguageDC LanguageDC { get; }
        public INonSettableObservablePropertyArray<string> AvailableIPs { get; }
        public ISettableObservableProperty<string> SelectedIP { get; }
        public ISettableObservableProperty<int> Port { get; }
        public INonSettableObservablePropertyArray<Socket> Clients { get; } = new ObservablePropertyArray<Socket>();


        ILog log;
        IPEndPoint localEndPoint;
        Socket listener;


        public TCPServerMultiClientDC(LanguageDC languageDC, ISettingsCollection settingsCollection, ILog log)
        {
            LanguageDC = languageDC;
            this.log = log;

            string[] availableIps = Utils.GetLocalIPAddresses();

            AvailableIPs = new ObservablePropertyArray<string>(availableIps);
            SelectedIP = new StorableObservableProperty<string>(availableIps[0], settingsCollection, nameof(TCPServerMultiClientDC), nameof(SelectedIP));
            Port = new StorableObservableProperty<int>(2000, settingsCollection, nameof(TCPServerMultiClientDC), nameof(Port));

            Start();
        }

        private void Start()
        {
            if (AvailableIPs.CurrentValue.Length == 0)
            {
                SelectedIP.CurrentValue = "";
                return;
            }

            try
            {
                Initialize();
                log.NewMessage(LogTypes.Successful, string.Format("{0} -> {1}", LanguageDC.TCPServer.CurrentValue, LanguageDC.Initialization.CurrentValue));
                Task.Run(() => AcceptMethod());
            }
            catch { }
        }

        private void Stop()
        {
            foreach (Socket socket in Clients.CurrentValue)
            {
                socket.Disconnect(false);
                socket.Dispose();
            }

            Clients.ForceClear();
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
                    Clients.ForceAdd(newClient);
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
                    DataReceived?.Invoke(client, receivedData);
                }
            }
            catch { }

            client.Dispose();
            Clients.ForceRemove(client);
        }

        private void Initialize()
        {
            IPAddress ipAddress = IPAddress.Parse(SelectedIP.CurrentValue);
            localEndPoint = new IPEndPoint(ipAddress, Port.CurrentValue);
            listener = new Socket(localEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(1);
        }

        public void Send(Socket client, string data) => client.Send(Encoding.ASCII.GetBytes(data));
    }
}