using System;


namespace OpenCLWrapper.Internals.Exceptions
{
    public class OpenCLDeviceNotFoundException : Exception
    {
        public DeviceTypePriority Priority { get; }

        public OpenCLDeviceNotFoundException(DeviceTypePriority priority)
        {
            Priority = priority;
        }

        public override string Message
        {
            get
            {
                return string.Format("OpenCL supported device not found {0}", Priority);
            }
        }
    }
}
