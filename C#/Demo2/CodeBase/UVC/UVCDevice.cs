using System;
using UVC.DirectShow;
using System.Threading.Tasks;
using System.Threading;
using UVC.Internals;


namespace UVC
{
    public class UVCDevice : IImageSource
    {
        public string Name { get; }
        public string Resolution { get; }
        public string FrameRate { get; }
        public BitmapDataDTO Frame => videoCaptureDevice.Grabber.Frame;
        public bool IsRunning => videoCaptureDevice.IsRunning;
        public DevicePropertiesDC Properties => videoCaptureDevice.DevicePropertiesViewModel;


        VideoCaptureDevice videoCaptureDevice;


        public UVCDevice(FilterInfo filterInfo)
        {
            Name = filterInfo.Name;
            videoCaptureDevice = new VideoCaptureDevice(filterInfo.MonikerString);
            VideoCapability selectedResolution = GetMaxResolution();
            //VideoCapability selectedResolution = GetHdResolution();
            videoCaptureDevice.VideoResolution = selectedResolution;
            Resolution = selectedResolution.FrameSize.Width + "x" + selectedResolution.FrameSize.Height;
            FrameRate = selectedResolution.AverageFrameRate.ToString();
        }

        private VideoCapability GetHdResolution()
        {
            VideoCapability[] videoCapabilities = videoCaptureDevice.VideoCapabilities;

            if (videoCapabilities.Length == 0)
                throw new NotSupportedException(nameof(GetHdResolution));

            for (int i = 0; i < videoCapabilities.Length; i++)
            {
                if (videoCapabilities[i].FrameSize.Width == 1280 || videoCapabilities[i].FrameSize.Height == 720)
                    return videoCapabilities[i];
            }

            throw new NotSupportedException("HdResolution not supported");
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

        public Task StartAsync() => Task.Run(() => Start());

        public void Start() => videoCaptureDevice.Start();

        public Task StopAsync() => Task.Run(() => Stop());

        public void Stop() => videoCaptureDevice.Stop();

        public void Capture()
        {
            Interlocked.Exchange(ref videoCaptureDevice.Grabber.MakeSnapshoot, 1);

            while(videoCaptureDevice.Grabber.MakeSnapshoot == 1)
                Thread.Sleep(1);
        }
    }
}
