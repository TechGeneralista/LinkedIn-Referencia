using AppLog;
using Common;
using Common.NotifyProperty;
using Common.Settings;
using Language;
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


        public LanguageDC LanguageDC { get; }
        public ISettableObservableProperty<string> IPAddressString { get; }
        public ISettableObservableProperty<int> Port { get; }
        public INonSettableObservableProperty<bool?> ListenStatus { get; } = new ObservableProperty<bool?>();
        public INonSettableObservableProperty<bool?> ClientStatus { get; } = new ObservableProperty<bool?>();


        ILog log;
        IPEndPoint localEndPoint;
        Socket listener;
        Socket clientSocket;


        public TCPServerSingleClientDC(LanguageDC languageDC, ISettingsCollection settingsCollection, ILog log)
        {
            LanguageDC = languageDC;
            this.log = log;

            IPAddressString = new StorableObservableProperty<string>(Utils.GetLocalIPAddress(), settingsCollection, nameof(TCPServerSingleClientDC), nameof(IPAddressString));
            Port = new StorableObservableProperty<int>(2000, settingsCollection, nameof(TCPServerSingleClientDC), nameof(Port));

            Task.Run(() => ServerMethod());
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
                    log.NewMessage(LogTypes.Successful, string.Format("{0} -> {1}", LanguageDC.TCPServer.CurrentValue, LanguageDC.Initialization.CurrentValue));

                    while (true)
                    {
                        log.NewMessage(LogTypes.Information, string.Format("{0} -> {1}", LanguageDC.TCPServer.CurrentValue, LanguageDC.WaitingForClientToConnect.CurrentValue));
                        ListenStatus.ForceSet(true);
                        ClientStatus.ForceSet(false);

                        try
                        {
                            clientSocket = listener.Accept();
                        }
                        catch (Exception ex)
                        {
                            log.NewMessage(LogTypes.Error, string.Format("{0} -> {1}", LanguageDC.TCPServer.CurrentValue, ex.Message));
                            break;
                        }

                        log.NewMessage(LogTypes.Information, string.Format("{0} -> {1}", LanguageDC.TCPServer.CurrentValue, LanguageDC.ClientConnected.CurrentValue));
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
                                log.NewMessage(LogTypes.Error, string.Format("{0} -> {1}", LanguageDC.TCPServer.CurrentValue, ex.Message));
                                break;
                            }

                            if (receivedLength == 0)
                            {
                                log.NewMessage(LogTypes.Warning, string.Format("{0} -> {1}", LanguageDC.TCPServer.CurrentValue, LanguageDC.ClientDisconnected.CurrentValue));
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
                    log.NewMessage(LogTypes.Error, string.Format("{0} -> {1}", LanguageDC.TCPServer.CurrentValue, ex.Message));
                }

                ListenStatus.ForceSet(false);
                ClientStatus.ForceSet(false);
                Thread.Sleep(1000);
            }
        }

        public void Send(Socket client, string data) => client.Send(Encoding.ASCII.GetBytes(data));
    }
}