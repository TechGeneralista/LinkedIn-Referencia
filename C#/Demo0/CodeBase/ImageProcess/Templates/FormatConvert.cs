using ImageProcess.KernelSource;
using OpenCLWrapper;
using OpenCLWrapper.Internals;
using System.Windows.Media.Imaging;


namespace ImageProcess.Templates
{
    public class FormatConvert
    {
        public Buffer.Image2DBuffer Output { get; private set; }


        readonly OpenCLAccelerator openCLAccelerator;
        readonly Kernel kernelBGR24ToBGRA32;


        public FormatConvert(OpenCLAccelerator openCLAccelerator) 
        {
            this.openCLAccelerator = openCLAccelerator;
            kernelBGR24ToBGRA32 = openCLAccelerator.GetKernel(KernelSourceBGR24ToBGRA32.FunctionName, KernelSourceBGR24ToBGRA32.FunctionSource);

            Output = new Buffer.Image2DBuffer(openCLAccelerator);
        }

        public void ConvertBGR24ToBGRA32(int width, int height, int stride, Buffer.DataBuffer<byte> sourceBuffer)
        {
            Output.Create(width, height);
            kernelBGR24ToBGRA32.SetArg(0, sourceBuffer.Buffer);
            kernelBGR24ToBGRA32.SetArg(1, Output.BufferImage);
            kernelBGR24ToBGRA32.SetArg(2, stride);
            openCLAccelerator.Enqueue.Execute(kernelBGR24ToBGRA32, new SizeT[] { width, height });
        }

        public void ConvertBGR32ToBGRA32(WriteableBitmap source, Buffer.DataBuffer<byte> sourceBuffer)
        {
            Output.Create(source.PixelWidth, source.PixelHeight);
            Kernel kernel = openCLAccelerator.GetKernel(KernelSourceBGR32ToBGRA32.FunctionName, KernelSourceBGR32ToBGRA32.FunctionSource);
            kernel.SetArg(0, sourceBuffer.Buffer);
            kernel.SetArg(1, Output.BufferImage);
            kernel.SetArg(2, source.BackBufferStride);
            openCLAccelerator.Enqueue.Execute(kernel, new SizeT[] { source.PixelWidth, source.PixelHeight });
        }
    }
}
