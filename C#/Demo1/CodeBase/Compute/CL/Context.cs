using Compute.CL.Internals;
using System;


namespace Compute.CL
{
    public class Context : ErrorHandler
    {
        public Device Device { get; }


        public Context(Device device)
        {
            Device = device;

            IntPtr[] ctxProperties = new IntPtr[3];
            ctxProperties[0] = new IntPtr((int)CLContextProperties.Platform);
            ctxProperties[1] = device.Platform.Handle;
            ctxProperties[2] = IntPtr.Zero;

            Handle = clCreateContext(ctxProperties, (uint)1, new IntPtr[] { device.Handle }, null, IntPtr.Zero, out CLError err);
            ThrowExceptionOnError(err);
        }

        protected override void FreeUnmanagedResource() => ThrowExceptionOnError(clReleaseContext(Handle));
    }
}
