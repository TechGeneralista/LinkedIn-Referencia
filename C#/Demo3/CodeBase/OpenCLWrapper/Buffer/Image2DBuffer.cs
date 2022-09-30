using Common;
using OpenCLWrapper.CL;
using OpenCLWrapper.CL.Internals;
using OpenCLWrapper.KernelSources;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace OpenCLWrapper.Buffer
{
    public class Image2DBuffer
    {
        public BufferImage BufferImage { get; private set; }


        readonly OpenCLAccelerator accelerator;
        readonly Kernel kernelFillBuffer;


        public Image2DBuffer(OpenCLAccelerator accelerator) 
        {
            this.accelerator = accelerator;
            kernelFillBuffer = accelerator.GetKernel(KernelSourceFillBuffer.FunctionName, KernelSourceFillBuffer.FunctionSource);
        }


        public void Create(Image2DBuffer source) => Create(source.BufferImage.Descriptor.Width, source.BufferImage.Descriptor.Height);

        public void Create(int width, int height)
        {
            if (BufferImage.IsNull() || (BufferImage.Descriptor.Width != width || BufferImage.Descriptor.Height != height))
            {
                BufferImage?.Dispose();
                CLImageFormat format = new CLImageFormat(CLChannelOrder.RGBA, CLChannelType.UnSignedInt8);
                CLImageDesc desc = new CLImageDesc(CLMemObjectType.Image2D, (uint)width, (uint)height);
                BufferImage = new BufferImage(accelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, format, desc);
            }
        }

        public void CreateCopyFrom(Image2DBuffer source)
        {
            Create(source);
            accelerator.Enqueue.Copy(source.BufferImage, BufferImage);
        }

        public void Download(WriteableBitmap source)
        {
            if (source.Format != PixelFormats.Bgra32)
                throw new NotSupportedException();

            Create(source.PixelWidth, source.PixelHeight);
            accelerator.Enqueue.WriteBuffer(BufferImage, source.BackBuffer);
        }

        public WriteableBitmap Upload()
        {
            WriteableBitmap bitmap = new WriteableBitmap(BufferImage.Descriptor.Width, BufferImage.Descriptor.Height, 96, 96, PixelFormats.Bgra32, null);
            bitmap.Lock();
            accelerator.Enqueue.ReadBuffer(BufferImage, bitmap.BackBuffer);
            bitmap.AddDirtyRect(new Int32Rect(0,0, bitmap.PixelWidth, bitmap.PixelHeight));
            bitmap.Unlock();
            bitmap.Freeze();
            return bitmap;
        }

        public void Clear(Color color)
        {
            kernelFillBuffer.SetArg(0, BufferImage);
            kernelFillBuffer.SetArg(1, color.B);
            kernelFillBuffer.SetArg(2, color.G);
            kernelFillBuffer.SetArg(3, color.R);
            kernelFillBuffer.SetArg(4, color.A);
            accelerator.Enqueue.Execute(kernelFillBuffer, new SizeT[] { BufferImage.Descriptor.Width, BufferImage.Descriptor.Height });
        }
    }
}
