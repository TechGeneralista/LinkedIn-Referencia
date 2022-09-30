using CommonLib.Components;
using CommonLib.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UVCLib;


namespace ComponentCheckApp.Views.DeviceSelect
{
    public enum DeviceSelectModelStatus { Work, DeviceAvailable, DeviceNotAvailable, DeviceConnected, DeviceDisconnected }

    public class DeviceSelectModel : ObservableProperty
    {
        public event Action<WriteableBitmap> NewImage;
        public event Action<DeviceSelectModelStatus> StatusChanged;

        public UVCDeviceScanner UVCDeviceScanner { get; private set; }

        public IVideoCaptureDevice SelectedUVCDevice
        {
            get => selectedUVCDevice;
            set => SetField(value, ref selectedUVCDevice);
        }

        public IVideoCaptureDevice ConnectedUVCDevice { get; private set; }

        public BitmapSource PreviewImage
        {
            get => previewImage;
            set => SetField(value, ref previewImage);
        }

        public DeviceSelectModelStatus Status
        {
            get => status;
            set { SetField(value, ref status); StatusChanged?.Invoke(status); }
        }


        DeviceSelectModelStatus status;
        IVideoCaptureDevice selectedUVCDevice;
        BitmapSource previewImage;


        public DeviceSelectModel()
        {
            UVCDeviceScanner = new UVCDeviceScanner();
            ScanAsync();
        }

        public Task ScanAsync() => Task.Run(() => Scan());

        public void Scan()
        {
            Status = DeviceSelectModelStatus.Work;

            SelectedUVCDevice = null;
            UVCDeviceScanner.Scan();

            if (UVCDeviceScanner.AvailableDevices.Length > 0)
            {
                SelectedUVCDevice = UVCDeviceScanner.AvailableDevices[0];
                Status = DeviceSelectModelStatus.DeviceAvailable;
            }
            else
            {
                SelectedUVCDevice = null;
                Status = DeviceSelectModelStatus.DeviceNotAvailable;
            }
        }

        public Task ConnectAsync() => Task.Run(() => Connect());

        public void Connect()
        {
            Status = DeviceSelectModelStatus.Work;

            ConnectedUVCDevice = selectedUVCDevice;
            ConnectedUVCDevice.NewImageEvent += ConnectedUVCDevice_NewImageEvent;
            ConnectedUVCDevice.Connect();

            if (ConnectedUVCDevice.IsRunning)
            {
                Thread.Sleep(2000);
                ConnectedUVCDevice.Capture();
                Status = DeviceSelectModelStatus.DeviceConnected;
            }
            else
            {
                Disconnect();
            }
        }

        private void ConnectedUVCDevice_NewImageEvent(WriteableBitmap image)
        {
            PreviewImage = image;
            NewImage?.Invoke(image);
        }

        public void ShowSettings()
        {
            ConnectedUVCDevice.ImagePropertiesChangedEvent += ConnectedUVCDevice_ImagePropertiesChangedEvent;
            ConnectedUVCDevice.ShowImageProperties();
        }

        private void ConnectedUVCDevice_ImagePropertiesChangedEvent()
        {
            ConnectedUVCDevice.Capture();
            ConnectedUVCDevice.ImagePropertiesChangedEvent -= ConnectedUVCDevice_ImagePropertiesChangedEvent;
        }

        public Task DisconnectAsync() => Task.Run(() => Disconnect());

        public void Disconnect()
        {
            Status = DeviceSelectModelStatus.Work;

            ConnectedUVCDevice.NewImageEvent -= ConnectedUVCDevice_NewImageEvent;
            ConnectedUVCDevice.Disconnect();
            ConnectedUVCDevice = null;
            PreviewImage = null;
            Status = DeviceSelectModelStatus.DeviceDisconnected;
        }
    }
}
