using Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ImageCaptureDevice.UniversalVideoClass.DirectShow;
using Compute;
using ImageCaptureDevice;
using ImageCaptureDevice.UniversalVideoClass;
using Common.Generics;


namespace ImageProcess.ImageCaptureDevice.UniversalVideoClass
{
    public class UVCScannerDC : DCBase
    {
        public bool ViewIsEnabled
        {
            get => viewIsEnabled;
            private set => SetField(ref viewIsEnabled, value);
        }
        bool viewIsEnabled = true;

        public ThreadSafeObservableCollection<UVCPreviewDC> Previews { get; private set; }

        public UVCPreviewDC SelectedPreview
        {
            get => selectedPreview;
            set => SetField(ref selectedPreview, value);
        }
        UVCPreviewDC selectedPreview;


        readonly ComputeAccelerator computeAccelerator;
        readonly IEnumerable<IImageCaptureDevice> inUseDevices;
        UVCScannerV universalVideoDeviceSelectorV;
        bool autoStartIsEnabled;
        IImageCaptureDevice selectedImageCaptureDeviceReturn;


        public UVCScannerDC(ComputeAccelerator computeAccelerator)
        {
            this.computeAccelerator = computeAccelerator;

            Utils.InvokeIfNecessary(() =>
            {
                Previews = new ThreadSafeObservableCollection<UVCPreviewDC>();
            });
        }

        public UVCScannerDC(ComputeAccelerator computeAccelerator, IEnumerable<IImageCaptureDevice> inUseDevices) : this(computeAccelerator)
        {
            this.inUseDevices = inUseDevices;
        }

        public IImageCaptureDevice ShowDialog(Window parent)
        {
            autoStartIsEnabled = true;

            universalVideoDeviceSelectorV = new UVCScannerV();
            universalVideoDeviceSelectorV.DataContext = this;
            universalVideoDeviceSelectorV.Owner = parent;
            universalVideoDeviceSelectorV.Closing += UniversalVideoDeviceSelectorV_Closing;
            universalVideoDeviceSelectorV.ShowDialog();

            return selectedImageCaptureDeviceReturn;
        }

        private void UniversalVideoDeviceSelectorV_Closing(object sender, CancelEventArgs e)
        {
            foreach (UVCPreviewDC uvcPreviewDC in Previews)
            {
                IImageCaptureDevice imageCaptureDevice = uvcPreviewDC.ImageCaptureDevice;
                uvcPreviewDC.ImageCaptureDevice = null;

                if (selectedImageCaptureDeviceReturn == imageCaptureDevice)
                    continue;
                else
                    imageCaptureDevice.Stop();
            }
        }

        public Task ScanAsync()
            => Task.Run(() => Scan());

        public void Scan()
        {
            ViewIsEnabled = false;

            foreach (UVCPreviewDC uvcPreviewDC in Previews)
                uvcPreviewDC.ImageCaptureDevice.Stop();

            Previews.Clear();
            selectedImageCaptureDeviceReturn = null;

            try
            {
                FilterInfoCollection filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                foreach (FilterInfo filterInfo in filterInfoCollection)
                {
                    if (inUseDevices != null && inUseDevices.Where(x => x.Guid == filterInfo.Guid).FirstOrDefault() != null)
                        continue;

                    try
                    {
                        IImageCaptureDevice imageCaptureDevice = new UniversalVideoClassDC(filterInfo);
                        UVCPreviewDC uvcPreviewDC = new UVCPreviewDC(computeAccelerator);
                        uvcPreviewDC.ImageCaptureDevice = imageCaptureDevice;
                        Previews.Add(uvcPreviewDC);

                        if (autoStartIsEnabled)
                            imageCaptureDevice.Start();
                    }
                    catch { }
                }
            }
            catch { }

            if (Previews.Count != 0)
                SelectedPreview = Previews[0];

            ViewIsEnabled = true;
        }

        public void Select()
        {
            IImageCaptureDevice selectedImageCaptureDevice = selectedPreview.ImageCaptureDevice;
            selectedImageCaptureDeviceReturn = selectedImageCaptureDevice;
            universalVideoDeviceSelectorV.Close();
        }

        public IImageCaptureDevice SelectDevice(string guid, string frameSizeString)
        {
            autoStartIsEnabled = false;
            Scan();

            if(Previews.Count != 0 &&
               Previews.Where(p => p.ImageCaptureDevice.Guid == guid).FirstOrDefault() is UVCPreviewDC uvcPreviewDC)
            {
                IImageCaptureDevice imageCaptureDevice = uvcPreviewDC.ImageCaptureDevice;

                if(imageCaptureDevice.AvailableResolutions.Where(ar => ar.FrameSizeString == frameSizeString).FirstOrDefault() is IImageCaptureDeviceCapability imageCaptureDeviceCapability)
                {
                    imageCaptureDevice.SelectedResolution = imageCaptureDeviceCapability;
                    imageCaptureDevice.Start();
                    return imageCaptureDevice;
                }
            }

            return null;
        }
    }
}
