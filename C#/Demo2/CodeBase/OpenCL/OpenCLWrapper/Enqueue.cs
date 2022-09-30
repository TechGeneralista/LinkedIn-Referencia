using Common.Types;
using OpenCLWrapper.Internals;
using System;
using System.Runtime.InteropServices;


namespace OpenCLWrapper
{
    public class Enqueue : ErrorHandler
    {
        public CommandQueue CommandQueue { get; }


        public Enqueue(CommandQueue commandQueue)
        {
            CommandQueue = commandQueue;
        }

        // execute
        public void Execute(Kernel kernel, SizeT[] globalWorkSize) => Execute(kernel, null, globalWorkSize, null);
        public void Execute(Kernel kernel, SizeT[] globalWorkOffset, SizeT[] globalWorkSize) => Execute(kernel, globalWorkOffset, globalWorkSize, null);
        public void Execute(Kernel kernel, SizeT[] globalWorkOffset, SizeT[] globalWorkSize, SizeT[] localWorkSize) => ThrowExceptionOnError(clEnqueueNDRangeKernel(CommandQueue.Handle, kernel.Handle, (uint)globalWorkSize.Length, globalWorkOffset, globalWorkSize, localWorkSize, 0, null, IntPtr.Zero));

        // write
        public void WriteBuffer<T>(Buffer<T> buffer, T[] data)
        {
            GCHandle h = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                ThrowExceptionOnError(clEnqueueWriteBuffer(CommandQueue.Handle, buffer.Handle, CLBool.True, 0, (SizeT)(Marshal.SizeOf(typeof(T)) * data.Length), h.AddrOfPinnedObject(), 0, null, IntPtr.Zero));
            }
            finally
            {
                h.Free();
            }
        }

        public void WriteBuffer<T>(Buffer<T> buffer, IntPtr data, SizeT size) => ThrowExceptionOnError(clEnqueueWriteBuffer(CommandQueue.Handle, buffer.Handle, CLBool.True, 0, (SizeT)(Marshal.SizeOf(typeof(T)) * size), data, 0, null, IntPtr.Zero));
        public void WriteBuffer(BufferImage2D buffer, IntPtr data) => ThrowExceptionOnError(clEnqueueWriteImage(CommandQueue.Handle, buffer.Handle, CLBool.True, new SizeT[] { 0, 0, 0 }, new SizeT[] { buffer.Width, buffer.Height, 1 }, 0, 0, data, 0, null, IntPtr.Zero));
        public void WriteBuffer(BufferImage buffer, IntPtr data) => ThrowExceptionOnError(clEnqueueWriteImage(CommandQueue.Handle, buffer.Handle, CLBool.True, new SizeT[] { 0, 0, 0 }, new SizeT[] { buffer.Descriptor.Width, buffer.Descriptor.Height, 1 }, 0, 0, data, 0, null, IntPtr.Zero));

        // read
        public void ReadBuffer<T>(Buffer<T> buffer, T[] data)
        {
            GCHandle h = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                ThrowExceptionOnError(clEnqueueReadBuffer(CommandQueue.Handle, buffer.Handle, CLBool.True, 0, (uint)(Marshal.SizeOf(typeof(T)) * data.Length), h.AddrOfPinnedObject(), 0, null, IntPtr.Zero));
            }
            finally
            {
                h.Free();
            }
        }

        public void Copy(BufferImage source, BufferImage destination) => ThrowExceptionOnError(clEnqueueCopyImage(CommandQueue.Handle, source.Handle, destination.Handle, new SizeT[] { 0, 0, 0}, new SizeT[] { 0, 0, 0 }, new SizeT[] { source.Descriptor.Width, source.Descriptor.Height , 1}, 0, null, IntPtr.Zero));
        public void ReadBuffer<T>(Buffer<T> buffer, IntPtr data, SizeT size) => ThrowExceptionOnError(clEnqueueReadBuffer(CommandQueue.Handle, buffer.Handle, CLBool.True, 0, (SizeT)(Marshal.SizeOf(typeof(T)) * size), data, 0, null, IntPtr.Zero));
        public void ReadBuffer(BufferImage2D buffer, IntPtr data) => ThrowExceptionOnError(clEnqueueReadImage(CommandQueue.Handle, buffer.Handle, CLBool.True, new SizeT[] { 0, 0, 0 }, new SizeT[] { buffer.Width, buffer.Height, 1 }, 0, 0, data, 0, null, IntPtr.Zero));
        public void ReadBuffer(BufferImage buffer, IntPtr data) => ThrowExceptionOnError(clEnqueueReadImage(CommandQueue.Handle, buffer.Handle, CLBool.True, new SizeT[] { 0, 0, 0 }, new SizeT[] { buffer.Descriptor.Width, buffer.Descriptor.Height, 1 }, 0, 0, data, 0, null, IntPtr.Zero));
    }
}
