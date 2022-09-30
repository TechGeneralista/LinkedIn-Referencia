﻿using ImageProcess.Buffer;
using ImageProcess.KernelSource;
using OpenCLWrapper.Internals;


namespace ImageProcess.Templates
{
    public class Monochrome
    {
        public Image2DBuffer Output { get; private set; }


        readonly OpenCLWrapper.OpenCLAccelerator openCLAccelerator;
        readonly OpenCLWrapper.Kernel kernelMonochrome;


        public Monochrome(OpenCLWrapper.OpenCLAccelerator openCLAccelerator)
        {
            this.openCLAccelerator = openCLAccelerator;
            kernelMonochrome = openCLAccelerator.GetKernel(KernelSourceMonochrome.FunctionName, KernelSourceMonochrome.FunctionSource);

            Output = new Image2DBuffer(openCLAccelerator);
        }

        public void Convert(Image2DBuffer input)
        {
            Output.Create(input.BufferImage.Descriptor.Width, input.BufferImage.Descriptor.Height);
            kernelMonochrome.SetArg(0, input.BufferImage);
            kernelMonochrome.SetArg(1, Output.BufferImage);
            openCLAccelerator.Enqueue.Execute(kernelMonochrome, new SizeT[] { input.BufferImage.Descriptor.Width, input.BufferImage.Descriptor.Height });
        }
    }
}
