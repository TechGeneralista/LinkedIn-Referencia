using IOBoardServer.IOBoard.VCP;
using System.Collections.Generic;


namespace IOBoardServer.IOBoard
{
    public class IOBoardScanner : IIOBoardScanner
    {
        static readonly string AcceptableDeviceType = "C6EAC0BC";


        public IEnumerable<IIOBoardDevice> Devices { get; private set; }


        ILog log;


        public IOBoardScanner(ILog log)
        {
            this.log = log;
        }

        public void Scan()
        {
            log.NewMessage(LogLevels.Information, nameof(IIOBoardDevice) + " -> Scanning...");
            List<IIOBoardDevice> devList = new List<IIOBoardDevice>();

            IIOBoardScanner scanner = new VCPIOBoardScanner(AcceptableDeviceType, log);
            scanner.Scan();
            devList.AddRange(scanner.Devices);

            Devices = devList;
            log.NewMessage(LogLevels.Information, nameof(IIOBoardDevice) + " -> Scan complete");
        }
    }
}