using UVC.DirectShow;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.NotifyProperty;


namespace UVC
{
    public class UVCDeviceScanner
    {
        public INonSettableObservableProperty<IImageSource[]> AvailableDevices { get; } = new ObservableProperty<IImageSource[]>();


        public Task ScanAsync() => Task.Run(() => Scan());

        public void Scan()
        {
            ((ISettableObservableProperty<IImageSource[]>)AvailableDevices).Value = null;

            List<IImageSource> devices = new List<IImageSource>();
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
                        devices.Add(new UVCDevice(filterInfo));
                    }
                    catch { }
                }
            }

            ((ISettableObservableProperty<IImageSource[]>)AvailableDevices).Value = devices.ToArray();
        }
    }
}
