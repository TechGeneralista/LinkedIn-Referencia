using ImageProcess.KernelSource;
using OpenCLWrapper;
using OpenCLWrapper.Internals;
using System;
using System.Windows.Media;

namespace ImageProcess.Templates
{
    public class BarrelDistortion
    {
        public Buffer.Image2DBuffer Output { get; private set; }


        readonly OpenCLAccelerator openCLAccelerator;
        readonly Kernel kernelBarrelDistortion;


        public BarrelDistortion(OpenCLAccelerator openCLAccelerator) 
        {
            this.openCLAccelerator = openCLAccelerator;
            kernelBarrelDistortion = openCLAccelerator.GetKernel(KernelSourceBarrelDistortion.FunctionName, KernelSourceBarrelDistortion.FunctionSource);

            Output = new Buffer.Image2DBuffer(openCLAccelerator);
        }

        public void Remove(Buffer.Image2DBuffer source, float strength, float zoom)
        {
            if (strength < (float)0.001)
                strength = (float)0.001;

            Output.Create(source);
            Output.Clear(Colors.Black);
            float correctionRadius = (float)Math.Sqrt(Math.Pow((int)Output.BufferImage.Descriptor.Width, 2) + Math.Pow((int)Output.BufferImage.Descriptor.Height, 2)) / strength;

            kernelBarrelDistortion.SetArg(0, source.BufferImage);
            kernelBarrelDistortion.SetArg(1, Output.BufferImage);
            kernelBarrelDistortion.SetArg(2, correctionRadius);
            kernelBarrelDistortion.SetArg(3, strength);
            kernelBarrelDistortion.SetArg(4, zoom);
            openCLAccelerator.Enqueue.Execute(kernelBarrelDistortion, new SizeT[] { Output.BufferImage.Descriptor.Width, source.BufferImage.Descriptor.Height });
        }
    }
}
