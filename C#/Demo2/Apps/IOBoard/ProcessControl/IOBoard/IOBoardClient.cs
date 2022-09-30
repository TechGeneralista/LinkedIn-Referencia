using Common.Log;
using Common.Prop;
using Common.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace IOBoard
{
    public enum IOBoardClientStates { Connecting, Connected }


    public class IOBoardClient
    {
        public INonSettableObservableProperty<IOBoardClientStates> Status { get; } = new ObservableProperty<IOBoardClientStates>();
        public INonSettableObservableProperty<IEnumerable<IOBoardDevice>> Devices { get; } = new ObservableProperty<IEnumerable<IOBoardDevice>>();


        readonly IPEndPoint ipEndPoint;
        readonly ILog log;
        Socket client;


        public IOBoardClient(IPAddress serverAddress, int serverPort, ILog log)
        {
            this.log = log;

            ipEndPoint = new IPEndPoint(serverAddress, serverPort);
            Task.Run(() => AutoConnectMethod());
        }

        private void AutoConnectMethod()
        {
            Status.ForceSet(IOBoardClientStates.Connecting);

            while(true)
            {
                switch(Status.Value)
                {
                    case IOBoardClientStates.Connecting:
                        try
                        {
                            if(client == null)
                                client = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                            client.Connect(ipEndPoint);
                            string response = SendAndReceive(Process.GetCurrentProcess().ProcessName);

                            if (response != "ok")
                                throw new Exception("Invalid response: " + response);

                            Status.ForceSet(IOBoardClientStates.Connected);
                        }

                        catch(Exception ex)
                        {
                            log.NewMessage(ex);

                            Devices.ForceSet(null);
                            client.Dispose();
                            client = null;
                        }

                        break;

                    case IOBoardClientStates.Connected:
                        try
                        {
                            if(Devices.Value.IsNull())
                                GetDevices();
                            else
                            {
                                string response = SendAndReceive(".");

                                if (response != "ok")
                                    throw new Exception("Invalid response: " + response);
                            }

                            Thread.Sleep(1000);
                        }

                        catch(Exception ex)
                        {
                            log.NewMessage(ex.Message);
                            Status.ForceSet(IOBoardClientStates.Connecting);

                            if(Devices.Value.IsNotNull())
                            {
                                foreach (IOBoardDevice device in Devices.Value)
                                    device.StopRefreshIfEnabled();
                            }

                            Devices.ForceSet(null);
                            client.Dispose();
                            client = null;
                        }

                        break;
                }
            }
        }

        private string SendAndReceive(string txData)
        {
            lock (client)
            {
                byte[] rxBuffer = new byte[128];
                int receivedLength = 0;

                try
                {
                    client.Send(Encoding.UTF8.GetBytes(txData));
                    receivedLength = client.Receive(rxBuffer);
                }

                catch
                {
                    return null;
                }

                return Encoding.UTF8.GetString(rxBuffer, 0, receivedLength);
            }
        }

        private string SendAndReceiveWithStatusCheck(string txData)
        {
            if (Status.Value == IOBoardClientStates.Connecting)
                return null;

            return SendAndReceive(txData);
        }

        private void GetDevices()
        {
            string[] deviceSerialNos = SendAndReceive("gdl").Split('/');
            List<IOBoardDevice> devices = new List<IOBoardDevice>();

            foreach (string serial in deviceSerialNos)
                devices.Add(new IOBoardDevice(serial, SendAndReceiveWithStatusCheck));

            Devices.ForceSet(devices);
        }

        public void Write(IOBoardArgs args)
        {
            if (Devices.Value.IsNull())
                throw new Exception(string.Format("IOBoard nem sikerült kapcsolódni a szerverhez, eszköz nem található: {0}", args.SerialNo));

            IOBoardDevice device = Devices.Value.FirstOrDefault(d => d.SerialNo == args.SerialNo);

            if (device.IsNull())
                throw new Exception(string.Format("IOBoard eszköz nem található: {0}", args.SerialNo));

            Write(device, args);
        }

        private void Write(IOBoardDevice device, IOBoardArgs args)
        {
            bool? result;

            if (args.ChannelName == "DigitalOutput" || args.ChannelName == "DO")
            {
                CheckIndex(device.DigitalOutput.Count(), args.ChannelIndex);

                if (args.LogicalValue.IsNull())
                    throw new Exception("IOBoard nincs beállítva logikai érték");

                result = device.DigitalOutput.ElementAt(args.ChannelIndex).Write((bool)args.LogicalValue);
            }

            else
                throw new Exception(string.Format("IOBoard csatorna nem létezik: {0}", args.ChannelName));

            if (result.IsNull())
                throw new Exception(string.Format("IOBoard nincs kapcsolat: {0}", args.SerialNo));
        }

        public void Read(IOBoardArgs args)
        {
            if (Devices.Value.IsNull())
                throw new Exception(string.Format("IOBoard nem sikerült kapcsolódni a szerverhez, eszköz nem található: {0}", args.SerialNo));

            IOBoardDevice device = Devices.Value.FirstOrDefault(d => d.SerialNo == args.SerialNo);

            if (device.IsNull())
                throw new Exception(string.Format("IOBoard eszköz nem található: {0}", args.SerialNo));

            Read(device, args);
        }

        private void Read(IOBoardDevice device, IOBoardArgs args)
        {
            bool? result;

            if (args.ChannelName == "DigitalInput" || args.ChannelName == "DI")
            {
                CheckIndex(device.DigitalInput.Count(), args.ChannelIndex);
                result = device.DigitalInput.ElementAt(args.ChannelIndex).Read();
            }

            else if (args.ChannelName == "DigitalInputRising" || args.ChannelName == "DIR")
            {
                CheckIndex(device.DigitalInput.Count(), args.ChannelIndex);
                result = device.DigitalInput.ElementAt(args.ChannelIndex).Read(DigitalTriggers.Rising);
            }

            else if (args.ChannelName == "DigitalInputFalling" || args.ChannelName == "DIF")
            {
                CheckIndex(device.DigitalInput.Count(), args.ChannelIndex);
                result = device.DigitalInput.ElementAt(args.ChannelIndex).Read(DigitalTriggers.Falling);
            }

            else if (args.ChannelName == "DigitalOutput" || args.ChannelName == "DO")
            {
                CheckIndex(device.DigitalOutput.Count(), args.ChannelIndex);
                result = device.DigitalOutput.ElementAt(args.ChannelIndex).Read();
            }

            else if (args.ChannelName == "DigitalOutputRising" || args.ChannelName == "DOR")
            {
                CheckIndex(device.DigitalOutput.Count(), args.ChannelIndex);
                result = device.DigitalOutput.ElementAt(args.ChannelIndex).Read(DigitalTriggers.Rising);
            }

            else if (args.ChannelName == "DigitalOutputFalling" || args.ChannelName == "DOF")
            {
                CheckIndex(device.DigitalOutput.Count(), args.ChannelIndex);
                result = device.DigitalOutput.ElementAt(args.ChannelIndex).Read(DigitalTriggers.Falling);
            }

            else
                throw new Exception(string.Format("IOBoard csatorna nem létezik: {0}", args.ChannelName));

            if (result.IsNull())
                throw new Exception(string.Format("IOBoard nincs kapcsolat: {0}", args.SerialNo));

            args.LogicalValue = result;
        }

        private void CheckIndex(int elementCount, int index)
        {
            if (index < 0 || index > (elementCount - 1))
                throw new Exception(string.Format("IOBoard csatorna index határokon kívűl: {0}", index));
        }
    }
}
