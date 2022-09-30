using Common.Types;
using ImageProcess.Buffers;
using ImageProcess.Operations.Kernels;
using OpenCLWrapper;
using System.Windows.Media.Imaging;


namespace ImageProcess.Operations
{
    public class EdgeDetect
    {
        public WriteableBitmapBuffer Output { get; private set; }


        readonly OpenCLAccelerator accelerator;
        readonly Monochrome monochrome;
        readonly Blur blur;
        readonly WriteableBitmapBuffer sobelOutput;
        readonly DataBuffer<uint> magnitudeSobelOutput;


        public EdgeDetect(OpenCLAccelerator accelerator)
        {
            this.accelerator = accelerator;
            Output = new WriteableBitmapBuffer(accelerator);
            monochrome = new Monochrome(accelerator);
            blur = new Blur(accelerator);
            sobelOutput = new WriteableBitmapBuffer(accelerator);
            magnitudeSobelOutput = new DataBuffer<uint>(accelerator);
        }

        public void Create(WriteableBitmapBuffer input, uint blurSize)
        {
            Output.Create(input.Data.Descriptor.Width, input.Data.Descriptor.Height);

            monochrome.Convert(input);
            blur.Create(monochrome.Output, blurSize);

            sobelOutput.Create(input.Data.Descriptor.Width, input.Data.Descriptor.Height);
            magnitudeSobelOutput.Create(input.Data.Descriptor.Width * input.Data.Descriptor.Height);

            Kernel kernel = accelerator.GetKernel(KernelSourceSobel.FunctionName, KernelSourceSobel.FunctionSource);
            kernel.SetArg(0, blur.Output.Data);
            kernel.SetArg(1, sobelOutput.Data);
            kernel.SetArg(2, magnitudeSobelOutput.Data);
            accelerator.Enqueue.Execute(kernel, new SizeT[] { input.Data.Descriptor.Width, input.Data.Descriptor.Height });

            //kernel = accelerator.GetKernel(KernelSourceNonMaxSupress.FunctionName, KernelSourceNonMaxSupress.FunctionSource);

        }

        public WriteableBitmap Upload() => sobelOutput.Upload();
    }
}
