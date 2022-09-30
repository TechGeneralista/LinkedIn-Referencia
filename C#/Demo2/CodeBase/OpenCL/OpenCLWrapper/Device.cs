using OpenCLWrapper.Internals;
using System;
using System.Runtime.InteropServices;


namespace OpenCLWrapper
{
    public class Device : ErrorHandler
    {
        public Platform Platform { get; }
        public CLDeviceType Type { get; }
        public string Name { get; }


        public Device(Platform platform, IntPtr handle)
        {
            Platform = platform;
            Handle = handle;

            Type = (CLDeviceType)GetDeviceInfo(CLDeviceInfo.Type);
            Name = ((string)GetDeviceInfo(CLDeviceInfo.Name)).Replace("\0", string.Empty);
        }

        private object GetDeviceInfo(CLDeviceInfo info)
        {
            object result = null;

            ThrowExceptionOnError(clGetDeviceInfo(Handle, info, 0, IntPtr.Zero, out uint param_value_size_ret));

            if ((int)param_value_size_ret == 0)
                return result;

            IntPtr ptr = Marshal.AllocHGlobal(new IntPtr(param_value_size_ret));

            try
            {
                clGetDeviceInfo(Handle, info,
                param_value_size_ret, ptr, out param_value_size_ret);

                switch (info)
                {
                    case CLDeviceInfo.Type:
                        result = (CLDeviceType)Marshal.ReadInt64(ptr);
                        break;
                    case CLDeviceInfo.VendorID:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.MaxComputeUnits:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.MaxWorkItemDimensions:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.MaxWorkGroupSize:
                        result = Marshal.PtrToStructure(ptr, typeof(IntPtr));
                        break;
                    case CLDeviceInfo.MaxWorkItemSizes:
                        uint dims = (uint)GetDeviceInfo(CLDeviceInfo.MaxWorkItemDimensions);
                        IntPtr[] sizes = new IntPtr[dims];
                        for (int i = 0; i < dims; i++)
                        {
                            sizes[i] = new IntPtr(Marshal.ReadIntPtr(ptr, i * IntPtr.Size).ToInt64());
                        }

                        result = sizes;
                        break;
                    case CLDeviceInfo.PreferredVectorWidthChar:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.PreferredVectorWidthShort:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.PreferredVectorWidthInt:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.PreferredVectorWidthLong:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.PreferredVectorWidthFloat:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.PreferredVectorWidthDouble:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.MaxClockFrequency:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.AddressBits:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.MaxReadImageArgs:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.MaxWriteImageArgs:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.MaxMemAllocSize:
                        result = (ulong)Marshal.ReadInt64(ptr);
                        break;
                    case CLDeviceInfo.Image2DMaxWidth:
                        result = Marshal.PtrToStructure(ptr, typeof(IntPtr));
                        break;
                    case CLDeviceInfo.Image2DMaxHeight:
                        result = Marshal.PtrToStructure(ptr, typeof(IntPtr));
                        break;
                    case CLDeviceInfo.Image3DMaxWidth:
                        result = Marshal.PtrToStructure(ptr, typeof(IntPtr));
                        break;
                    case CLDeviceInfo.Image3DMaxHeight:
                        result = Marshal.PtrToStructure(ptr, typeof(IntPtr));
                        break;
                    case CLDeviceInfo.Image3DMaxDepth:
                        result = Marshal.PtrToStructure(ptr, typeof(IntPtr));
                        break;
                    case CLDeviceInfo.ImageSupport:
                        result = (CLBool)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.MaxParameterSize:
                        result = Marshal.PtrToStructure(ptr, typeof(IntPtr));
                        break;
                    case CLDeviceInfo.MaxSamplers:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.MemBaseAddrAlign:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.MinDataTypeAlignSize:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.SingleFPConfig:
                        result = (CLDeviceFPConfig)Marshal.ReadInt64(ptr);
                        break;
                    case CLDeviceInfo.GlobalMemCacheType:
                        result = (CLDeviceMemCacheType)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.GlobalMemCacheLineSize:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.GlobalMemCacheSize:
                        result = (ulong)Marshal.ReadInt64(ptr);
                        break;
                    case CLDeviceInfo.GlobalMemSize:
                        result = (ulong)Marshal.ReadInt64(ptr);
                        break;
                    case CLDeviceInfo.MaxConstantBufferSize:
                        result = (ulong)Marshal.ReadInt64(ptr);
                        break;
                    case CLDeviceInfo.MaxConstantArgs:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.LocalMemType:
                        result = (CLDeviceLocalMemType)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.LocalMemSize:
                        result = (ulong)Marshal.ReadInt64(ptr);
                        break;
                    case CLDeviceInfo.ErrorCorrectionSupport:
                        result = (CLBool)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.ProfilingTimerResolution:
                        result = Marshal.PtrToStructure(ptr, typeof(IntPtr));
                        break;
                    case CLDeviceInfo.EndianLittle:
                        result = (CLBool)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.Available:
                        result = (CLBool)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.CompilerAvailable:
                        result = (CLBool)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.ExecutionCapabilities:
                        result = (CLDeviceExecCapabilities)Marshal.ReadInt64(ptr);
                        break;
                    case CLDeviceInfo.QueueProperties:
                        result = (CLCommandQueueProperties)Marshal.ReadInt64(ptr);
                        break;
                    case CLDeviceInfo.Name:
                        result = Marshal.PtrToStringAnsi(ptr, (int)param_value_size_ret);
                        break;
                    case CLDeviceInfo.Vendor:
                        result = Marshal.PtrToStringAnsi(ptr, (int)param_value_size_ret);
                        break;
                    case CLDeviceInfo.DriverVersion:
                        result = Marshal.PtrToStringAnsi(ptr, (int)param_value_size_ret);
                        break;
                    case CLDeviceInfo.Profile:
                        result = Marshal.PtrToStringAnsi(ptr, (int)param_value_size_ret);
                        break;
                    case CLDeviceInfo.Version:
                        result = Marshal.PtrToStringAnsi(ptr, (int)param_value_size_ret);
                        break;
                    case CLDeviceInfo.Extensions:
                        result = Marshal.PtrToStringAnsi(ptr, (int)param_value_size_ret);
                        break;
                    case CLDeviceInfo.Platform:
                        result = Marshal.PtrToStructure(ptr, typeof(IntPtr));
                        break;
                    case CLDeviceInfo.PreferredVectorWidthHalf:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.HostUnifiedMemory:
                        result = (CLBool)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.NativeVectorWidthChar:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.NativeVectorWidthShort:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.NativeVectorWidthInt:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.NativeVectorWidthLong:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.NativeVectorWidthFloat:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.NativeVectorWidthDouble:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.NativeVectorWidthHalf:
                        result = (uint)Marshal.ReadInt32(ptr);
                        break;
                    case CLDeviceInfo.OpenCLCVersion:
                        result = Marshal.PtrToStringAnsi(ptr, (int)param_value_size_ret);
                        break;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} - {2}", Type, Name, Platform.Version);
        }
    }
}