using CommonLib.Components;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;


namespace D4I4OLib
{
    public enum Statuses { Scanning, Run }

    public class D4I4O : ObservableProperty
    {
        public event Action In0RisingEvent;
        public event Action In0FallingEvent;
        public event Action In1RisingEvent;
        public event Action In1FallingEvent;
        public event Action In2RisingEvent;
        public event Action In2FallingEvent;
        public event Action In3RisingEvent;
        public event Action In3FallingEvent;


        public string GUID
        {
            get => guid;
            set => SetField(value, ref guid);
        }

        public Statuses Status
        {
            get => status;
            private set => SetField(value, ref status);
        }

        public int RefreshInterval
        {
            get => refreshInterval;
            set => SetField(value, ref refreshInterval);
        }

        public bool In0
        {
            get { return in0; }
            private set
            {
                if(in0 == false && value == true)
                    In0RisingEvent?.Invoke();

                if(in0 == true && value == false)
                    In0FallingEvent?.Invoke();

                in0 = value;

                if (in0)
                    In0Color = Colors.Green.ToString();
                else
                    In0Color = Colors.Red.ToString();
            }
        }

        public string In0Color
        {
            get => in0Color;
            private set => SetField(value, ref in0Color);
        }

        public bool In1
        {
            get { return in1; }
            private set
            {
                if(in1 == false && value == true)
                    In1RisingEvent?.Invoke();

                if(in1 == true && value == false)
                    In1FallingEvent?.Invoke();

                in1 = value;

                if(in1)
                    In1Color = Colors.Green.ToString();
                else
                    In1Color = Colors.Red.ToString();
            }
        }

        public string In1Color
        {
            get => in1Color;
            private set => SetField(value, ref in1Color);
        }

        public bool In2
        {
            get { return in2; }
            private set
            {
                if(in2 == false && value == true)
                    In2RisingEvent?.Invoke();

                if(in2 == true && value == false)
                    In2FallingEvent?.Invoke();

                in2 = value;

                if(in2)
                    In2Color = Colors.Green.ToString();
                else
                    In2Color = Colors.Red.ToString();
            }
        }

        public string In2Color
        {
            get => in2Color;
            private set => SetField(value, ref in2Color);
        }

        public bool In3
        {
            get { return in3; }
            private set
            {
                if(in3 == false && value == true)
                    In3RisingEvent?.Invoke();

                if(in3 == true && value == false)
                    In3FallingEvent?.Invoke();

                in3 = value;

                if(in3)
                    In3Color = Colors.Green.ToString();
                else
                    In3Color = Colors.Red.ToString();
            }
        }

        public string In3Color
        {
            get => in3Color;
            private set => SetField(value, ref in3Color);
        }

        public bool Out0
        {
            get => out0;
            set => SetField(value, ref out0);
        }

        public bool Out1
        {
            get => out1;
            set => SetField(value, ref out1);
        }

        public bool Out2
        {
            get => out2;
            set => SetField(value, ref out2);
        }

        public bool Out3
        {
            get => out3;
            set => SetField(value, ref out3);
        }



        //Fields
        string guid;
        SerialPort serialPort;
        Statuses status;
        int refreshInterval;
        int responseTimeout;
        int charTimeout;
        string receivedData;
        bool in0;
        string in0Color;
        bool in1;
        string in1Color;
        bool in2;
        string in2Color;
        bool in3;
        string in3Color;
        bool enableEvents;
        bool out0;
        bool out1;
        bool out2;
        bool out3;



        //Constructor
        public D4I4O()
        {
            receivedData = "";
            responseTimeout = 500;
            charTimeout = 50;
            RefreshInterval = 0;
            SetAllSCB(Colors.Orange);
            Task.Run(() => ComTask());
        }


        //Methods
        private void SetAllSCB(Color c)
        {
            In3Color = In2Color = In1Color = In0Color = c.ToString();
        }

        private void ComTask()
        {
            while(true)
            {
                if(status == Statuses.Scanning)
                {
                    SetAllSCB(Colors.Orange);

                    if(!string.IsNullOrEmpty(guid))
                    {
                        string[] availablePorts = SerialPort.GetPortNames();

                        if (availablePorts.Length > 0)
                        {
                            foreach (string str in availablePorts)
                            {
                                if (serialPort != null)
                                {
                                    if (serialPort.IsOpen)
                                    {
                                        try
                                        {
                                            serialPort.Close();
                                        }
                                        catch { }
                                    }
                                }

                                serialPort = new SerialPort();
                                serialPort.PortName = str;
                                serialPort.BaudRate = 115200;

                                try
                                {
                                    serialPort.Open();
                                    receivedData = SendAndReceive("ID?");

                                    if (receivedData == guid)
                                    {
                                        receivedData = "";
                                        status = Statuses.Run;
                                        enableEvents = false;
                                        break;
                                    }
                                }

                                catch
                                {

                                }
                            }
                        }
                    }
                }

                else if(status == Statuses.Run)
                {
                    try
                    {
                        string outputCommand = "";

                        if(out0)
                            outputCommand += "O0H";
                        else
                            outputCommand += "O0L";

                        if(out1)
                            outputCommand += "O1H";
                        else
                            outputCommand += "O1L";

                        if(out2)
                            outputCommand += "O2H";
                        else
                            outputCommand += "O2L";

                        if(out3)
                            outputCommand += "O3H";
                        else
                            outputCommand += "O3L";

                        SendAndReceive(outputCommand);

                        receivedData = SendAndReceive("IN?");

                        char[] arr = receivedData.ToCharArray();

                        if(arr.Length != 0)
                        {
                            if(arr[0] == 'L')
                                In0 = false;
                            if(arr[0] == 'H')
                                In0 = true;

                            if(arr[1] == 'L')
                                In1 = false;
                            if(arr[1] == 'H')
                                In1 = true;

                            if(arr[2] == 'L')
                                In2 = false;
                            if(arr[2] == 'H')
                                In2 = true;

                            if(arr[3] == 'L')
                                In3 = false;
                            if(arr[3] == 'H')
                                In3 = true;
                        }

                        if(enableEvents == false)
                            enableEvents = true;
                        
                        Thread.Sleep(refreshInterval);
                    }

                    catch
                    {
                        Status = Statuses.Scanning;
                    }
                }
            }
        }

        private string SendAndReceive(string data)
        {
            serialPort.Write(data);

            string rxBuffer = "";
            Stopwatch responseStopwatch = new Stopwatch();
            Stopwatch charStopwatch = new Stopwatch();
            responseStopwatch.Start();

            while(responseStopwatch.ElapsedMilliseconds < responseTimeout && charStopwatch.ElapsedMilliseconds < charTimeout)
            {
                if(serialPort.BytesToRead != 0)
                {
                    if(responseStopwatch.IsRunning)
                        responseStopwatch.Stop();

                    charStopwatch.Restart();

                    rxBuffer += (char)serialPort.ReadChar();
                }

                Thread.Sleep(1);
            }

            return rxBuffer;
        }
    }
}