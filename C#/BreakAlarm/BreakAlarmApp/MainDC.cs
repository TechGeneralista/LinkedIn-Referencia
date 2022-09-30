using Communication.TCP.Server.SingleClient;
using System.Net.Sockets;


namespace BreakAlarmApp
{
    internal class MainDC
    {
        public TimedAlarms TimedAlarms { get; } = new TimedAlarms();
        //public ExternalAlarms ExternalAlarms { get; } = new ExternalAlarms();
        //public TCPServerSingleClientDC TCPServerSingleClientDC { get; } = new TCPServerSingleClientDC();


        public MainDC()
        {
            //TCPServerSingleClientDC.DataReceived += TCPServerSingleClientDC_DataReceived;
        }

        private void TCPServerSingleClientDC_DataReceived(Socket clientSocket, string data)
        {
            int inputIndex = -1;

            try { inputIndex = int.Parse(data); }
            catch { return; }

            //ExternalAlarms.Start(inputIndex);
        }

        internal void Write()
        {
            TimedAlarms.WriteList();
            //ExternalAlarms.WriteList();
            //TCPServerSingleClientDC.Write();
        }
    }
}