using Common;
using Common.NotifyProperty;
using Common.Settings;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Communication.TCP.Server.SingleClient
{
    public class TCPServerSingleClientDC
    {
        public event Action<Socket, string> DataReceived;


        public ISettableObservableProperty<string> IPAddressString { get; } = new ObservableProperty<string>(Utils.GetLocalIPAddress());
        public ISettableObservableProperty<int> Port { get; } = new ObservableProperty<int>(2000);
        public INonSettableObservableProperty<bool?> ListenStatus { get; } = new ObservableProperty<bool?>();
        public INonSettableObservableProperty<bool?> ClientStatus { get; } = new ObservableProperty<bool?>();


        FileSettingsStore fileSettingsStore = new FileSettingsStore(Utils.GetPath("TCPServerSingleClientDC.bin"));
        IPEndPoint localEndPoint;
        Socket listener;
        Socket clientSocket;


        public TCPServerSingleClientDC()
        {
            SettingsCollection settingsCollection = new SettingsCollection(fileSettingsStore);

            if(settingsCollection.Count != 0)
            {
                IPAddressString.CurrentValue = settingsCollection.GetValue<string>(nameof(IPAddressString));
                Port.CurrentValue = settingsCollection.GetValue<int>(nameof(Port));
            }

            Task.Run(() => ServerMethod());
        }

        ~TCPServerSingleClientDC()
        {
            Write();
        }

        public void Restart()
        {
            listener.Dispose();

            if(clientSocket.IsNotNull())
                clientSocket.Dispose();
        }

        private void ServerMethod()
        {
            while (true)
            {
                try
                {
                    IPAddress ipAddress = IPAddress.Parse(IPAddressString.CurrentValue);
                    localEndPoint = new IPEndPoint(ipAddress, Port.CurrentValue);
                    listener = new Socket(localEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    listener.Bind(localEndPoint);
                    listener.Listen(1);

                    while (true)
                    {
                        ListenStatus.ForceSet(true);
                        ClientStatus.ForceSet(false);

                        try
                        {
                            clientSocket = listener.Accept();
                        }
                        catch (Exception ex)
                        {
                            break;
                        }

                        ListenStatus.ForceSet(false);
                        ClientStatus.ForceSet(true);
                        byte[] rxBuffer = new byte[1024];

                        while (true)
                        {
                            int receivedLength = 0;

                            try
                            {
                                receivedLength = clientSocket.Receive(rxBuffer);
                            }
                            catch (Exception ex)
                            {
                                break;
                            }

                            if (receivedLength == 0)
                            {
                                break;
                            }

                            string receivedData = Encoding.ASCII.GetString(rxBuffer, 0, receivedLength).Replace("\0", string.Empty);
                            DataReceived?.Invoke(clientSocket, receivedData);
                        }

                        ClientStatus.ForceSet(false);

                        if (clientSocket.IsNotNull())
                            clientSocket.Dispose();
                    }
                }
                catch(Exception ex)
                {
                }

                ListenStatus.ForceSet(false);
                ClientStatus.ForceSet(false);
                Thread.Sleep(1000);
            }
        }

        public void Send(Socket client, string data) => client.Send(Encoding.ASCII.GetBytes(data));

        internal void Write()
        {
            SettingsCollection settingsCollection = new SettingsCollection(fileSettingsStore);
            settingsCollection.SetValue<string>(nameof(IPAddressString), IPAddressString.CurrentValue);
            settingsCollection.SetValue<int>(nameof(Port), Port.CurrentValue);
            settingsCollection.Write();
        }
    }
}