using Compute.CL.Internals;
using System;
using System.Runtime.InteropServices;


namespace Compute.CL
{
    public class Buffer<T> : ErrorHandler
    {
        public CLMemFlags MemFlags { get; private set; }
        public int Size { get; private set; }


        private readonly Context context;


        protected Buffer(ComputeAccelerator computeAccelerator)
        {
            context = computeAccelerator.Context;
        }

        public void CreateIfNeed(CLMemFlags memFlags, int size)
        {
            if (HandleIsValid && (MemFlags != memFlags || Size != size))
            {
                FreeUnmanagedResource();
            }

            if (!HandleIsValid)
            {
                MemFlags = memFlags;
                Size = size;

                Handle = clCreateBuffer(context.Handle, memFlags, Marshal.SizeOf(typeof(T)) * size, IntPtr.Zero, out CLError err);
                ThrowExceptionOnError(err);
            }
        }

        protected override void FreeUnmanagedResource()
        {
            if (HandleIsValid)
            {
                ThrowExceptionOnError(clReleaseMemObject(Handle));
                Handle = IntPtr.Zero;
                Size = 0;
            }
        }
    }
}
