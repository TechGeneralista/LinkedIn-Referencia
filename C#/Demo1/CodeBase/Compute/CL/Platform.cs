using Compute.CL.Internals;
using System;
using System.Runtime.InteropServices;


namespace Compute.CL
{
    public class Platform : ErrorHandler
    {
        public string Version { get; }
        public double VersionNumber { get; }


        public Platform(IntPtr handle)
        {
            Handle = handle;
            Version = ((string)GetPlatformInfo(CLPlatformInfo.Version)).Replace("\0", string.Empty);
            string versionString = Version.Replace("OpenCL ", string.Empty).Substring(0, 3).Replace('.', ',');
            double version = 0;

            if(double.TryParse(versionString, out version))
                VersionNumber = version;
            else
            {
                double.TryParse(versionString.Replace(',','.'), out version);
                VersionNumber = version;
            }
        }

        private object GetPlatformInfo(CLPlatformInfo info)
        {
            IntPtr value = IntPtr.Zero;
            object result = null;

            ThrowExceptionOnError(clGetPlatformInfo(Handle, info, 0, value, out uint size));

            if (size == 0)
                return result;

            value = Marshal.AllocHGlobal(new IntPtr(size));

            try
            {
                clGetPlatformInfo(Handle, info, size, value, out size);

                switch (info)
                {
                    case CLPlatformInfo.Profile:
                        result = Marshal.PtrToStringAnsi(value, (int)size);
                        break;
                    case CLPlatformInfo.Version:
                        result = Marshal.PtrToStringAnsi(value, (int)size);
                        break;
                    case CLPlatformInfo.Name:
                        result = Marshal.PtrToStringAnsi(value, (int)size);
                        break;
                    case CLPlatformInfo.Vendor:
                        result = Marshal.PtrToStringAnsi(value, (int)size);
                        break;
                    case CLPlatformInfo.Extensions:
                        result = Marshal.PtrToStringAnsi(value, (int)size);
                        break;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(value);
            }

            return result;
        }

        public override string ToString() => Version;
    }
}