using Common;
using Common.Interfaces;
using Common.Language;
using Common.NotifyProperty;
using Common.PopupWindow;
using ImageCaptureDevice.Interfaces;
using ImageCaptureDevice.UVC.DirectShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ImageCaptureDevice.UVC
{
    public class UVCDeviceScannerDC : ICanScan, ICanConnect, ICanAddNewImageCaptureDevice, IHasStartDevice, IHasHandleType, ICanStartImageCaptureDevice
    {
        public event Action<IImageCaptureDevice> StartDevice;

        public ImageSourceDeviceTypes HandleType { get; } = ImageSourceDeviceTypes.UVC;
        public LanguageDC LanguageDC { get; }
        public IReadOnlyPropertyArray<IImageCaptureDevice> AvailableDevices { get; } = new PropertyArray<IImageCaptureDevice>();
        public IProperty<IImageCaptureDevice> SelectedDevice { get; } = new Property<IImageCaptureDevice>();


        readonly PopupWindowDC popupWindowDC;
        readonly List<IImageCaptureDevice> startedDevices = new List<IImageCaptureDevice>();


        public UVCDeviceScannerDC(LanguageDC languageDC, PopupWindowDC popupWindowDC)
        {
            LanguageDC = languageDC;
            this.popupWindowDC = popupWindowDC;
        }

        public async void ScanButtonClick()
        {
            popupWindowDC.Show();
            await ScanAsync();
            popupWindowDC.Close();
        }

        public Task ScanAsync() => Task.Run(()=>Scan());

        public void Scan()
        {
            List<IImageCaptureDevice> availableImageCaptureDevices = new List<IImageCaptureDevice>();

            try
            {
                FilterInfoCollection filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                foreach (FilterInfo filterInfo in filterInfoCollection)
                {
                    try
                    {
                        availableImageCaptureDevices.Add(new UVCDeviceDC(LanguageDC, filterInfo));
                    }
                    catch { }
                }
            }
            catch { }


            List<IImageCaptureDevice> availableDevicesToRemove = new List<IImageCaptureDevice>();

            foreach (IImageCaptureDevice availableDevice in availableImageCaptureDevices)
            {
                foreach (IImageCaptureDevice startedDevice in startedDevices)
                {
                    if (availableDevice.Type == startedDevice.Type && availableDevice.Id == startedDevice.Id)
                        availableDevicesToRemove.Add(availableDevice);
                }
            }

            foreach (IImageCaptureDevice imageCaptureDevice in availableDevicesToRemove)
                availableImageCaptureDevices.Remove(imageCaptureDevice);

            AvailableDevices.ToSettable().ReAddRange(availableImageCaptureDevices.ToArray());

            if (AvailableDevices.Value.Length > 0)
                SelectedDevice.Value = AvailableDevices.Value.First();
        }

        public async void ConnectButtonClick()
        {
            if (SelectedDevice.Value.IsNull())
                return;

            popupWindowDC.Show();
            await ConnectAsync();
            popupWindowDC.Close();
        }

        public Task ConnectAsync()
            => Task.Run(() => Connect());

        public void Connect()
        {
            IImageCaptureDevice imageCaptureDevice = SelectedDevice.Value;
            SelectedDevice.Value = null;
            AvailableDevices.ToSettable().Remove(imageCaptureDevice);
            startedDevices.Add(imageCaptureDevice);
            StartDevice?.Invoke(imageCaptureDevice);
        }

        public void AddNewImageCaptureDevice(IImageCaptureDevice imageCaptureDevice)
        {
            if(imageCaptureDevice.Type == ImageSourceDeviceTypes.UVC)
            {
                startedDevices.Remove(imageCaptureDevice);
                AvailableDevices.ToSettable().Add(imageCaptureDevice);
            }
        }

        public void StartImageCaptureDevice(string id, string selectedResolution)
        {
            IImageCaptureDevice imageCaptureDevice = AvailableDevices.Value.FirstOrDefault(x => x.Id == id);

            if(imageCaptureDevice.IsNull())
            {
                Scan();
                imageCaptureDevice = AvailableDevices.Value.FirstOrDefault(x => x.Id == id);

                if (imageCaptureDevice.IsNull())
                    return;
            }

            SelectedDevice.Value = imageCaptureDevice;

            if(SelectedDevice.Value.SelectedResolution.Value.Text != selectedResolution)
            {
                IImageCaptureDeviceResolutionInfo imageCaptureDeviceResolutionInfo = SelectedDevice.Value.AvailableResolutions.Value.FirstOrDefault(x => x.Text == selectedResolution);

                if (imageCaptureDeviceResolutionInfo.IsNull())
                    return;

                SelectedDevice.Value.SelectedResolution.Value = imageCaptureDeviceResolutionInfo;
            }

            Connect();
        }
    }
}
