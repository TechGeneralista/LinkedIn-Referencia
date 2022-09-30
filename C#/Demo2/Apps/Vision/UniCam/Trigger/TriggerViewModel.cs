using Common;
using Common.NotifyProperty;
using ImageProcess.Buffers;
using ImageProcess.Operations;
using OpenCLWrapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UniCamApp.ImageSourceSelector;
using UniCamApp.TaskList;
using UVC;


namespace UniCamApp.Trigger
{
    public class TriggerViewModel
    {
        public ISettableObservableProperty<bool> IsInternalTrigger { get; } = new ObservableProperty<bool>();
        public int MaximumDelay { get; } = 3000;
        public ISettableObservableProperty<int> Delay { get; } = new ObservableProperty<int>(100);
        public int MinimumDelay { get; } = 0;
        public ISettableObservableProperty<bool> SingleStartButtonIsEnable { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<bool> SourceComboBoxIsEnable { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<long> CycleTime { get; } = new ObservableProperty<long>();


        int automaticRestart;
        readonly ImageBufferBGR24 capturedImageBuffer;
        readonly FormatConvert formatConvert;
        readonly FlipVertical flipVertical;


        public TriggerViewModel()
        {
            IsInternalTrigger.ValueChanged += SelectedSource_ValueChanged;

            OpenCLAccelerator imageProcess = ObjectContainer.Get<OpenCLAccelerator>();
            capturedImageBuffer = new ImageBufferBGR24(imageProcess);
            formatConvert = new FormatConvert(imageProcess);
            flipVertical = new FlipVertical(imageProcess);
        }

        internal async void SingleStartButtonClick()
        {
            SingleStartButtonIsEnable.Value = false;
            await Task.Run(() => Cycle());
            SingleStartButtonIsEnable.Value = true;
        }

        private void SelectedSource_ValueChanged(bool oldValue, bool newValue)
        {
            if (newValue)
                Task.Run(() => TaskCycle());
        }

        private void TaskCycle()
        {
            SingleStartButtonIsEnable.Value = false;
            Interlocked.Exchange(ref automaticRestart, 1);

            while (IsInternalTrigger.Value)
                Cycle();

            Interlocked.Exchange(ref automaticRestart, 0);
            SingleStartButtonIsEnable.Value = true;
        }

        public Task CycleAsync() => Task.Run(() => Cycle());

        private void Cycle()
        {
            IImageSource imageSource = ObjectContainer.Get<ImageSourceSelectorViewModel>().SelectedDevice.Value;
            MainViewModel mainViewModel = ObjectContainer.Get<MainViewModel>();
            
            if(Delay.Value != 0)
                Thread.Sleep(Delay.Value);

            imageSource.Capture();

            TimeMeasure timeMeasure = TimeMeasure.StartNew();

            capturedImageBuffer.CopyToBuffer(imageSource.Frame.Width, imageSource.Frame.Height, imageSource.Frame.Format, imageSource.Frame.Buffer, imageSource.Frame.BufferLength);
            formatConvert.Convert(capturedImageBuffer);
            flipVertical.Flip(formatConvert.Output);

            try
            {
                if (mainViewModel.IsCurrentContentImageSourceSelectorOrImageOptimization())
                {
                    ObjectContainer.Get<TaskListViewModel>().LastImageBuffer = flipVertical.Output;
                    mainViewModel.ImageSource.Value = flipVertical.Output.Upload();
                }
                else
                    ObjectContainer.Get<TaskListViewModel>().RunTasks(flipVertical.Output);
            }
            catch (Exception ex)
            {
                IsInternalTrigger.Value = false;
                MessageBox.Show(ex.Message, "Error:", MessageBoxButton.OK);

                Interlocked.Exchange(ref automaticRestart, 0);
                SingleStartButtonIsEnable.Value = true;
            }

            CycleTime.Value = timeMeasure.Stop();
        }

        internal void Shutdown()
        {
            if (automaticRestart == 1)
            {
                IsInternalTrigger.Value = false;

                while (automaticRestart == 1)
                { Thread.Sleep(5); }
            }
        }
    }
}
