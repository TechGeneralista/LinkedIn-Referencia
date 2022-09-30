using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace IOBoardServer.IOBoard.VCP
{
    internal class VCPIOBoardDevice : IIOBoardDevice
    {
        public IOBoardStates Status
        {
            get => status;
            private set
            {
                status = value;

                switch (status)
                {
                    case IOBoardStates.Connected:
                        log.NewMessage(LogLevels.Information, nameof(VCPIOBoardDevice) + " (" + SerialNo + ") -> " + nameof(IOBoardStates.Connected));
                        break;

                    case IOBoardStates.Connecting:
                        log.NewMessage(LogLevels.Information, nameof(VCPIOBoardDevice) + " (" + SerialNo + ") -> " + nameof(IOBoardStates.Connecting));
                        break;

                    case IOBoardStates.Disconnected:
                        log.NewMessage(LogLevels.Information, nameof(VCPIOBoardDevice) + " (" + SerialNo + ") -> " + nameof(IOBoardStates.Disconnected));
                        break;
                }
            }
        }

        public string SerialNo { get; private set; }


        IOBoardStates status;
        readonly ILog log;
        readonly SerialPort serialPort;
        Task autoReconnectTask;
        CancellationTokenSource cancellationTokenSource;
        object lockObject = new object();


        public VCPIOBoardDevice(SerialPort serialPort, string serialNo, ILog log)
        {
            this.serialPort = serialPort;
            SerialNo = serialNo;
            this.log = log;
        }

        public void Connect()
        {
            if (Status != IOBoardStates.Disconnected)
                return;

            cancellationTokenSource = new CancellationTokenSource();
            autoReconnectTask = Task.Run(() => AutoReconnectMethod(), cancellationTokenSource.Token);

            while (status != IOBoardStates.Connected)
                Thread.Sleep(100);
        }

        private void AutoReconnectMethod()
        {
            Status = IOBoardStates.Connecting;

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                switch (Status)
                {
                    case IOBoardStates.Connecting:
                        try
                        {
                            OpenComPort();
                            string receivedData = SendAndReceive("gsn");

                            if (receivedData == SerialNo)
                                Status = IOBoardStates.Connected;
                            else
                                CloseComPort();
                        }
                        catch { }
                        break;

                    case IOBoardStates.Connected:
                        if (!serialPort.IsOpen)
                        {
                            log.NewMessage(LogLevels.Error, nameof(VCPIOBoardDevice) + " (" + SerialNo + ") -> Connection lost");
                            Status = IOBoardStates.Connecting;
                        }
                        break;
                }

                Thread.Sleep(100);
            }

            Status = IOBoardStates.Disconnected;
        }

        private void CloseComPort()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }

        private void OpenComPort()
        {
            if (!serialPort.IsOpen)
                serialPort.Open();
        }

        public string SendAndReceive(string txData)
        {
            lock (lockObject)
            {
                try
                {
                    serialPort.Write(txData);
                    StringBuilder stringBuilder = new StringBuilder();
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    while (true)
                    {
                        stringBuilder.Append(serialPort.ReadExisting());

                        if (stringBuilder.Length != 0)
                        {
                            if (stringBuilder.ToString().Last() == 0)
                                break;
                        }

                        if (stopwatch.ElapsedMilliseconds > 10)
                            return null;
                    }

                    return stringBuilder.Replace("\0", string.Empty).ToString();
                }
                catch
                {
                    Status = IOBoardStates.Connecting;
                }
            }

            return null;
        }

        public void Disconnect()
        {
            if (Status != IOBoardStates.Connecting || Status != IOBoardStates.Connected)
                return;

            cancellationTokenSource.Cancel();

            while (status != IOBoardStates.Disconnected)
                Thread.Sleep(100);
        }
    }
}