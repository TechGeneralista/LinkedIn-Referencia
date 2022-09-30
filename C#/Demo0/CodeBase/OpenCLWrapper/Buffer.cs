using OpenCLWrapper.Internals;
using System;
using System.Runtime.InteropServices;


namespace OpenCLWrapper
{
    public class Buffer<T> : Resource
    {
        public Context Context { get; }
        public CLMemFlags MemFlags { get; }
        public int Size { get; }


        public Buffer(Context context, CLMemFlags memFlags, int size)
        {
            Context = context;
            MemFlags = memFlags;
            Size = size;

            Handle = clCreateBuffer(context.Handle, memFlags, Marshal.SizeOf(typeof(T)) * size, IntPtr.Zero, out CLError err);
            ThrowExceptionOnError(err);
        }

        protected override void FreeUnmanagedResource() => ThrowExceptionOnError(clReleaseMemObject(Handle));
    }
}
