using IOBoardServer.IOBoard;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IOBoardServer
{
    class Program
    {
        static readonly string cfgFileName = "config.cfg";
        static Settings settings;
        static ILog log = new ConsoleLog();
        static IOBoardScanner ioBoardScanner;
        static TCPServer tcpServer;
        static int operationPerSec = 0;


        static void Main(string[] args)
        {
            log.NewMessage(LogLevels.Information, nameof(Program) + " -> IOBoard server V1.0 started...");

            if(IsProcessRunning())
            {
                log.NewMessage(LogLevels.Error, nameof(Program) + " -> An instance of the program is already running");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            try
            {
                settings = new Settings(cfgFileName);
            }
            catch
            {
                log.NewMessage(LogLevels.Error, nameof(Program) + " -> Cant't load configuration file: " + cfgFileName);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            ioBoardScanner = new IOBoardScanner(log);
            ioBoardScanner.Scan();

            if (ioBoardScanner.Devices.Count() != 0)
                log.NewMessage(LogLevels.Information, nameof(IIOBoardDevice) + " -> Available device(s): " + ioBoardScanner.Devices.Count());
            else
            {
                log.NewMessage(LogLevels.Error, nameof(IIOBoardDevice) + " -> Device not found");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            foreach (IIOBoardDevice ioBoardDevice in ioBoardScanner.Devices)
                ioBoardDevice.Connect();

            Task.Run(() => OperationCounterMethod());

            tcpServer = new TCPServer(settings.ServerIPAddress, settings.ServerPort, log);
            tcpServer.DataReceived += TcpServer_DataReceived;

            try
            {
                tcpServer.Start();
            }
            catch (Exception ex)
            {
                log.NewMessage(LogLevels.Error, nameof(Program) + " -> " + ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
        }

        private static bool IsProcessRunning() => Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length == 1 ? false : true;

        private static void TcpServer_DataReceived(Socket client, string clientName, string data)
        {
            if(data == "gdl")
            {
                StringBuilder stringBuilder = new StringBuilder();

                for(int i=0;i<ioBoardScanner.Devices.Count();i++)
                {
                    stringBuilder.Append(ioBoardScanner.Devices.ElementAt(i).SerialNo);

                    if (i != ioBoardScanner.Devices.Count() - 1)
                        stringBuilder.Append('/');
                }

                log.NewMessage(LogLevels.Information, nameof(Program) + " -> " + clientName + " read device list");
                tcpServer.Send(client, stringBuilder.ToString());
            }

            else
            {
                string[] temp = data.Split(new[] { '/' }, StringSplitOptions.None);

                if (temp.Length != 2)
                {
                    log.NewMessage(LogLevels.Error, nameof(Program) + " -> " + clientName + " arguments != 2");
                    tcpServer.Send(client, "ep"); // parameter error
                    return;
                }

                IIOBoardDevice device = ioBoardScanner.Devices.FirstOrDefault(d => d.SerialNo == temp[0]);

                if (device == null)
                {
                    log.NewMessage(LogLevels.Error, nameof(Program) + " -> " + clientName + " requested device not exist: " + temp[0]);
                    tcpServer.Send(client, "ene"); // not exist error
                    return;
                }

                string recvData = device.SendAndReceive(temp[1]);

                if (recvData == null)
                {
                    log.NewMessage(LogLevels.Error, nameof(Program) + " -> " + clientName + " requested device not connected: " + temp[0]);
                    tcpServer.Send(client, "enc"); // not connected error
                    return;
                }

                else if (recvData == string.Empty)
                {
                    log.NewMessage(LogLevels.Error, nameof(Program) + " -> " + clientName + " requested device communication error: " + temp[0]);
                    tcpServer.Send(client, "ece"); // not connected error
                    return;
                }

                else
                {
                    tcpServer.Send(client, recvData);
                    operationPerSec += 1;
                }
            }
        }

        private static void OperationCounterMethod()
        {
            Thread.Sleep(1000);

            while(true)
            {
                log.NewMessage(LogLevels.Information, nameof(Program) + " -> Communication: " + operationPerSec + " operation/sec    ");
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                operationPerSec = 0;
                Thread.Sleep(1000);
            }
        }
    }
}
