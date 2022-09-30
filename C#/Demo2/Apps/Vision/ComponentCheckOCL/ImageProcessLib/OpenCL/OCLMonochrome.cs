using CommonLib;
using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLMonochrome : ObservableProperty
    {
        public WriteableBitmap InputImage
        {
            get => inputImage;
            set { SetField(value, ref inputImage); Start(); }
        }

        public WriteableBitmap MonochromeImage
        {
            get => monochromeImage;
            set => SetField(value, ref monochromeImage);
        }


        OpenCLDevice openCLDevice;
        WriteableBitmap inputImage, monochromeImage;
        ComputeKernel computeKernel;

        readonly string cSource =
            @"
                
                kernel void monochrome(global uchar* input, global uchar* output, int stride, int bpp) 
                {
                    int x = get_global_id(0);
                    int y = get_global_id(1);

                    int addr = (stride*y) + (x*bpp);
                    uchar mon = (uchar)(((float)input[addr] * (float)0.0721) + ((float)input[addr+1] * (float)0.7154) + ((float)input[addr+2] * (float)0.2125));
                    output[addr] = output[addr+1] = output[addr+2] = mon;
                    output[addr + 3] = input[addr + 3];
                }
                
            ";

        WriteableBitmap inputBitmap, outputBitmap;

        public OCLMonochrome(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;

            if(computeKernel.IsNull())
            {
                ComputeProgram computeProgram = new ComputeProgram(openCLDevice.ComputeContext, cSource);
                computeProgram.Build(null, null, null, IntPtr.Zero);
                computeKernel = computeProgram.CreateKernel("monochrome");
            }
        }

        private void Start()
        {
            if (inputImage == null)
                return;

            inputBitmap = inputImage.Clone();
            int stride = inputBitmap.BackBufferStride;
            int bpp = inputBitmap.Format.BitsPerPixel / 8;
            int width = inputBitmap.PixelWidth;
            int height = inputBitmap.PixelHeight;

            if (outputBitmap is null || outputBitmap.PixelWidth != width || outputBitmap.PixelHeight != height)
                outputBitmap = new WriteableBitmap(width, height, 96, 96, inputBitmap.Format, null);
            else
                outputBitmap = outputBitmap.Clone();

            inputBitmap.Lock();
            outputBitmap.Lock();

            ComputeBuffer<byte> inputBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, stride * height, inputBitmap.BackBuffer);
            ComputeBuffer<byte> outputBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.UseHostPointer, stride * height, outputBitmap.BackBuffer);
            
            computeKernel.SetMemoryArgument(0, inputBuffer);
            computeKernel.SetMemoryArgument(1, outputBuffer);
            computeKernel.SetValueArgument(2, stride);
            computeKernel.SetValueArgument(3, bpp);
            openCLDevice.ComputeCommandQueue.Execute(computeKernel, new long[] { 0 }, new long[] { width, height }, null, null);

            if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
                openCLDevice.ComputeCommandQueue.Finish();

            else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu)
                openCLDevice.ComputeCommandQueue.Read(outputBuffer, true, 0, stride * height, outputBitmap.BackBuffer, null);

            inputBuffer.Dispose();
            outputBuffer.Dispose();

            outputBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            inputBitmap.Unlock();
            outputBitmap.Unlock();
            outputBitmap.Freeze();
            MonochromeImage = outputBitmap;
        }
    }
}
