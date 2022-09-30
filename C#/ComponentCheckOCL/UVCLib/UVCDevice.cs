using System;
using UVCLib.DirectShow;
using CommonLib.Interfaces;
using System.Windows.Media.Imaging;
using System.Threading;



namespace UVCLib
{
    public class UVCDevice : IVideoCaptureDevice
    {
        public event Action<WriteableBitmap> NewImageEvent
        {
            add { videoCaptureDevice.Grabber.NewImageEvent += value; }
            remove { videoCaptureDevice.Grabber.NewImageEvent -= value; }
        }

        public event Action ImagePropertiesChangedEvent;


        public string Name { get; }
        public string Resolution { get; }

        public int Brightness
        {
            get => videoCaptureDevice.PropertiesModel.Brightness.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Brightness.CurrentValue = value;
        }

        public int Contrast
        {
            get => videoCaptureDevice.PropertiesModel.Contrast.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Contrast.CurrentValue = value;
        }

        public int Hue
        {
            get => videoCaptureDevice.PropertiesModel.Hue.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Hue.CurrentValue = value;
        }

        public int Saturation
        {
            get => videoCaptureDevice.PropertiesModel.Saturation.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Saturation.CurrentValue = value;
        }

        public int Sharpness
        {
            get => videoCaptureDevice.PropertiesModel.Sharpness.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Sharpness.CurrentValue = value;
        }

        public int Gamma
        {
            get => videoCaptureDevice.PropertiesModel.Gamma.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Gamma.CurrentValue = value;
        }

        public int ColorEnable
        {
            get => videoCaptureDevice.PropertiesModel.ColorEnable.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.ColorEnable.CurrentValue = value;
        }

        public int WhiteBalance
        {
            get => videoCaptureDevice.PropertiesModel.WhiteBalance.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.WhiteBalance.CurrentValue = value;
        }

        public int BacklightCompensation
        {
            get => videoCaptureDevice.PropertiesModel.BacklightCompensation.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.BacklightCompensation.CurrentValue = value;
        }

        public int Gain
        {
            get => videoCaptureDevice.PropertiesModel.Gain.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Gain.CurrentValue = value;
        }

        public int Pan
        {
            get => videoCaptureDevice.PropertiesModel.Pan.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Pan.CurrentValue = value;
        }

        public int Tilt
        {
            get => videoCaptureDevice.PropertiesModel.Tilt.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Tilt.CurrentValue = value;
        }

        public int Roll
        {
            get => videoCaptureDevice.PropertiesModel.Roll.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Roll.CurrentValue = value;
        }

        public int Zoom
        {
            get => videoCaptureDevice.PropertiesModel.Zoom.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Zoom.CurrentValue = value;
        }

        public int Exposure
        {
            get => videoCaptureDevice.PropertiesModel.Exposure.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Exposure.CurrentValue = value;
        }

        public int Iris
        {
            get => videoCaptureDevice.PropertiesModel.Iris.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Iris.CurrentValue = value;
        }

        public int Focus
        {
            get => videoCaptureDevice.PropertiesModel.Focus.CurrentValue;
            set => videoCaptureDevice.PropertiesModel.Focus.CurrentValue = value;
        }

        public bool IsRunning => videoCaptureDevice.IsRunning;


        VideoCaptureDevice videoCaptureDevice;


        public UVCDevice(FilterInfo filterInfo)
        {
            Name = filterInfo.Name;
            videoCaptureDevice = new VideoCaptureDevice(filterInfo.MonikerString);
            VideoCapability selectedResolution = GetMaxResolution();
            videoCaptureDevice.VideoResolution = selectedResolution;
            Resolution = selectedResolution.FrameSize.Width + "x" + selectedResolution.FrameSize.Height;
        }

        public void Connect() => videoCaptureDevice.Start();

        public void Capture()
        {
            videoCaptureDevice.Grabber.MakeSnapshot = true;

            while(videoCaptureDevice.Grabber.MakeSnapshot)
            {
                Thread.Sleep(10);
            }
        }

        public void Disconnect() => videoCaptureDevice.Stop();

        private VideoCapability GetMaxResolution()
        {
            int width = 0;
            int heigt = 0;
            int biggestResolutionIndex = 0;

            VideoCapability[] videoCapabilities = videoCaptureDevice.VideoCapabilities;

            if (videoCapabilities.Length == 0)
                throw new NotSupportedException(nameof(GetMaxResolution));

            for(int i=0;i< videoCapabilities.Length;i++)
            {
                if (videoCapabilities[i].FrameSize.Width > width || videoCapabilities[i].FrameSize.Height > heigt)
                {
                    width = videoCapabilities[i].FrameSize.Width;
                    heigt = videoCapabilities[i].FrameSize.Height;
                    biggestResolutionIndex = i;
                }
            }

            return videoCapabilities[biggestResolutionIndex];
        }

        public void ShowImageProperties()
        {
            videoCaptureDevice.ShowPropertiesWindow();
            ImagePropertiesChangedEvent?.Invoke();
        }
    }
}
