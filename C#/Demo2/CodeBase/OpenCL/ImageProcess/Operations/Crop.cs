using Common.Types;
using ImageProcess.Buffers;
using ImageProcess.Operations.Kernels;
using OpenCLWrapper;
using System.Drawing;


namespace ImageProcess.Operations
{
    public class Crop
    {
        public WriteableBitmapBuffer Output { get; private set; }


        OpenCLAccelerator openCLAccelerator;


        public Crop(OpenCLAccelerator openCLAccelerator) 
        {
            this.openCLAccelerator = openCLAccelerator;
            Output = new WriteableBitmapBuffer(openCLAccelerator);
        }

        public void Start(WriteableBitmapBuffer input, Rectangle rect)
        {
            Output.Create(rect.Width, rect.Height);
            Kernel kernel = openCLAccelerator.GetKernel(KernelSourceCrop.FunctionName, KernelSourceCrop.FunctionSource);
            kernel.SetArg(0, input.Data);
            kernel.SetArg(1, Output.Data);
            kernel.SetArg(2, rect.X);
            kernel.SetArg(3, rect.Y);
            openCLAccelerator.Enqueue.Execute(kernel, new SizeT[] { rect.Width, rect.Height});
        }
    }
}
