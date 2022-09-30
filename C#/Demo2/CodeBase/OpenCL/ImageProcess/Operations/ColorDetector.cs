using Common.Types;
using ImageProcess.Buffers;
using ImageProcess.Operations.Kernels;
using OpenCLWrapper;
using System.Windows.Media;


namespace ImageProcess.Operations
{
    public class ColorDetector
    {
        public WriteableBitmapBuffer Output { get; private set; }


        readonly OpenCLAccelerator accelerator;


        public ColorDetector(OpenCLAccelerator accelerator) 
        {
            this.accelerator = accelerator;
            Output = new WriteableBitmapBuffer(accelerator);
        }

        public void Start(WriteableBitmapBuffer input, Color[] referenceColors, uint tolerance)
        {
            Output.Create(input.Data.Descriptor.Width, input.Data.Descriptor.Height);
            Output.Clear(Colors.Black);

            Kernel kernel = accelerator.GetKernel(KernelSourceColorDetector.FunctionName, KernelSourceColorDetector.FunctionSource);
            kernel.SetArg(0, input.Data);
            kernel.SetArg(1, Output.Data);
            kernel.SetArg(2, tolerance);

            foreach (Color color in referenceColors)
            {
                kernel.SetArg(3, color.B);
                kernel.SetArg(4, color.G);
                kernel.SetArg(5, color.R);
                accelerator.Enqueue.Execute(kernel, new SizeT[] { input.Data.Descriptor.Width, input.Data.Descriptor.Height });
            }
        }
    }
}
