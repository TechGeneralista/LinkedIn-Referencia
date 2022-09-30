using Common;
using Common.Communication.TCP5;
using Common.Log;
using Common.Tool;
using System;


namespace ServerApp
{
    class Program
    {
        static string serverHost;
        static int serverPort;
        static string sqlHost;
        static int sqlPort;
        static string sqlCatalog;
        static string sqlUser;
        static string sqlPassword;


        static void Main(string[] args)
        {
            try
            {
                ReadableConfigFile readableConfigFile = new ReadableConfigFile(Utils.GetPath("config.ini"), '=');
                readableConfigFile.ReadFromDisk();

                serverHost = readableConfigFile.GetValue("ServiceListenIP");
                serverPort = int.Parse(readableConfigFile.GetValue("ServiceListenPort"));
                sqlHost = readableConfigFile.GetValue("DatabaseHostName");
                sqlPort = int.Parse(readableConfigFile.GetValue("DatabasePort"));
                sqlCatalog = readableConfigFile.GetValue("DatabaseCatalog");
                sqlUser = readableConfigFile.GetValue("DatabaseUserName");
                sqlPassword = readableConfigFile.GetValue("DatabaseUserPassword");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba a config.ini fájl beolvasásakor: " + ex.Message);
                return;
            }


            TCPServerDC tcpServerDC = new TCPServerDC(serverHost, serverPort, Utils.ConvertMegaBytesToBytes(1), Utils.ConvertMegaBytesToBytes(10), Utils.ConvertSecondsToMilliSeconds(30), Constants.DataEnd, Constants.DivisionSign.ToString());
            tcpServerDC.NewClientConnected += TCPServerDC_NewClientConnected;
            tcpServerDC.Started += TcpServerDC_Started;
            tcpServerDC.Start();
        }

        private static void TcpServerDC_Started(string ip, int port)
            => Console.WriteLine(string.Format("Szerver elindult: {0}/{1}", ip, port));

        private static void TCPServerDC_NewClientConnected(TCPClientHandler tcpClientHandler)
        {
            ConsoleLog consoleLog = new ConsoleLog();
            consoleLog.NewMessage(string.Format("Kliens kapcsolódott: {0}/{1}", tcpClientHandler.IP, tcpClientHandler.Port));
            string[] receivedArray = tcpClientHandler.ReceiveTextArray();

            if (receivedArray.Length == 5)
            {
                Repository repository = new Repository(sqlHost, sqlPort, sqlCatalog, sqlUser, sqlPassword);
                repository.Connect();

                try
                {
                    ServiceDC applicationServiceDC = new ServiceDC(tcpClientHandler, repository, consoleLog, receivedArray);
                    consoleLog.NewMessage(string.Format("Kliens azonosítás sikeres: {0} V{1}.{2}.{3} - {4}", applicationServiceDC.Name, applicationServiceDC.Major, applicationServiceDC.Minor, applicationServiceDC.Build, applicationServiceDC.PersonalComputerId));
                    tcpClientHandler.SendText(Constants.Ok);
                    applicationServiceDC.Start();
                }
                catch
                {
                    consoleLog.NewMessage(string.Format("Kliens azonosítás hiba: {0}/{1}", tcpClientHandler.IP, tcpClientHandler.Port));
                }

                repository.Disconnect();
            }
            else
            {
                consoleLog.NewMessage(string.Format("Kliens sikertelen azonosítás: {0}/{1}", tcpClientHandler.IP, tcpClientHandler.Port));
            }

            consoleLog.NewMessage(string.Format("Kliens levált: {0}/{1}", tcpClientHandler.IP, tcpClientHandler.Port));
        }
    }
}
