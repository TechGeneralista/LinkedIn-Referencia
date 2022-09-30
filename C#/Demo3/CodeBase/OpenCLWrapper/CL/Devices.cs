using OpenCLWrapper.CL.Internals;
using OpenCLWrapper.CL.Internals.Exceptions;
using System;
using System.Collections.Generic;


namespace OpenCLWrapper.CL
{
    public class Devices : ErrorHandler
    {
        public IEnumerable<Device> List { get; private set; }


        readonly IEnumerable<Platform> platforms;


        public Devices(IEnumerable<Platform> platforms)
        {
            this.platforms = platforms;
        }

        public void Scan()
        {
            List<Device> list = new List<Device>();

            foreach(Platform platform in platforms)
            {
                IntPtr[] deviceIds = GetDevices(platform, CLDeviceType.All);

                foreach (IntPtr ptr in deviceIds)
                    list.Add(new Device(platform, ptr));
            }

            List = list;
        }

        public Device GetDevice(DeviceTypePriority priority)
        {
            switch(priority)
            {
                case DeviceTypePriority.GPUCPU:
                    foreach (Device device in List)
                    {
                        if (device.Type == CLDeviceType.GPU)
                            return device;
                    }

                    foreach (Device device in List)
                    {
                        if (device.Type == CLDeviceType.CPU)
                            return device;
                    }

                    throw new OpenCLDeviceNotFoundException(priority);

                case DeviceTypePriority.GPU:
                    foreach (Device device in List)
                    {
                        if (device.Type == CLDeviceType.GPU)
                            return device;
                    }

                    throw new OpenCLDeviceNotFoundException(priority);

                case DeviceTypePriority.CPU:
                    foreach (Device device in List)
                    {
                        if (device.Type == CLDeviceType.CPU)
                            return device;
                    }

                    throw new OpenCLDeviceNotFoundException(priority);

                default:
                    throw new IndexOutOfRangeException(string.Format("Priority not exist: {0}", priority));
            }
        }

        private IntPtr[] GetDevices(Platform platform, CLDeviceType devType)
        {
            ThrowExceptionOnError(clGetDeviceIDs(platform.Handle, devType, 0, null, out uint num_devices));

            if (num_devices == 0)
                return new IntPtr[0];

            IntPtr[] devices = new IntPtr[num_devices];
            clGetDeviceIDs(platform.Handle, devType, num_devices, devices, out num_devices);

            return devices;
        }
    }
}
