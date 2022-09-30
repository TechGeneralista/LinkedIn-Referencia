using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLDraw : ObservableProperty
    {
        public WriteableBitmap InputImage
        {
            get => inputImage;
            set => SetField(value, ref inputImage);
        }

        public WriteableBitmap DrawedImage
        {
            get => drawedImage;
            private set => SetField(value, ref drawedImage);
        }


        WriteableBitmap inputImage, drawedImage;
        OpenCLDevice openCLDevice;
        ComputeProgram computeProgram;
        ComputeKernel computeKernel;
        WriteableBitmap writeableBitmap;
        ComputeBuffer<byte> dataBuffer;
        int width, height, stride, bpp;

        readonly string pixelSource =
            @"
                
                kernel void pixel( global uchar* data, int stride, int bpp,
                                        uchar r, uchar g, uchar b, uchar a) 
                {
                    int x = get_global_id(0);
                    int y = get_global_id(1);
                    
                    int addr = (stride * y) + (x * bpp);

                    int diff_b = b - data[addr];
                    int diff_g = g - data[addr + 1];
                    int diff_r = r - data[addr + 2];

                    int delta_b = (diff_b * (int)a) / (int)255;
                    int delta_g = (diff_g * (int)a) / (int)255;
                    int delta_r = (diff_r * (int)a) / (int)255;

                    data[addr] = (uchar)(data[addr] + delta_b);
                    data[addr+1] = (uchar)(data[addr + 1] + delta_g);
                    data[addr+2] = (uchar)(data[addr + 2] + delta_r);
                    data[addr+3] = (uchar)255; 
                }
                
            ";

        public OCLDraw(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;

            computeProgram = new ComputeProgram(openCLDevice.ComputeContext, pixelSource);
            computeProgram.Build(null, null, null, IntPtr.Zero);
            computeKernel = computeProgram.CreateKernel("pixel");
        }

        ~OCLDraw()
        {
            computeProgram.Dispose();
            computeKernel.Dispose();
        }

        public void Start()
        {
            if (inputImage == null)
                return;

            writeableBitmap = inputImage.Clone();
            width = writeableBitmap.PixelWidth;
            height = writeableBitmap.PixelHeight;
            stride = writeableBitmap.BackBufferStride;
            bpp = writeableBitmap.Format.BitsPerPixel / 8;
            writeableBitmap.Lock();

            if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
                dataBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, stride * height, writeableBitmap.BackBuffer);

            else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu)
                dataBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, stride * height, writeableBitmap.BackBuffer);
        }

        public void Pixel(int x, int y, Color color)
        {
            if (inputImage == null)
                return;

            if (x < 0 || y < 0 || x > (width - 1) || y > (height - 1))
                return;

            computeKernel.SetMemoryArgument(0, dataBuffer);
            computeKernel.SetValueArgument(1, stride);
            computeKernel.SetValueArgument(2, bpp);
            computeKernel.SetValueArgument(3, color.R);
            computeKernel.SetValueArgument(4, color.G);
            computeKernel.SetValueArgument(5, color.B);
            computeKernel.SetValueArgument(6, color.A);

            openCLDevice.ComputeCommandQueue.Execute(computeKernel, new long[] { x, y}, new long[] { 1,1 }, null, null);
        }

        public void Rectangle(Int32Rect rect, Color color)
        {
            if (inputImage == null)
                return;

            computeKernel.SetMemoryArgument(0, dataBuffer);
            computeKernel.SetValueArgument(1, stride);
            computeKernel.SetValueArgument(2, bpp);
            computeKernel.SetValueArgument(3, color.R);
            computeKernel.SetValueArgument(4, color.G);
            computeKernel.SetValueArgument(5, color.B);
            computeKernel.SetValueArgument(6, color.A);

            openCLDevice.ComputeCommandQueue.Execute(computeKernel, new long[] { rect.X,rect.Y }, new long[] { rect.Width, rect.Height }, null, null);
        }

        public void End()
        {
            if (inputImage == null)
                return;

            if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
                openCLDevice.ComputeCommandQueue.Finish();

            else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu) 
                openCLDevice.ComputeCommandQueue.Read(dataBuffer, true, 0, stride * height, writeableBitmap.BackBuffer, null);
            
            dataBuffer.Dispose();
            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            writeableBitmap.Unlock();
            writeableBitmap.Freeze();
            DrawedImage = writeableBitmap;
        }
    }
}
