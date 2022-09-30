using ImageProcess.KernelSources;
using OpenCLWrapper;
using OpenCLWrapper.Buffer;
using OpenCLWrapper.CL;
using OpenCLWrapper.CL.Internals;


namespace ImageProcess.Templates
{
    public class FlipVertical
    {
        public Image2DBuffer Output { get; private set; }


        readonly OpenCLAccelerator openCLAccelerator;
        readonly Kernel kernelFlipVertical;


        public FlipVertical(OpenCLAccelerator openCLAccelerator)
        {
            this.openCLAccelerator = openCLAccelerator;
            kernelFlipVertical = openCLAccelerator.GetKernel(KernelSourceFlipVertical.FunctionName, KernelSourceFlipVertical.FunctionSource);

            Output = new Image2DBuffer(openCLAccelerator);
        }

        public void Flip(Image2DBuffer input)
        {
            Output.Create(input.BufferImage.Descriptor.Width, input.BufferImage.Descriptor.Height);
            kernelFlipVertical.SetArg(0, input.BufferImage);
            kernelFlipVertical.SetArg(1, Output.BufferImage);
            kernelFlipVertical.SetArg(2, input.BufferImage.Descriptor.Height-1);
            openCLAccelerator.Enqueue.Execute(kernelFlipVertical, new SizeT[] { input.BufferImage.Descriptor.Width, input.BufferImage.Descriptor.Height });
        }
    }
}
