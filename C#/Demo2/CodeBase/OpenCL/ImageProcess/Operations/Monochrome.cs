using Common.Types;
using ImageProcess.Buffers;
using ImageProcess.Operations.Kernels;
using OpenCLWrapper;


namespace ImageProcess.Operations
{
    public class Monochrome
    {
        public WriteableBitmapBuffer Output { get; private set; }


        readonly OpenCLAccelerator openCLAccelerator;


        public Monochrome(OpenCLAccelerator openCLAccelerator)
        {
            this.openCLAccelerator = openCLAccelerator;
            Output = new WriteableBitmapBuffer(openCLAccelerator);
        }

        public void Convert(WriteableBitmapBuffer input)
        {
            Output.Create(input.Data.Descriptor.Width, input.Data.Descriptor.Height);
            Kernel kernel = openCLAccelerator.GetKernel(KernelSourceMonochrome.FunctionName, KernelSourceMonochrome.FunctionSource);
            kernel.SetArg(0, input.Data);
            kernel.SetArg(1, Output.Data);
            openCLAccelerator.Enqueue.Execute(kernel, new SizeT[] { input.Data.Descriptor.Width, input.Data.Descriptor.Height });
        }
    }
}
