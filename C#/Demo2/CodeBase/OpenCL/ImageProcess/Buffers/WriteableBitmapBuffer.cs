using Common;
using Common.Types;
using ImageProcess.Operations.Kernels;
using OpenCLWrapper;
using OpenCLWrapper.Internals;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageProcess.Buffers
{
    public class WriteableBitmapBuffer
    {
        public BufferImage Data { get; private set; }


        readonly OpenCLAccelerator accelerator;
        WriteableBitmap temp;
        object tempLock = new object();


        public WriteableBitmapBuffer(OpenCLAccelerator accelerator) => this.accelerator = accelerator;


        public void Create(int width, int height)
        {
            if(Data.IsNull() || (Data.Descriptor.Width != width || Data.Descriptor.Height != height))
            {
                Data?.Dispose();
                CLImageFormat format = new CLImageFormat(CLChannelOrder.RGBA, CLChannelType.UnSignedInt8);
                CLImageDesc desc = new CLImageDesc(CLMemObjectType.Image2D, (uint)width, (uint)height);
                Data = new BufferImage(accelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, format, desc);
            }
        }

        public void CreateCopyFrom(WriteableBitmapBuffer source)
        {
            Create(source.Data.Descriptor.Width, source.Data.Descriptor.Height);
            accelerator.Enqueue.Copy(source.Data, Data);
        }

        public void Download(WriteableBitmap source)
        {
            Create(source.PixelWidth, source.PixelHeight);
            accelerator.Enqueue.WriteBuffer(Data, source.BackBuffer);
        }

        public WriteableBitmap Upload()
        {
            lock(tempLock)
            {
                if (temp.IsNull() || (temp.PixelWidth != Data.Descriptor.Width || temp.PixelHeight != Data.Descriptor.Height))
                    temp = new WriteableBitmap(Data.Descriptor.Width, Data.Descriptor.Height, 96, 96, PixelFormats.Bgra32, null);
                else
                {
                    if (temp.IsFrozen)
                        temp = temp.Clone();
                }

                accelerator.Enqueue.ReadBuffer(Data, temp.BackBuffer);
                temp.Freeze();

                return temp;
            }
        }

        public void Clear(Color color)
        {
            Kernel kernel = accelerator.GetKernel(KernelSourceFillBuffer.FunctionName, KernelSourceFillBuffer.FunctionSource);
            kernel.SetArg(0,Data);
            kernel.SetArg(1,(uint)color.B);
            kernel.SetArg(2,(uint)color.G);
            kernel.SetArg(3,(uint)color.R);
            kernel.SetArg(4,(uint)color.A);
            accelerator.Enqueue.Execute(kernel, new SizeT[] { Data.Descriptor.Width, Data.Descriptor.Height });
        }
    }
}
