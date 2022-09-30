using UVCLib.DirectShow;
using System.Collections.Generic;
using CommonLib.Interfaces;
using System.Threading.Tasks;
using System;
using CommonLib.Components;


namespace UVCLib
{
    public class UVCDeviceScanner : ObservableProperty
    {
        public event Action AvailableDevicesChanged;

        public IVideoCaptureDevice[] AvailableDevices 
        {
            get => availableDevices;
            private set { SetField(value, ref availableDevices); AvailableDevicesChanged?.Invoke(); }
        }
        
        IVideoCaptureDevice[] availableDevices;


        public Task ScanAsync() => Task.Run(() => Scan());

        public void Scan()
        {
            List<IVideoCaptureDevice> devices = new List<IVideoCaptureDevice>();
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
                    catch
                    {
                        continue;
                    }
                }
            }

            AvailableDevices = devices.ToArray();
        }
    }
}
