using Common.Types;
using ImageProcess.Buffers;
using ImageProcess.Operations.Kernels;
using OpenCLWrapper;


namespace ImageProcess.Operations
{
    public class FormatConvert
    {
        public WriteableBitmapBuffer Output { get; private set; }


        OpenCLAccelerator openCLAccelerator;


        public FormatConvert(OpenCLAccelerator openCLAccelerator) 
        {
            this.openCLAccelerator = openCLAccelerator;
            Output = new WriteableBitmapBuffer(openCLAccelerator);
        }

        public void Convert(ImageBufferBGR24 input)
        {
            Output.Create(input.Width, input.Height);
            Kernel kernel = openCLAccelerator.GetKernel(KernelSourceBGR24toBGRA32.FunctionName, KernelSourceBGR24toBGRA32.FunctionSource);
            kernel.SetArg(0, input.Buffer);
            kernel.SetArg(1, Output.Data);
            kernel.SetArg(2, input.Stride);
            kernel.SetArg(3, input.BytePerPixel);
            openCLAccelerator.Enqueue.Execute(kernel, new SizeT[] { (uint)input.Width, (uint)input.Height });
        }
    }
}
