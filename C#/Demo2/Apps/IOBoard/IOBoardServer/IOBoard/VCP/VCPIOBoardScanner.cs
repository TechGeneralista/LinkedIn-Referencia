using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;


namespace IOBoardServer.IOBoard.VCP
{
    public class VCPIOBoardScanner : IIOBoardScanner
    {
        public static readonly int VCPDeviceResponseDelay = 1; //ms

        public IEnumerable<IIOBoardDevice> Devices { get; private set; }


        string acceptableDeviceType;
        ILog log;


        public VCPIOBoardScanner(string acceptableDeviceType, ILog log)
        {
            this.acceptableDeviceType = acceptableDeviceType;
            this.log = log;
        }

        
        public void Scan()
        {
            List<IIOBoardDevice> vcpDevices = new List<IIOBoardDevice>();
            string[] availablePorts = SerialPort.GetPortNames();

            foreach (string port in availablePorts)
            {
                SerialPort serialPort = new SerialPort(port);

                try
                {
                    serialPort.Open();

                    serialPort.Write("gdt");
                    Thread.Sleep(VCPDeviceResponseDelay);
                    string receivedDeviceType = serialPort.ReadExisting().Replace("\0", string.Empty);

                    if (string.IsNullOrEmpty(receivedDeviceType) || receivedDeviceType != acceptableDeviceType)
                    {
                        serialPort.Close();
                        continue;
                    }

                    serialPort.Write("gsn");
                    Thread.Sleep(VCPDeviceResponseDelay);
                    string receivedSerialNo = serialPort.ReadExisting().Replace("\0", string.Empty);

                    if (string.IsNullOrEmpty(receivedSerialNo))
                    {
                        serialPort.Close();
                        continue;
                    }

                    vcpDevices.Add(new VCPIOBoardDevice(serialPort, receivedSerialNo, log));
                }
                catch { continue; }
            }

            Devices = vcpDevices;
        }
    }
}
