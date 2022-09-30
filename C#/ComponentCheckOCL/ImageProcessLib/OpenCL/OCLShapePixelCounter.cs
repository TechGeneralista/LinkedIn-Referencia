using CommonLib;
using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLShapePixelCounter : ObservableProperty
    {
        public int PixelsCount
        {
            get => pixelsCount;
            set => SetField(value, ref pixelsCount); 
        }


        int pixelsCount;
        OpenCLDevice openCLDevice;
        static ComputeKernel computeKernel;

        readonly string cSource =
            @"
                
                __kernel void count_pixels(  __global uchar* ref, int rstride, int rbpp, __global int* cnt)
                {
                    int x = get_global_id(0);
                    int y = get_global_id(1);

                    int addr = (rstride * y) + (rbpp * x);

                    if(ref[addr] == 255)
                        atomic_inc(cnt);      
                }
                
            ";


        public OCLShapePixelCounter(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;

            if(computeKernel.IsNull())
            {
                ComputeProgram computeProgram = new ComputeProgram(openCLDevice.ComputeContext, cSource);
                computeProgram.Build(null, null, null, IntPtr.Zero);
                computeKernel = computeProgram.CreateKernel("count_pixels");
                computeProgram.Dispose();
            }
        }

        public unsafe void Start(WriteableBitmap inputImage)
        {
            if (inputImage is null)
            {
                PixelsCount = 0;
                return;
            }

            WriteableBitmap inputBitmap = inputImage.Clone();
            int iwidth = inputBitmap.PixelWidth;
            int iheight = inputBitmap.PixelHeight;
            int istride = inputBitmap.BackBufferStride;
            int ibpp = inputBitmap.Format.BitsPerPixel / 8;

            int pc = 0;
            IntPtr pcPtr = new IntPtr(&pc);

            inputBitmap.Lock();

            ComputeBuffer<byte> iBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, istride * iheight, inputBitmap.BackBuffer);
            ComputeBuffer<int> pcBuffer = new ComputeBuffer<int>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, 1, pcPtr);

            computeKernel.SetMemoryArgument(0, iBuffer);
            computeKernel.SetValueArgument(1, istride);
            computeKernel.SetValueArgument(2, ibpp);
            computeKernel.SetMemoryArgument(3, pcBuffer);
            openCLDevice.ComputeCommandQueue.Execute(computeKernel, new long[] { 0 }, new long[] { iwidth, iheight }, null, null);

            if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
                openCLDevice.ComputeCommandQueue.Finish();

            else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu)
                openCLDevice.ComputeCommandQueue.Read(pcBuffer, true, 0, 1, pcPtr, null);

            iBuffer.Dispose();
            pcBuffer.Dispose();
            inputBitmap.Unlock();

            PixelsCount = pc;
        }
    }
}
