using Common.Types;
using ImageProcess.Buffers;
using ImageProcess.Operations.Kernels;
using OpenCLWrapper;


namespace ImageProcess.Operations
{
    public class Erode
    {
        public WriteableBitmapBuffer Output { get; private set; }


        readonly OpenCLAccelerator accelerator;
        readonly WriteableBitmapBuffer temp;


        public Erode(OpenCLAccelerator accelerator)
        {
            this.accelerator = accelerator;
            temp = new WriteableBitmapBuffer(accelerator);
            Output = new WriteableBitmapBuffer(accelerator);
        }

        public void Start(WriteableBitmapBuffer input, uint size)
        {
            Output.CreateCopyFrom(input);

            if (size == 0) 
                return;

            Kernel kernel = accelerator.GetKernel(KernelSourceErode.FunctionName, KernelSourceErode.FunctionSource);
            temp.CreateCopyFrom(input);

            for (int i = 0; i < size; i++)
            {
                kernel.SetArg(0, temp.Data);
                kernel.SetArg(1, Output.Data);
                accelerator.Enqueue.Execute(kernel, new SizeT[] { input.Data.Descriptor.Width, input.Data.Descriptor.Height });

                if (i != (size - 1))
                    temp.CreateCopyFrom(Output);
            }
        }
    }
}
