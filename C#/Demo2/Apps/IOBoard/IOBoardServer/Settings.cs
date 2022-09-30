using System.IO;
using System.Net;


namespace IOBoardServer
{
    public class Settings
    {
        public IPAddress ServerIPAddress { get; private set; }
        public int ServerPort { get; private set; }


        public Settings(string cfgFileName)
        {
            string[] data = File.ReadAllLines(cfgFileName);
            ServerIPAddress = IPAddress.Parse(data[0]);
            ServerPort = int.Parse(data[1]);
        }
    }
}
