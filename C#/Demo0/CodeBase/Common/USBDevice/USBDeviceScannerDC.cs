using Common.NotifyProperty;
using System;
using System.Collections.Generic;
using System.Management;


namespace Common.USBDevice
{
    public class USBDeviceScannerDC
    {
        public INonSettableObservablePropertyArray<USBDeviceInfo> Devices { get; } = new ObservablePropertyArray<USBDeviceInfo>();


        public void Scan()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub");
            ManagementObjectCollection collection = searcher.Get();

            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            foreach (ManagementObject device in collection)
                devices.Add(new USBDeviceInfo(device));

            collection.Dispose();
            searcher.Dispose();

            Devices.ForceAddRange(devices.ToArray());
        }
    }
}
