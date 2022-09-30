using CommonLib;
using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLMonochromeWeakPixelRemove : ObservableProperty
    {
        public WriteableBitmap OutputImage
        {
            get => outputImage;
            set => SetField(value, ref outputImage);
        }


        OpenCLDevice openCLDevice;
        WriteableBitmap outputImage;
        ComputeKernel computeKernel;

        readonly string cSource =
            @"

                kernel void weakPixelRemove(global uchar* output, int stride, int bpp) 
                {
                    int x = get_global_id(0);
                    int y = get_global_id(1);

                    int addr = (stride * y) + (bpp * x);

                    if(output[addr] == 64)
                    {
                        output[addr] = 
                        output[addr+1] = 
                        output[addr+2] = 0;
                        output[addr+3] = 255;
                    }
                }
                
            ";

        public OCLMonochromeWeakPixelRemove(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;

            if (computeKernel.IsNull())
            {
                ComputeProgram computeProgram = new ComputeProgram(openCLDevice.ComputeContext, cSource);
                computeProgram.Build(null, null, null, IntPtr.Zero);
                computeKernel = computeProgram.CreateKernel("weakPixelRemove");
            }
        }

        public void Start(WriteableBitmap inputImage)
        {
            if (inputImage == null)
            {
                OutputImage = null;
                return;
            }

            inputImage = inputImage.Clone();
            int stride = inputImage.BackBufferStride;
            int bpp = inputImage.Format.BitsPerPixel / 8;
            int width = inputImage.PixelWidth;
            int height = inputImage.PixelHeight;

            inputImage.Lock();

            ComputeBuffer<byte> buffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, stride * height, inputImage.BackBuffer);

            computeKernel.SetMemoryArgument(0, buffer);
            computeKernel.SetValueArgument(1, stride);
            computeKernel.SetValueArgument(2, bpp);
            openCLDevice.ComputeCommandQueue.Execute(computeKernel, new long[] { 1, 1 }, new long[] { width - 1, height - 1 }, null, null);

            if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
                openCLDevice.ComputeCommandQueue.Finish();

            else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu)
                openCLDevice.ComputeCommandQueue.Read(buffer, true, 0, stride * height, inputImage.BackBuffer, null);

            buffer.Dispose();
            inputImage.Unlock();
            inputImage.Freeze();
            OutputImage = inputImage;
        }
    }
}
