using Common.Settings;
using ImageSourceDevice.UVC.DirectShow;
using Language;
using System;
using System.Drawing;


namespace ImageSourceDevice.UVC
{
    public class UVCDevice : IImageSourceDevice, ICanSaveLoadSettings
    {
        public ImageSourceDeviceTypes Type { get; }
        public string Resolution { get; }
        public int FrameRate { get; }
        public string Location { get; }
        public string Address { get; }
        public IImageSourceDeviceProperties Properties { get; }
        public IImageSourceDeviceOutput Output => videoCaptureDevice.Grabber.Output;

        
        readonly VideoCaptureDevice videoCaptureDevice;


        public UVCDevice(LanguageDC languageDC, FilterInfo filterInfo)
        {
            videoCaptureDevice = new VideoCaptureDevice(filterInfo.MonikerString);
            Properties = new UVCDevicePropertiesDC(languageDC, videoCaptureDevice);
            VideoCapability selectedResolution = GetMaxResolution();
            videoCaptureDevice.VideoResolution = selectedResolution;
            UVCDeviceInfo uvcDeviceInfo = new UVCDeviceInfo(filterInfo.MonikerString);

            Type = ImageSourceDeviceTypes.UVC;
            Resolution = selectedResolution.FrameSize.Width + "x" + selectedResolution.FrameSize.Height;
            FrameRate = selectedResolution.AverageFrameRate;
            Location = uvcDeviceInfo.Location;
            Address = null;
        }

        private VideoCapability GetResolution(Size size)
        {
            VideoCapability[] videoCapabilities = videoCaptureDevice.VideoCapabilities;

            if (videoCapabilities.Length == 0)
                throw new NotSupportedException(nameof(GetResolution));

            for (int i = 0; i < videoCapabilities.Length; i++)
            {
                if (videoCapabilities[i].FrameSize.Width == size.Width || videoCapabilities[i].FrameSize.Height == size.Height)
                    return videoCapabilities[i];
            }

            throw new NotSupportedException("Resolution not supported");
        }

        private VideoCapability GetMaxResolution()
        {
            int width = 0;
            int height = 0;
            int biggestResolutionIndex = 0;

            VideoCapability[] videoCapabilities = videoCaptureDevice.VideoCapabilities;

            if (videoCapabilities.Length == 0)
                throw new NotSupportedException(nameof(GetMaxResolution));

            for (int i = 0; i < videoCapabilities.Length; i++)
            {
                if (videoCapabilities[i].FrameSize.Width > width || videoCapabilities[i].FrameSize.Height > height)
                {
                    width = videoCapabilities[i].FrameSize.Width;
                    height = videoCapabilities[i].FrameSize.Height;
                    biggestResolutionIndex = i;
                }
            }

            return videoCapabilities[biggestResolutionIndex];
        }

        public void Start()
        {
            videoCaptureDevice.Start();
            videoCaptureDevice.Grabber.AutoResetEvent.WaitOne();
        }

        public void Capture() => videoCaptureDevice.Grabber.AutoResetEvent.WaitOne();

        public void Stop() => videoCaptureDevice.Stop();

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(UVCDevice));
            (Properties as ICanSaveLoadSettings)?.SaveSettings(settingsCollection);
            settingsCollection.KeyCreator.RemoveLast();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(UVCDevice));
            (Properties as ICanSaveLoadSettings)?.LoadSettings(settingsCollection);
            settingsCollection.KeyCreator.RemoveLast();
        }
    }
}
