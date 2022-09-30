using System;


namespace Compute.CL.Internals.Exceptions
{
    public class OpenCLDeviceNotFoundException : Exception
    {
        public ComputeDeviceTypePriority Priority { get; }

        public OpenCLDeviceNotFoundException(ComputeDeviceTypePriority priority)
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
