using Common.Prop;
using Common.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace IOBoard
{
    public enum DigitalTriggers { Rising, Falling }


    public class IOBoardDevice
    {
        public string SerialNo { get; }
        public IEnumerable<IOBoardDigitalInput> DigitalInput { get; private set; }
        public IEnumerable<IOBoardDigitalOutput> DigitalOutput { get; private set; }
        public ISettableObservableProperty<bool> IsRefreshEnable { get; } = new ObservableProperty<bool>();


        readonly Func<string, string> sendAndReceive;
        Task refreshTask;
        CancellationTokenSource refreshCancellationTokenSource;


        public IOBoardDevice(string serialNo, Func<string, string> sendAndReceive)
        {
            SerialNo = serialNo;
            this.sendAndReceive = sendAndReceive;
            IsRefreshEnable.ValueChanged += SelectedRefreshInterval_ValueChanged;

            GetDigitalInput();
            GetDigitalOutput();
        }

        private void SelectedRefreshInterval_ValueChanged(bool b)
        {
            if (b)
            {
                if (refreshTask.IsNull())
                {
                    refreshCancellationTokenSource = new CancellationTokenSource();
                    refreshTask = Task.Run(() => RefreshMethod(), refreshCancellationTokenSource.Token);
                }
            }
            else
                StopRefreshThread();
        }

        public void StopRefreshIfEnabled()
        {
            if (IsRefreshEnable.Value)
                StopRefreshThread();
        }

        private void StopRefreshThread()
        {
            refreshCancellationTokenSource.Cancel();

            while (refreshTask.Status != TaskStatus.RanToCompletion)
                Thread.Sleep(100);

            refreshCancellationTokenSource = null;
            refreshTask = null;
        }

        private void RefreshMethod()
        {
            while (!refreshCancellationTokenSource.IsCancellationRequested)
            {
                foreach (IOBoardDigitalInput digitalInput in DigitalInput)
                    digitalInput.Refresh();

                foreach (IOBoardDigitalOutput digitalOutput in DigitalOutput)
                    digitalOutput.Refresh();

                Thread.Sleep(100);
            }
        }

        private void GetDigitalInput()
        {
            string response = SendAndReceive("gccdi");

            if(response == null || response == "e" || response == "enc")
            {
                DigitalInput = new IOBoardDigitalInput[0];
                return;
            }

            int count = int.Parse(response);
            List<IOBoardDigitalInput> inputs = new List<IOBoardDigitalInput>();

            for (int i = 0; i < count; i++)
                inputs.Add(new IOBoardDigitalInput(i, SendAndReceive));

            DigitalInput = inputs;
        }

        private void GetDigitalOutput()
        {
            string response = SendAndReceive("gccdo");

            if (response == null || response == "e" || response == "enc")
            {
                DigitalOutput = new IOBoardDigitalOutput[0];
                return;
            }

            int count = int.Parse(SendAndReceive("gccdo"));
            List<IOBoardDigitalOutput> outputs = new List<IOBoardDigitalOutput>();

            for (int i = 0; i < count; i++)
                outputs.Add(new IOBoardDigitalOutput(i, SendAndReceive));

            DigitalOutput = outputs;
        }

        private string SendAndReceive(string txData) => sendAndReceive.Invoke(SerialNo + '/' + txData);
    }
}