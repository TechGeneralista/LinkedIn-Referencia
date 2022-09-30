using Common;
using OpenCLWrapper.CL;
using OpenCLWrapper.CL.Internals;
using System;
using System.Windows.Media.Imaging;


namespace OpenCLWrapper.Buffer
{
    public class DataBuffer<T> : IDisposable
    {
        public Buffer<T> Buffer { get; private set; }


        readonly OpenCLAccelerator accelerator;


        public DataBuffer(OpenCLAccelerator accelerator) => this.accelerator = accelerator;


        public void Create(int size)
        {
            if(Buffer.IsNull() || Buffer.Size != size)
            {
                Buffer?.Dispose();
                Buffer = new Buffer<T>(accelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, size);
            }
        }

        public void Download(T[] source)
        {
            Create(source.Length);
            accelerator.Enqueue.WriteBuffer(Buffer, source);
        }

        public void Download(IntPtr buffer, int bufferLength)
        {
            Create(bufferLength);
            accelerator.Enqueue.WriteBuffer(Buffer, buffer, bufferLength);
        }

        public void Download(WriteableBitmap source)
        {
            int bufferLength = source.PixelHeight * source.BackBufferStride;
            Create(bufferLength);
            accelerator.Enqueue.WriteBuffer(Buffer, source.BackBuffer, bufferLength);
        }

        public T[] Upload()
        {
            T[] temp = new T[Buffer.Size];
            accelerator.Enqueue.ReadBuffer(Buffer, temp);
            return temp;
        }

        public void Dispose() => Buffer?.Dispose();
    }
}
