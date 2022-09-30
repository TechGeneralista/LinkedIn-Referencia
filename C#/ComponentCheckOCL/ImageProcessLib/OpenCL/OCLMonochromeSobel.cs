using CommonLib;
using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLMonochromeSobel : ObservableProperty
    {
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

        public int[] Magnitude { get; private set; }


        OpenCLDevice openCLDevice;
        WriteableBitmap inputImage, outputImage;
        static ComputeKernel computeKernel;

        readonly string cSource =
            @"
                constant int sobel[] = {1, 2, 1, -1, -2, -1};

                int getAddress(int x, int y, int stride, int bpp)
                {
                    return (stride*y) + (x*bpp);
                }

                int getHValue(global uchar* input, int x, int y, int stride, int bpp)
                {
                    int value =  (int)( ((int)input[getAddress(x-1, y-1, stride, bpp)] * (int)sobel[0]) + 
                                        ((int)input[getAddress(x-1, y, stride, bpp)] * (int)sobel[1]) + 
                                        ((int)input[getAddress(x-1, y+1, stride, bpp)] * (int)sobel[2]) + 
                                        ((int)input[getAddress(x+1, y-1, stride, bpp)] * (int)sobel[3]) + 
                                        ((int)input[getAddress(x+1, y, stride, bpp)] * (int)sobel[4]) + 
                                        ((int)input[getAddress(x+1, y+1, stride, bpp)] * (int)sobel[5]));

                    return value;
                }

                int getVValue(global uchar* input, int x, int y, int stride, int bpp)
                {
                    int value = (int)(  ((int)input[getAddress(x - 1, y - 1, stride, bpp)] * (int)sobel[0]) + 
                                        ((int)input[getAddress(x    , y - 1, stride, bpp)] * (int)sobel[1]) + 
                                        ((int)input[getAddress(x + 1, y - 1, stride, bpp)] * (int)sobel[2]) + 
                                        ((int)input[getAddress(x - 1, y + 1, stride, bpp)] * (int)sobel[3]) + 
                                        ((int)input[getAddress(x    , y + 1, stride, bpp)] * (int)sobel[4]) + 
                                        ((int)input[getAddress(x + 1, y + 1, stride, bpp)] * (int)sobel[5]));

                    return value;
                }
                
                kernel void sobelFunction(global uchar* input, global uchar* output, global int* magnitude, int stride, int bpp, int width) 
                {
                    int x = get_global_id(0);
                    int y = get_global_id(1);

                    int hValue = getHValue(input, x, y, stride, bpp);
                    int vValue = getVValue(input, x, y, stride, bpp);

                    float rad = atan2((float)vValue, (float)hValue);
                    int degArrayAddr = getAddress(x, y, width, 1);
                    magnitude[degArrayAddr] = (rad * (float)180) / M_PI_F;

                    if(magnitude[degArrayAddr] < 0)
                        magnitude[degArrayAddr] += 180;

                    int pixelValue = (int)sqrt( (float)((hValue*hValue) + (vValue*vValue)) );

                    if(pixelValue > 255)
                        pixelValue = 255;
                    
                    int outputArrayAddr = getAddress(x, y, stride, bpp);

                    output[outputArrayAddr] =                         //b
                    output[outputArrayAddr + 1] =                     //g
                    output[outputArrayAddr + 2] = (uchar)pixelValue;  //r
                    output[outputArrayAddr + 3] = 255;                //a
                }
                
            ";

        WriteableBitmap outputBitmap;

        public OCLMonochromeSobel(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;

            if(computeKernel.IsNull())
            {
                ComputeProgram computeProgram = new ComputeProgram(openCLDevice.ComputeContext, cSource);
                computeProgram.Build(null, null, null, IntPtr.Zero);
                computeKernel = computeProgram.CreateKernel("sobelFunction");
            }
        }

        private void Start()
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

            if (Magnitude is null || Magnitude.Length != (width * height))
                Magnitude = new int[width * height];

            GCHandle handle = GCHandle.Alloc(Magnitude, GCHandleType.Pinned);

            ComputeBuffer<byte> inputBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, stride * height, inputImage.BackBuffer);
            ComputeBuffer<byte> outputBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.UseHostPointer, stride * height, outputBitmap.BackBuffer);
            ComputeBuffer<byte> magnitudeBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.UseHostPointer, stride * height, handle.AddrOfPinnedObject());

            computeKernel.SetMemoryArgument(0, inputBuffer);
            computeKernel.SetMemoryArgument(1, outputBuffer);
            computeKernel.SetMemoryArgument(2, magnitudeBuffer);
            computeKernel.SetValueArgument(3, stride);
            computeKernel.SetValueArgument(4, bpp);
            computeKernel.SetValueArgument(5, width);
            openCLDevice.ComputeCommandQueue.Execute(computeKernel, new long[] { 1,1 }, new long[] { width-2, height-2 }, null, null);

            if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
                openCLDevice.ComputeCommandQueue.Finish();

            else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu)
            {
                openCLDevice.ComputeCommandQueue.Read(outputBuffer, true, 0, stride * height, outputBitmap.BackBuffer, null);
                openCLDevice.ComputeCommandQueue.Read(magnitudeBuffer, true, 0, stride * height, handle.AddrOfPinnedObject(), null);
            }

            inputBuffer.Dispose();
            outputBuffer.Dispose();
            handle.Free();
            magnitudeBuffer.Dispose();

            outputBitmap.Freeze();
            OutputImage = outputBitmap;
        }
    }
}
