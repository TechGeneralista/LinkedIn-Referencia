using System;
using System.IO;
using System.Net;


namespace ProcessControlApp
{
    public class Settings
    {
        public IPAddress ServerIPAddress { get; private set; }
        public int ServerPort { get; private set; }


        public Settings(string cfgFileName)
        {
            cfgFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cfgFileName);
            string[] data = File.ReadAllLines(cfgFileName);
            ServerIPAddress = IPAddress.Parse(data[0]);
            ServerPort = int.Parse(data[1]);
        }
    }
}
