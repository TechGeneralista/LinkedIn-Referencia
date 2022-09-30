using OpenCLWrapper.Internals;
using System;


namespace OpenCLWrapper
{
    public class BufferImage2D : Resource
    {
        public Context Context { get; }
        public CLMemFlags MemFlags { get; }
        public CLImageFormat Format { get; }
        public int Width { get; }
        public int Height { get; }
        public int Stride { get; }
        public int BytePerPixel { get; }


        public BufferImage2D(Context context, CLMemFlags memFlags, CLImageFormat format, int width, int height, int stride)
        {
            Context = context;
            MemFlags = memFlags;
            Format = format;
            Width = width;
            Height = height;
            Stride = stride;
            BytePerPixel = stride / width;

            Handle = clCreateImage2D(context.Handle, memFlags, ref format, width, height, 0, IntPtr.Zero, out CLError err);
            ThrowExceptionOnError(err);
        }

        protected override void FreeUnmanagedResource() => ThrowExceptionOnError(clReleaseMemObject(Handle));
    }
}
