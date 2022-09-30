using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLCrop : ObservableProperty
    {
        public WriteableBitmap InputImage
        {
            get => inputImage;
            set => SetField(value, ref inputImage);
        }

        public WriteableBitmap CroppedImage
        {
            get => croppedImage;
            private set => SetField(value, ref croppedImage);
        }


        WriteableBitmap inputImage, croppedImage;
        OpenCLDevice openCLDevice;
        ComputeProgram computeProgram;
        ComputeKernel computeKernel;

        readonly string cSource =
            @"
                
                kernel void crop(   global uchar* input, int istride, int ibpp, 
                                    global uchar* output, int ostride, int obpp,
                                    int offsetx, int offsety) 
                {
                    int ox = get_global_id(0);
                    int oy = get_global_id(1);

                    int iAddr = (istride*(oy+offsety)) + ((ox+offsetx)*ibpp);
                    int oAddr = (ostride*oy) + (ox*obpp);

                    output[oAddr] = input[iAddr];
                    output[oAddr+1] = input[iAddr+1];
                    output[oAddr+2] = input[iAddr+2];
                    output[oAddr+3] = input[iAddr+3];
                }
                
            ";

        public OCLCrop(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;
            computeProgram = new ComputeProgram(openCLDevice.ComputeContext, cSource);
            computeProgram.Build(null, null, null, IntPtr.Zero);
            computeKernel = computeProgram.CreateKernel("crop");
        }

        ~OCLCrop()
        {
            computeProgram.Dispose();
            computeKernel.Dispose();
        }

        public void Start(Int32Rect rect)
        {
            if (inputImage is null)
                return;

            WriteableBitmap iwriteableBitmap = inputImage.Clone();
            int istride = iwriteableBitmap.BackBufferStride;
            int ibpp = iwriteableBitmap.Format.BitsPerPixel / 8;
            int iheight = iwriteableBitmap.PixelHeight;

            WriteableBitmap owriteableBitmap = new WriteableBitmap(rect.Width, rect.Height, 96, 96, iwriteableBitmap.Format, null);
            int ostride = owriteableBitmap.BackBufferStride;
            int obpp = owriteableBitmap.Format.BitsPerPixel / 8;

            iwriteableBitmap.Lock();
            owriteableBitmap.Lock();

            ComputeBuffer<byte> iBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, istride * iheight, iwriteableBitmap.BackBuffer);
            ComputeBuffer<byte> oBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.UseHostPointer, ostride * rect.Height, owriteableBitmap.BackBuffer);

            computeKernel.SetMemoryArgument(0, iBuffer);
            computeKernel.SetValueArgument(1, istride);
            computeKernel.SetValueArgument(2, ibpp);
            computeKernel.SetMemoryArgument(3, oBuffer);
            computeKernel.SetValueArgument(4, ostride);
            computeKernel.SetValueArgument(5, obpp);
            computeKernel.SetValueArgument(6, rect.X);
            computeKernel.SetValueArgument(7, rect.Y);
            openCLDevice.ComputeCommandQueue.Execute(computeKernel, new long[] { 0 }, new long[] { rect.Width, rect.Height }, null, null);

            if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
                openCLDevice.ComputeCommandQueue.Finish();

            else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu)
                openCLDevice.ComputeCommandQueue.Read(oBuffer, true, 0, ostride * rect.Height, owriteableBitmap.BackBuffer, null);

            iBuffer.Dispose();
            oBuffer.Dispose();

            owriteableBitmap.AddDirtyRect(new Int32Rect(0, 0, rect.Width, rect.Height));
            iwriteableBitmap.Unlock();
            owriteableBitmap.Unlock();
            owriteableBitmap.Freeze();
            CroppedImage = owriteableBitmap;
        }
    }
}
