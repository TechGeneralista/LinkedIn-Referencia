using CommonLib;
using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLMonochromeEdgeTrackingWithHysteresis : ObservableProperty
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

                int getIAddr(int stride, int x, int y, int bpp)
                {
                    return (stride * y) + (bpp * x);
                }
                
                kernel void edgeTrackingHysteresis(global uchar* output, global uchar* setted, int stride, int bpp) 
                {
                    int x = get_global_id(0);
                    int y = get_global_id(1);

                    int addr = getIAddr(stride, x, y, bpp);

                    if(output[addr] == 64)
                    {
                        if( 
                            output[getIAddr(stride, x-1, y  , bpp)] == 255 ||
                            output[getIAddr(stride, x+1, y  , bpp)] == 255 ||
                            output[getIAddr(stride, x  , y-1, bpp)] == 255 ||
                            output[getIAddr(stride, x  , y+1, bpp)] == 255 ||
                            output[getIAddr(stride, x+1, y+1, bpp)] == 255 ||
                            output[getIAddr(stride, x-1, y-1, bpp)] == 255 ||
                            output[getIAddr(stride, x+1, y-1, bpp)] == 255 ||
                            output[getIAddr(stride, x-1, y+1, bpp)] == 255
                          )
                        {
                            output[addr] =
                            output[addr+1] =
                            output[addr+2] =
                            output[addr+3] = 255;

                            setted[0] = 1;
                        }
                    }
                }
                
            ";

        public OCLMonochromeEdgeTrackingWithHysteresis(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;

            if(computeKernel.IsNull())
            {
                ComputeProgram computeProgram = new ComputeProgram(openCLDevice.ComputeContext, cSource);
                computeProgram.Build(null, null, null, IntPtr.Zero);
                computeKernel = computeProgram.CreateKernel("edgeTrackingHysteresis");
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
            byte[] setted = new byte[1];
            GCHandle handle = GCHandle.Alloc(setted, GCHandleType.Pinned);

            inputImage.Lock();

            ComputeBuffer<byte> buffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, stride * height, inputImage.BackBuffer);
            ComputeBuffer<byte> settedBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.UseHostPointer, 1, handle.AddrOfPinnedObject());

            computeKernel.SetMemoryArgument(0, buffer);
            computeKernel.SetMemoryArgument(1, settedBuffer);
            computeKernel.SetValueArgument(2, stride);
            computeKernel.SetValueArgument(3, bpp);

            do
            {
                setted[0] = 0;
                openCLDevice.ComputeCommandQueue.Write(settedBuffer, true, 0, 1, handle.AddrOfPinnedObject(), null);
                openCLDevice.ComputeCommandQueue.Execute(computeKernel, new long[] { 1,1 }, new long[] { width-1, height-1 }, null, null);

                if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
                    openCLDevice.ComputeCommandQueue.Finish();

                else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu)
                    openCLDevice.ComputeCommandQueue.Read(settedBuffer, true, 0, 1, handle.AddrOfPinnedObject(), null);

            } while (setted[0] == 1);

            openCLDevice.ComputeCommandQueue.Read(buffer, true, 0, stride * height, inputImage.BackBuffer, null);

            buffer.Dispose();
            settedBuffer.Dispose();
            handle.Free();

            inputImage.Unlock();
            inputImage.Freeze();
            OutputImage = inputImage;
        }
    }
}
