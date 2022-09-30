using Common;
using Common.NotifyProperty;
using System.Threading;
using System.Threading.Tasks;
using UniCamApp.Trigger;
using UVC;


namespace UniCamApp.ImageSourceSelector
{
    public class ImageSourceSelectorViewModel
    {
        public UVCDeviceScanner UVCDeviceScanner { get; } = new UVCDeviceScanner();
        public ISettableObservableProperty<IImageSource> SelectedDevice { get; } = new ObservableProperty<IImageSource>();
        public ISettableObservableProperty<bool> ScanButtonIsEnable { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<bool> StartButtonIsEnable { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<bool> StopButtonIsEnable { get; } = new ObservableProperty<bool>();


        public ImageSourceSelectorViewModel()
        {
            ScanButtonIsEnable.Value = true;
            SelectedDevice.ValueChanged += SelectedDeviceChanged;
        }

        private void SelectedDeviceChanged(IImageSource oldValue, IImageSource newValue)
        {
            if(newValue.IsNotNull())
                StartButtonIsEnable.Value = true;
            else
                StartButtonIsEnable.Value = false;
        }

        internal async void ScanButtonClick()
        {
            ScanButtonIsEnable.Value = false;
            StartButtonIsEnable.Value = false;
            StopButtonIsEnable.Value = false;

            SelectedDevice.Value = null;

            await UVCDeviceScanner.ScanAsync();

            if (UVCDeviceScanner.AvailableDevices.Value.Length == 1)
                SelectedDevice.Value = UVCDeviceScanner.AvailableDevices.Value[0];

            ScanButtonIsEnable.Value = true;
        }

        internal async void StartButtonClick()
        {
            TriggerViewModel triggerViewModel = ObjectContainer.Get<TriggerViewModel>();
            MainViewModel mainViewModel = ObjectContainer.Get<MainViewModel>();

            ScanButtonIsEnable.Value = false;
            StartButtonIsEnable.Value = false;
            StopButtonIsEnable.Value = false;
            await SelectedDevice.Value.StartAsync();

            await Task.Run(() => Thread.Sleep(1000));

            StopButtonIsEnable.Value = true;

            await triggerViewModel.CycleAsync();
            triggerViewModel.SourceComboBoxIsEnable.Value = true;
            triggerViewModel.SingleStartButtonIsEnable.Value = true;
            mainViewModel.ShowImageOptimizationContentButtonIsEnable.Value = true;
            mainViewModel.ShowTasksContentButtonIsEnable.Value = true;
        }

        internal async void StopButtonClick()
        {
            ScanButtonIsEnable.Value = false;
            StartButtonIsEnable.Value = false;
            StopButtonIsEnable.Value = false;

            if(SelectedDevice.Value.IsNotNull() && SelectedDevice.Value.IsRunning)
                await SelectedDevice.Value.StopAsync();

            ScanButtonIsEnable.Value = true;
            StartButtonIsEnable.Value = true;

            MainViewModel mainViewModel = ObjectContainer.Get<MainViewModel>();
            mainViewModel.ShowImageOptimizationContentButtonIsEnable.Value = false;
            mainViewModel.ShowTasksContentButtonIsEnable.Value = false;
            mainViewModel.SetBlackImage();

            TriggerViewModel triggerViewModel = ObjectContainer.Get<TriggerViewModel>();
            triggerViewModel.SourceComboBoxIsEnable.Value = false;
            triggerViewModel.SingleStartButtonIsEnable.Value = false;
        }

        internal void Shutdown()
        {
            if (SelectedDevice.Value.IsNotNull() && SelectedDevice.Value.IsRunning)
                SelectedDevice.Value.Stop();
        }
    }
}
