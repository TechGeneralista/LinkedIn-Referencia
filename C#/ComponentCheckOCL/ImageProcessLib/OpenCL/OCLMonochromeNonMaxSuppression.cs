using CommonLib;
using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLMonochromeNonMaxSuppression : ObservableProperty
    {
        public WriteableBitmap OutputImage
        {
            get => outputImage;
            set => SetField(value, ref outputImage);
        }


        OpenCLDevice openCLDevice;
        WriteableBitmap outputImage;
        static ComputeKernel computeKernel;
        WriteableBitmap outputBitmap;

        readonly string cSource =
            @"
                int getAddress(int x, int y, int stride, int bpp)
                {
                    return (stride*y) + (x*bpp);
                }

                kernel void nonMaximumSuppression(global uchar* input, global uchar* output, global int* magnitude, int stride, int bpp, int width) 
                {
                    int x = get_global_id(0);
                    int y = get_global_id(1);
                    
                    int magnitudeArrayAddr = getAddress(x, y, width, 1);
                    int outputArrayAddr = getAddress(x, y, stride, bpp);

                    uchar q = 255;
                    uchar r = 255;

                    //angle 0
                    if (    ((0 <= magnitude[magnitudeArrayAddr]) && (magnitude[magnitudeArrayAddr] < 22.5)) || 
                        ((157.5 <= magnitude[magnitudeArrayAddr]) && (magnitude[magnitudeArrayAddr] <= 180))    )
                    {
                        q = input[getAddress(x+1, y, stride, bpp)];
                        r = input[getAddress(x-1, y, stride, bpp)];
                    }
                    
                    //angle 45
                    else if ((22.5 <= magnitude[magnitudeArrayAddr]) && (magnitude[magnitudeArrayAddr] < 67.5))
                    {
                        q = input[getAddress(x+1, y+1, stride, bpp)];
                        r = input[getAddress(x-1, y-1, stride, bpp)];
                    }
                        
                    //angle 90
                    else if ((67.5 <= magnitude[magnitudeArrayAddr]) && (magnitude[magnitudeArrayAddr] < 112.5))
                    {
                        q = input[getAddress(x, y+1, stride, bpp)];
                        r = input[getAddress(x, y-1, stride, bpp)];
                    }
                    
                    //angle 135
                    else if ((112.5 <= magnitude[magnitudeArrayAddr]) && (magnitude[magnitudeArrayAddr] < 157.5))
                    {
                        q = input[getAddress(x+1, y-1, stride, bpp)];
                        r = input[getAddress(x-1, y+1, stride, bpp)];
                    }
                    
                    uchar currentValue = input[getAddress(x, y, stride, bpp)];

                    if ((currentValue >= q) && (currentValue >= r))
                    {
                        output[outputArrayAddr] =                         //b
                        output[outputArrayAddr + 1] =                     //g
                        output[outputArrayAddr + 2] = currentValue;       //r
                        output[outputArrayAddr + 3] = 255;                //a
                    }
                    else
                    {
                        output[outputArrayAddr] =                         //b
                        output[outputArrayAddr + 1] =                     //g
                        output[outputArrayAddr + 2] = 0;                  //r
                        output[outputArrayAddr + 3] = 255;                //a
                    }
                }
                
            ";


        public OCLMonochromeNonMaxSuppression(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;

            if (computeKernel.IsNull())
            {
                ComputeProgram computeProgram = new ComputeProgram(openCLDevice.ComputeContext, cSource);
                
                try
                {
                    computeProgram.Build(null, null, null, IntPtr.Zero);
                }
                catch
                {
                    Debug.WriteLine(computeProgram.GetBuildLog(openCLDevice.ComputeDevice));
                }
                
                computeKernel = computeProgram.CreateKernel("nonMaximumSuppression");
            }
        }

        public void Start(WriteableBitmap inputImage, int[] magnitude)
        {
            if (inputImage is null)
                return;

            inputImage = inputImage.Clone();
            int stride = inputImage.BackBufferStride;
            int bpp = inputImage.Format.BitsPerPixel / 8;
            int width = inputImage.PixelWidth;
            int height = inputImage.PixelHeight;

            if (outputBitmap is null || outputBitmap.PixelWidth != width || outputBitmap.PixelHeight != height)
                outputBitmap = new WriteableBitmap(width, height, 96, 96, inputImage.Format, null);
            else
                outputBitmap = outputBitmap.Clone();

            GCHandle handle = GCHandle.Alloc(magnitude, GCHandleType.Pinned);

            ComputeBuffer<byte> inputBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, stride * height, inputImage.BackBuffer);
            ComputeBuffer<byte> magnitudeBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, stride * height, handle.AddrOfPinnedObject());
            ComputeBuffer<byte> outputBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.UseHostPointer, stride * height, outputBitmap.BackBuffer);

            computeKernel.SetMemoryArgument(0, inputBuffer);
            computeKernel.SetMemoryArgument(1, outputBuffer);
            computeKernel.SetMemoryArgument(2, magnitudeBuffer);
            computeKernel.SetValueArgument(3, stride);
            computeKernel.SetValueArgument(4, bpp);
            computeKernel.SetValueArgument(5, width);
            openCLDevice.ComputeCommandQueue.Execute(computeKernel, new long[] { 1, 1 }, new long[] { width - 2, height - 2 }, null, null);

            if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
                openCLDevice.ComputeCommandQueue.Finish();

            else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu)
                openCLDevice.ComputeCommandQueue.Read(outputBuffer, true, 0, stride * height, outputBitmap.BackBuffer, null);

            inputBuffer.Dispose();
            outputBuffer.Dispose();
            handle.Free();

            outputBitmap.Freeze();
            OutputImage = outputBitmap;
        }
    }
}
