using SmartVisionClientApp.CameraSelect;
using SmartVisionClientApp.Common;
using SmartVisionClientApp.Communication;
using SmartVisionClientApp.DTOs;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SmartVisionClientApp.Trigger
{
    public class TriggerViewModel
    {
        public string[] Sources { get; } = new string[] { "Külső", "Belső" };
        public ISettableObservableProperty<string> SelectedSource { get; }
        public int MaximumDelay { get; } = 3000;
        public ISettableObservableProperty<int> Delay { get; } = new ObservableProperty<int>(100);
        public int MinimumDelay { get; } = 0;
        public ISettableObservableProperty<bool> SingleStartButtonIsEnable { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<bool> SourceComboBoxIsEnable { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<long> CycleTime { get; } = new ObservableProperty<long>();


        int automaticRestart;


        public TriggerViewModel()
        {
            SelectedSource = new ObservableProperty<string>(Sources[0]);
            SelectedSource.ValueChanged += SelectedSource_ValueChanged;
        }

        internal async void SingleStartButtonClick()
        {
            SingleStartButtonIsEnable.Value = false;
            await Task.Run(() => Cycle());
            SingleStartButtonIsEnable.Value = true;
        }

        private void SelectedSource_ValueChanged(string newValue)
        {
            if (newValue == Sources[1])
                Task.Run(() => TaskCycle());
        }

        private void TaskCycle()
        {
            SingleStartButtonIsEnable.Value = false;
            Interlocked.Exchange(ref automaticRestart, 1);

            while (SelectedSource.Value == Sources[1])
                Cycle();

            Interlocked.Exchange(ref automaticRestart, 0);
            SingleStartButtonIsEnable.Value = true;
        }

        private void Cycle()
        {
            Thread.Sleep(Delay.Value);

            BitmapSource capturedImage = ObjectContainer.Get<Camera>().Capture();

            TimeMeasure timeMeasure = TimeMeasure.StartNew();

            try
            {
                ObjectContainer.Get<MainWindowViewModel>().CurrentImage.Value = capturedImage;
            }
            catch (Exception ex)
            {
                SelectedSource.Value = Sources[0];
                Utils.ShowError(ex.Message);
            }

            CycleTime.Value = timeMeasure.Stop();
        }

        public Task StopAndWaitForStopAsync() => Task.Run(() =>StopAndWaitForStop());

        internal void StopAndWaitForStop()
        {
            if (automaticRestart == 1)
            {
                SelectedSource.Value = Sources[0];

                while (automaticRestart == 1)
                { Thread.Sleep(5); }
            }
        }
    }
}
