using Common;
using Common.NotifyProperty;
using System;
using System.Threading;
using UVC;


namespace UniCamV2.Content.Model
{
    public class ImageSourceDC
    {
        public event Action<object, IImageSource> Connected;
        public event Action<object, IImageSource> Disconnected;

        public UVCDeviceScanner UVCDeviceScanner { get; } = new UVCDeviceScanner();
        public ObservableProperty<IImageSource> SelectedDevice { get; } = new ObservableProperty<IImageSource>();
        public ObservableProperty<bool> ScanButtonIsEnable { get; } = new ObservableProperty<bool>();
        public ObservableProperty<bool> StartButtonIsEnable { get; } = new ObservableProperty<bool>();
        public ObservableProperty<bool> StopButtonIsEnable { get; } = new ObservableProperty<bool>();


        public ImageSourceDC()
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

        internal async void Scan()
        {
            DisableAllControls();
            SelectedDevice.Value = null;
            await UVCDeviceScanner.ScanAsync();

            if (UVCDeviceScanner.AvailableDevices.Value.Length == 1)
                SelectedDevice.Value = UVCDeviceScanner.AvailableDevices.Value[0];

            ScanButtonIsEnable.Value = true;
        }

        private void DisableAllControls()
        {
            ScanButtonIsEnable.Value = false;
            StartButtonIsEnable.Value = false;
            StopButtonIsEnable.Value = false;
        }

        internal async void Connect()
        {
            DisableAllControls();
            await SelectedDevice.Value.StartAsync();
            //await System.Threading.Tasks.Task.Run(() => Thread.Sleep(3000));
            StopButtonIsEnable.Value = true;
            Connected?.Invoke(this, SelectedDevice.Value);
        }

        internal async void Disconnect()
        {
            DisableAllControls();

            if (SelectedDevice.Value.IsNotNull() && SelectedDevice.Value.IsRunning)
                await SelectedDevice.Value.StopAsync();

            ScanButtonIsEnable.Value = true;
            StartButtonIsEnable.Value = true;
            Disconnected?.Invoke(this, SelectedDevice.Value);
        }

        internal void Shutdown()
        {
            if (SelectedDevice.Value.IsNotNull() && SelectedDevice.Value.IsRunning)
                SelectedDevice.Value.Stop();
        }
    }
}
