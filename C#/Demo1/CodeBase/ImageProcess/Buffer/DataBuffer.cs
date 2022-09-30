using Compute;
using Compute.CL;
using Compute.CL.Internals;
using System;


namespace ImageProcess.Buffer
{
    public class DataBuffer<T> : Buffer<T>
    {
        private readonly CLMemFlags defaultMemFlags = CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr;
        private readonly Enqueue enqueue;


        public DataBuffer(ComputeAccelerator computeAccelerator) : base(computeAccelerator)
        {
            enqueue = computeAccelerator.Enqueue;
        }

        public void CreateIfNeed(int size)
            => CreateIfNeed(defaultMemFlags, size);

        public void Download(T[] source)
        {
            CreateIfNeed(source.Length);
            enqueue.WriteBuffer(this, source);
        }

        public void Download(IntPtr buffer, int bufferLength)
        {
            CreateIfNeed(bufferLength);
            enqueue.WriteBuffer(this, buffer, bufferLength);
        }

        public void Upload(out T[] array)
        {
            array = new T[Size];
            enqueue.ReadBuffer(this, array);
        }
    }
}
