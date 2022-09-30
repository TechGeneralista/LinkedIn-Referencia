using Microsoft.Win32;
using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;


namespace BalanceLib
{
    public class Balance : ObservableProperty
    {
        public event Action ConnectionError;

        public string SerialNo
        {
            get => serialNo;
            set
            {
                serialNo = value;

                if (string.IsNullOrEmpty(serialNo))
                    throw new ArgumentNullException(nameof(SerialNo));

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\BalanceV1\" + serialNo);

                if(key != null)
                {
                    zeroCalibrationValue = double.Parse(key.GetValue(nameof(zeroCalibrationValue)).ToString());
                    weightCalibrationDivider = double.Parse(key.GetValue(nameof(weightCalibrationDivider)).ToString());
                }
            }
        }

        public double IrawValue0
        {
            get => irawValue0;
            private set => SetField(value, ref irawValue0);
        }

        public double IrawValue1
        {
            get => irawValue1;
            private set => SetField(value, ref irawValue1);
        }

        public double IrawValue2
        {
            get => irawValue2;
            private set => SetField(value, ref irawValue2);
        }

        public double IrawValue3
        {
            get => irawValue3;
            private set => SetField(value, ref irawValue3);
        }

        public double CalibrationWeight
        {
            get => calibrationWeight;
            set => SetField(value, ref calibrationWeight);
        }

        public double MeasuredWeight
        {
            get => measuredWeight;
            private set => SetField(value, ref measuredWeight);
        }

        public bool MeasureOk
        {
            get => measureOk;
            private set => SetField(value, ref measureOk);
        }

        string serialNo;
        Task getValueTask;
        SerialPort serialPort;

        double irawValue0;
        double irawValue1;
        double irawValue2;
        double irawValue3;

        double calibrationWeight;
        double offsettedValue;
        double derivative;
        double lastMeasuredWeight;
        double measuredWeight;

        double integrateValue;

        double zeroCalibrationValue;
        double weightCalibrationDivider;
        bool measureOk;


        public Balance()
        {
            calibrationWeight = 1;
            integrateValue = 0.1;
            weightCalibrationDivider = 1;

            serialPort = new SerialPort();
            serialPort.ReadTimeout = 100;
        }

        public void Connect()
        {
            if (string.IsNullOrEmpty(SerialNo))
                throw new ArgumentNullException(nameof(SerialNo));

            string[] availablePorts = SerialPort.GetPortNames();
            bool error = true;

            foreach (string port in availablePorts)
            {
                try
                {
                    serialPort.PortName = port;
                    serialPort.Open();
                    serialPort.Write("GetInfo");
                    Thread.Sleep(100);
                    string receivedData = serialPort.ReadExisting();

                    if (receivedData.Contains(SerialNo))
                    {
                        StartGetValuesTask();
                        error = false;
                        break;
                    }

                    serialPort.Close();
                }
                catch
                {
                    if (serialPort.IsOpen)
                        serialPort.Close();
                }
            }

            if (error)
                ConnectionError?.Invoke();
        }

        private void StartGetValuesTask()
        {
            getValueTask = new Task(GetValueMethod);
            getValueTask.Start();
        }

        private void GetValueMethod()
        {
            while (true)
            {
                serialPort.Write("GetRawData");
                Thread.Sleep(50);
                string rawData = serialPort.ReadExisting();
                string[] rawDataArray = rawData.Split('/');
                int rawValue0i = int.Parse(rawDataArray[0]);
                int rawValue1i = int.Parse(rawDataArray[1]);
                int rawValue2i = int.Parse(rawDataArray[2]);
                int rawValue3i = int.Parse(rawDataArray[3]);

                IrawValue0 += (rawValue0i - irawValue0) * integrateValue;
                IrawValue1 += (rawValue1i - irawValue1) * integrateValue;
                IrawValue2 += (rawValue2i - irawValue2) * integrateValue;
                IrawValue3 += (rawValue3i - irawValue3) * integrateValue;

                offsettedValue = (irawValue0 + irawValue1 + irawValue2 + irawValue3) - zeroCalibrationValue;
                double measuredWeight = offsettedValue / weightCalibrationDivider;
                MeasuredWeight = Math.Round(measuredWeight, 1);

                derivative = Math.Abs(measuredWeight - lastMeasuredWeight);
                lastMeasuredWeight = measuredWeight;

                if (derivative < 0.1)
                    MeasureOk = true;
                else
                    MeasureOk = false;
            }
        }

        public void Zero()
        {
            zeroCalibrationValue = irawValue0 + irawValue1 + irawValue2 + irawValue3;
            RegistryKey zeroCalibrationValueKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\BalanceV1\" + serialNo);
            zeroCalibrationValueKey.SetValue(nameof(zeroCalibrationValue), zeroCalibrationValue);
        }

        public void Calibrate()
        {
            weightCalibrationDivider = offsettedValue / calibrationWeight;
            RegistryKey weightCalibrationDividerKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\BalanceV1\" + serialNo);
            weightCalibrationDividerKey.SetValue(nameof(weightCalibrationDivider), weightCalibrationDivider);
        }
    }
}
