using Common.Types;
using ImageProcess.Buffers;
using ImageProcess.Operations.Kernels;
using OpenCLWrapper;
using OpenCLWrapper.Internals;


namespace ImageProcess.Operations
{
    public class NonBlackPixelCounter
    {
        public uint CountedPixels { get; private set; }


        readonly OpenCLAccelerator accelerator;
        readonly Buffer<uint> buffer;


        public NonBlackPixelCounter(OpenCLAccelerator accelerator)
        {
            this.accelerator = accelerator;
            buffer = new Buffer<uint>(accelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, 1);
        }

        public void Count(WriteableBitmapBuffer input)
        {
            uint[] outputBuffer = new uint[] { 0 };
            accelerator.Enqueue.WriteBuffer(buffer, outputBuffer);

            Kernel kernel = accelerator.GetKernel(KernelSourceNonBlackPixelCounter.FunctionName, KernelSourceNonBlackPixelCounter.FunctionSource);
            kernel.SetArg(0, input.Data);
            kernel.SetArg(1, buffer);

            accelerator.Enqueue.Execute(kernel, new SizeT[] { input.Data.Descriptor.Width, input.Data.Descriptor.Height });

            accelerator.Enqueue.ReadBuffer(buffer, outputBuffer);
            CountedPixels = outputBuffer[0];
        }
    }
}
