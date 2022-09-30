using CommonLib;
using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLMonochromeGaussianBlur : ObservableProperty
    {
        public event Action BlurredImageChanged;

        public WriteableBitmap InputImage
        {
            get => inputImage;
            set { SetField(value, ref inputImage); Start(); }
        }

        public WriteableBitmap BlurredImage
        {
            get => blurredImage;
            set { SetField(value, ref blurredImage); BlurredImageChanged?.Invoke(); } 
        }

        public int BlurSize
        {
            get => blurSize;
            set { SetField(value, ref blurSize); Start(); }
        }


        OpenCLDevice openCLDevice;
        WriteableBitmap inputImage, blurredImage;
        ComputeKernel computeKernel;
        int blurSize;
        WriteableBitmap inputBitmap, outputBitmap;

        readonly string cSource =
            @"
                
                kernel void gaussianblur( global uchar* input, global uchar* output, int stride, int bpp, int width, int height, int size) 
                {
                    int cx = get_global_id(0);
                    int cy = get_global_id(1);

                    int fromX = cx - size;
                    int toX = cx + size + 1;
                    int fromY = cy - size;
                    int toY = cy + size + 1;
                    int sum = 0;
                    int divider = 0;

                    for (int x = fromX; x < toX; x++)
                    {
                        for (int y = fromY; y < toY; y++)
                        {
                            if (x > -1 && x < width && y > -1 && y < height)
                            {
                                int iAddr = (stride*y) + (x*bpp);
                                sum += input[iAddr];
                                divider++;
                            }
                        }
                    }

                    int result = sum / divider;

                    int oAddr = (stride*cy) + (cx*bpp);
                    output[oAddr] = (uchar)result;
                    output[oAddr+1] = (uchar)result;
                    output[oAddr+2] = (uchar)result;
                    output[oAddr+3] = 255;
                }
                
            ";


        public OCLMonochromeGaussianBlur(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;

            if(computeKernel.IsNull())
            {
                ComputeProgram computeProgram = new ComputeProgram(openCLDevice.ComputeContext, cSource);
                computeProgram.Build(null, null, null, IntPtr.Zero);
                computeKernel = computeProgram.CreateKernel("gaussianblur");
            }

            blurSize = 2;
        }

        private void Start()
        {
            if (inputImage == null)
                return;

            if (blurSize == 0)
            {
                BlurredImage = inputImage;
                return;
            }

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
            computeKernel.SetValueArgument(4, width);
            computeKernel.SetValueArgument(5, height);
            computeKernel.SetValueArgument(6, blurSize);

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
            
            BlurredImage = outputBitmap;
        }
    }
}
