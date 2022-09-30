using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLMonochromeDoubleThreshold : ObservableProperty
    {
        public event Action ThresholdChanged;

        public WriteableBitmap InputImage
        {
            get => inputImage;
            set { SetField(value, ref inputImage); Start(); }
        }

        public WriteableBitmap OutputImage
        {
            get => outputImage;
            set => SetField(value, ref outputImage);
        }

        public int HighThresholdValue
        {
            get => highThresholdValue;
            set { SetField(value, ref highThresholdValue); LowThresholdMaximum = highThresholdValue; ThresholdChanged?.Invoke(); }
        }

        public int LowThresholdValue
        {
            get => lowThresholdValue;
            set { SetField(value, ref lowThresholdValue); HighThresholdMinimum = lowThresholdValue; ThresholdChanged?.Invoke(); }
        }

        public int HighThresholdMinimum
        {
            get => highThresholdMinimum;
            set => SetField(value, ref highThresholdMinimum);
        }

        public int LowThresholdMaximum
        {
            get => lowThresholdMaximum;
            set => SetField(value, ref lowThresholdMaximum);
        }


        OpenCLDevice openCLDevice;
        WriteableBitmap inputImage, outputImage;
        ComputeProgram computeProgram;
        ComputeKernel computeKernel;
        int highThresholdValue, lowThresholdValue;
        int highThresholdMinimum, lowThresholdMaximum;

        readonly string cSource =
            @"
                
                kernel void threshold(global uchar* input, global uchar* output, int stride, int bpp, int highThresholdValue, int lowThresholdValue) 
                {
                    int x = get_global_id(0);
                    int y = get_global_id(1);

                    int addr = (stride*y) + (x*bpp);
                    uchar iPixel = input[addr];

                    if(iPixel >= highThresholdValue)
                        output[addr] = output[addr + 1] = output[addr + 2] = 255;
                    else if(iPixel < highThresholdValue && iPixel >= lowThresholdValue)
                        output[addr] = output[addr + 1] = output[addr + 2] = 64;
                    else
                        output[addr] = output[addr + 1] = output[addr + 2] = 0;

                    output[addr + 3] = 255;
                }
                
            ";

        WriteableBitmap inputBitmap, outputBitmap;

        public OCLMonochromeDoubleThreshold(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;

            computeProgram = new ComputeProgram(openCLDevice.ComputeContext, cSource);

            try { computeProgram.Build(null, null, null, IntPtr.Zero); }
            catch { Debug.WriteLine(computeProgram.GetBuildLog(openCLDevice.ComputeDevice)); }
            
            computeKernel = computeProgram.CreateKernel("threshold");

            HighThresholdValue = 32;
            LowThresholdValue = 16;
        }

        ~OCLMonochromeDoubleThreshold()
        {
            computeProgram.Dispose();
            computeKernel.Dispose();
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
            computeKernel.SetValueArgument(4, highThresholdValue);
            computeKernel.SetValueArgument(5, lowThresholdValue);
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

            OutputImage = outputBitmap;
        }
    }
}
