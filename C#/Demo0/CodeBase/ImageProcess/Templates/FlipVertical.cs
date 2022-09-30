using ImageProcess.KernelSource;
using OpenCLWrapper;
using OpenCLWrapper.Internals;


namespace ImageProcess.Templates
{
    public class FlipVertical
    {
        public Buffer.Image2DBuffer Output { get; private set; }


        readonly OpenCLAccelerator openCLAccelerator;
        readonly Kernel kernelFlipVertical;


        public FlipVertical(OpenCLAccelerator openCLAccelerator)
        {
            this.openCLAccelerator = openCLAccelerator;
            kernelFlipVertical = openCLAccelerator.GetKernel(KernelSourceFlipVertical.FunctionName, KernelSourceFlipVertical.FunctionSource);

            Output = new Buffer.Image2DBuffer(openCLAccelerator);
        }

        public void Flip(Buffer.Image2DBuffer input)
        {
            Output.Create(input.BufferImage.Descriptor.Width, input.BufferImage.Descriptor.Height);
            kernelFlipVertical.SetArg(0, input.BufferImage);
            kernelFlipVertical.SetArg(1, Output.BufferImage);
            kernelFlipVertical.SetArg(2, input.BufferImage.Descriptor.Height-1);
            openCLAccelerator.Enqueue.Execute(kernelFlipVertical, new SizeT[] { input.BufferImage.Descriptor.Width, input.BufferImage.Descriptor.Height });
        }
    }
}
