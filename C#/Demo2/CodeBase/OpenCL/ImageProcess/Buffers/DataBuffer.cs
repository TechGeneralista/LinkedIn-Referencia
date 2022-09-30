using Common;
using OpenCLWrapper;
using OpenCLWrapper.Internals;


namespace ImageProcess.Buffers
{
    public class DataBuffer<T>
    {
        public Buffer<T> Data { get; private set; }


        readonly OpenCLAccelerator accelerator;


        public DataBuffer(OpenCLAccelerator accelerator) => this.accelerator = accelerator;


        public void Create(int size)
        {
            if(Data.IsNull() || Data.Size != size)
            {
                Data?.Dispose();
                Data = new Buffer<T>(accelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, size);
            }
        }

        public void Download(T[] source) => accelerator.Enqueue.WriteBuffer(Data, source);

        public T[] Upload()
        {
            T[] temp = new T[Data.Size];
            accelerator.Enqueue.ReadBuffer(Data, temp);
            return temp;
        }
    }
}
