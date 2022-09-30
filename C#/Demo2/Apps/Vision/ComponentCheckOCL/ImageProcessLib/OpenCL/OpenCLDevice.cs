using ImageProcessLib.OpenCL.Compute;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;


namespace ImageProcessLib.OpenCL
{
    public class OpenCLDevice
    {
        public ComputePlatform ComputePlatform { get; private set; }
        public ComputeContext ComputeContext { get; private set; }
        public ComputeDevice ComputeDevice { get; private set; }
        public ComputeCommandQueue ComputeCommandQueue { get; private set; }


        public bool Select(ComputeDeviceTypes deviceType)
        {
            ReadOnlyCollection<ComputePlatform> platforms = ComputePlatform.Platforms;

            foreach(ComputePlatform computePlatform in platforms)
            {
                foreach(ComputeDevice computeDevice in computePlatform.Devices)
                {
                    if(computeDevice.Type == deviceType)
                    {
                        try
                        {
                            ComputePlatform = computePlatform;
                            ComputeContext = new ComputeContext(deviceType, new ComputeContextPropertyList(ComputePlatform), null, IntPtr.Zero);
                            ComputeDevice = computeDevice;
                            ComputeCommandQueue = new ComputeCommandQueue(ComputeContext, ComputeDevice, ComputeCommandQueueFlags.None);
                        }
                        catch
                        {
                            if (ComputeContext != null)
                                ComputeContext.Dispose();

                            if (ComputeCommandQueue != null)
                                ComputeCommandQueue.Dispose();
                        }
                    }
                }
            }

            if (ComputePlatform == null || ComputeContext == null || ComputeDevice == null || ComputeCommandQueue == null)
            {
                MessageBox.Show("OpenCL not supported on this platform", "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
