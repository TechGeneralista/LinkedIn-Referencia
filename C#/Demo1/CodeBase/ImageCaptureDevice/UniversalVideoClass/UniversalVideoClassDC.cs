using Common;
using System.Linq;
using ImageCaptureDevice.UniversalVideoClass.DirectShow;
using System;


namespace ImageCaptureDevice.UniversalVideoClass
{
    public class UniversalVideoClassDC : DCBase, IImageCaptureDevice
    {
        public event EventHandler<NewImageAvailableArgs> NewImageAvailable;
        public event EventHandler SelectedResolutionChanged;

        public string Guid => filterInfo.Guid;
        public string Name => filterInfo.Name;
        public IImageCaptureDeviceCapability[] AvailableResolutions { get; }

        public IImageCaptureDeviceCapability SelectedResolution
        {
            get => selectedResolution;
            set
            {
                bool isRunBackup = videoCaptureDevice.IsRun;

                if (isRunBackup)
                    videoCaptureDevice.Stop();

                SetField(ref selectedResolution, value);
                SelectedResolutionChanged?.Invoke(this, EventArgs.Empty);

                if (isRunBackup)
                    videoCaptureDevice.Start(selectedResolution);
            }
        }
        IImageCaptureDeviceCapability selectedResolution;


        readonly FilterInfo filterInfo;
        protected readonly VideoCaptureDevice videoCaptureDevice;


        public UniversalVideoClassDC(FilterInfo filterInfo)
        {
            this.filterInfo = filterInfo;

            videoCaptureDevice = new VideoCaptureDevice(filterInfo.MonikerString);
            videoCaptureDevice.NewImageAvailable += (s, e) => NewImageAvailable?.Invoke(this, e);
            AvailableResolutions = videoCaptureDevice.VideoCapabilities;
            SetSelectedResolutionToMax();
        }

        protected void NewFrameAvailable(int width, int height, IntPtr buffer, int bufferLength)
        {
            NewImageAvailable?.Invoke(this, new NewImageAvailableArgs(width, height, buffer, bufferLength));
        }

        private void SetSelectedResolutionToMax()
        {
            IImageCaptureDeviceCapability videoCapability0 = AvailableResolutions.First();

            foreach (IImageCaptureDeviceCapability videoCapability1 in AvailableResolutions)
            {
                if (videoCapability1.PixelsCount > videoCapability0.PixelsCount)
                    videoCapability0 = videoCapability1;
            }

            SelectedResolution = videoCapability0;
        }

        public void Start()
            => videoCaptureDevice.Start(selectedResolution);

        public void Stop()
            => videoCaptureDevice.Stop();
    }
}
