using Common.Language;
using Common.NotifyProperty;
using Common.Settings;
using ImageCaptureDevice.UVC.DirectShow;
using System.Linq;
using System.Threading;


namespace ImageCaptureDevice.UVC
{
    public class UVCDeviceDC : IImageCaptureDevice, ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public ImageSourceDeviceTypes Type { get; }
        public string Name { get; }
        public IReadOnlyPropertyArray<IImageCaptureDeviceResolutionInfo> AvailableResolutions { get; } = new PropertyArray<IImageCaptureDeviceResolutionInfo>();
        public IProperty<IImageCaptureDeviceResolutionInfo> SelectedResolution { get; } = new Property<IImageCaptureDeviceResolutionInfo>();
        public string Id { get; }
        public IImageCaptureDeviceProperties Properties { get; }
        public IImageCaptureDeviceOutput Output => videoCaptureDevice.Grabber;
        public IReadOnlyProperty<bool> IsRun => videoCaptureDevice.IsRun;


        readonly VideoCaptureDevice videoCaptureDevice;
        readonly object lockObject = new object();


        public UVCDeviceDC(LanguageDC languageDC, FilterInfo filterInfo)
        {
            LanguageDC = languageDC;

            Type = ImageSourceDeviceTypes.UVC;
            Name = filterInfo.Name;
            videoCaptureDevice = new VideoCaptureDevice(filterInfo.MonikerString);
            AvailableResolutions.ToSettable().AddRange(videoCaptureDevice.VideoCapabilities);
            SelectedResolution.OnValueChanged += (o, n) => videoCaptureDevice.VideoResolution = (VideoCapability)n;
            SetSelectedResolutionToMax();
            Properties = new UVCDevicePropertiesDC(languageDC, videoCaptureDevice);
            Id = new UVCDeviceInfo(filterInfo.MonikerString).Location;
        }

        private void SetSelectedResolutionToMax()
        {
            IImageCaptureDeviceResolutionInfo maxResolutionInfo = AvailableResolutions.Value.First();

            foreach(IImageCaptureDeviceResolutionInfo resolutionInfo in AvailableResolutions.Value)
            {
                if (resolutionInfo.Pixels > maxResolutionInfo.Pixels)
                    maxResolutionInfo = resolutionInfo;
            }

            SelectedResolution.Value = maxResolutionInfo;
        }

        public void Start()
        {
            lock(lockObject)
            {
                videoCaptureDevice.Start();
                videoCaptureDevice.Grabber.CaptureOccurred.WaitOne();
                Thread.Sleep(1000);
            }
        }

        public void Capture()
        {
            lock(lockObject)
            {
                if (IsRun.Value)
                    videoCaptureDevice.Grabber.CaptureOccurred.WaitOne();
            }
        }

        public void Stop()
        {
            lock(lockObject)
            {
                videoCaptureDevice.Stop();
            }
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            ((ICanSaveLoadSettings)Properties).SaveSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            ((ICanSaveLoadSettings)Properties).LoadSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }
    }
}
