using Common.Types;
using ImageProcess.Buffers;
using ImageProcess.Operations.Kernels;
using OpenCLWrapper;


namespace ImageProcess.Operations
{
    public class FlipVertical
    {
        public WriteableBitmapBuffer Output { get; private set; }


        OpenCLAccelerator openCLAccelerator;


        public FlipVertical(OpenCLAccelerator openCLAccelerator)
        {
            this.openCLAccelerator = openCLAccelerator;
            Output = new WriteableBitmapBuffer(openCLAccelerator);
        }

        public void Flip(WriteableBitmapBuffer input)
        {
            Output.Create((int)input.Data.Descriptor.Width, (int)input.Data.Descriptor.Height);
            Kernel kernel = openCLAccelerator.GetKernel(KernelSourceFlipVertical.FunctionName, KernelSourceFlipVertical.FunctionSource);
            kernel.SetArg(0, input.Data);
            kernel.SetArg(1, Output.Data);
            kernel.SetArg(2, (int)(input.Data.Descriptor.Height-1));
            openCLAccelerator.Enqueue.Execute(kernel, new SizeT[] { input.Data.Descriptor.Width, input.Data.Descriptor.Height });
        }
    }
}
