using OpenCLWrapper.Internals;
using System;


namespace OpenCLWrapper
{
    public class BufferImage : Resource
    {
        public Context Context { get; }
        public CLMemFlags MemFlags { get; }
        public CLImageFormat Format { get; }
        public CLImageDesc Descriptor { get; }





        public BufferImage(Context context, CLMemFlags flags, CLImageFormat format, CLImageDesc desc)
        {
            Context = context;
            MemFlags = flags;
            Format = format;
            Descriptor = desc;

            Handle = clCreateImage(context.Handle, flags, ref format, ref desc, IntPtr.Zero, out CLError err);
            ThrowExceptionOnError(err);
        }

        protected override void FreeUnmanagedResource() => ThrowExceptionOnError(clReleaseMemObject(Handle));
    }
}
