using Common;
using Common.NotifyProperty;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniCamV2.Tools;
using UVC;


namespace UniCamV2.Trigger
{
    public class TriggerDC
    {
        public event Action<object> CaptureNewImage;
        public event Action<object> Shoot;

        public ObservableProperty<bool> IsInternalTrigger { get; } = new ObservableProperty<bool>();
        public int MaximumDelay { get; } = 3000;
        public ObservableProperty<int> Delay { get; } = new ObservableProperty<int>(100);
        public int MinimumDelay { get; } = 33;
        public ObservableProperty<bool> SingleStartButtonIsEnable { get; } = new ObservableProperty<bool>();
        public ObservableProperty<bool> InternalTriggerCheckBoxIsEnable { get; } = new ObservableProperty<bool>();
        public ObservableProperty<long> CycleTime { get; } = new ObservableProperty<long>();
        public bool IsRun { get; private set; }


        readonly TimeMeasure timeMeasure = new TimeMeasure();


        public TriggerDC()
        {
            IsInternalTrigger.ValueChanged += (o,n) => 
            { 
                if (n) 
                    StartCycle();
            };
        }

        public void StartCycle() => Task.Run(() => Cycle());

        private void Cycle()
        {
            SingleStartButtonIsEnable.Value = false;
            IsRun = true;

            while (true)
            {
                Thread.Sleep(Delay.Value);
                CaptureNewImage?.Invoke(this);
                timeMeasure.Restart();
                Shoot?.Invoke(this);
                CycleTime.Value = timeMeasure.Stop();

                if (!IsInternalTrigger.Value)
                    break;
            }

            SingleStartButtonIsEnable.Value = true;
            IsRun = false;
        }

        public void WaitToStop()
        {
            while (IsRun)
                Thread.Sleep(MinimumDelay);
        }

        internal void Enable(object sender, IImageSource device)
        {
            InternalTriggerCheckBoxIsEnable.Value = true;
            SingleStartButtonIsEnable.Value = true;
        }

        internal void Disable(object sender, IImageSource device)
        {
            InternalTriggerCheckBoxIsEnable.Value = false;
            SingleStartButtonIsEnable.Value = false;
        }

        internal void Shutdown()
        {
            if (IsRun)
            {
                IsInternalTrigger.Value = false;
                WaitToStop();
            }
        }
    }
}
