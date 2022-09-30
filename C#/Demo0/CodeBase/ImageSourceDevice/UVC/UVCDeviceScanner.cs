using ImageSourceDevice.UVC.DirectShow;
using Language;
using System.Collections.Generic;


namespace ImageSourceDevice.UVC
{
    public class UVCDeviceScanner : IImageSourceDeviceScanner
    {
        public IImageSourceDevice[] AvailableDevices { get; private set; }


        readonly LanguageDC languageDC;


        public UVCDeviceScanner(LanguageDC languageDC)
        {
            this.languageDC = languageDC;
        }

        public void Scan()
        {
            AvailableDevices = new IImageSourceDevice[0];

            List<IImageSourceDevice> devices = new List<IImageSourceDevice>();
            FilterInfoCollection filterInfoCollection = null;

            try
            {
                filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            }
            catch { }

            if(filterInfoCollection != null)
            {
                foreach (FilterInfo filterInfo in filterInfoCollection)
                {
                    try
                    {
                        devices.Add(new UVCDevice(languageDC, filterInfo));
                    }
                    catch { }
                }
            }

            AvailableDevices = devices.ToArray();
        }
    }
}
