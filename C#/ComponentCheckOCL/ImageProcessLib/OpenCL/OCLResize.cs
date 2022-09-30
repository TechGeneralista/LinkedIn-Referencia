using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLResize : ObservableProperty
    {
        public WriteableBitmap InputImage
        {
            get => inputImage;
            set { SetField(value, ref inputImage); Start(); }
        }

        public WriteableBitmap ResizedImage
        {
            get => resizedImage;
            private set => SetField(value, ref resizedImage);
        }

        public double ScaleValue
        {
            get => scaleValue;
            set { SetField(value, ref scaleValue); Start(); }
        }


        WriteableBitmap inputImage, resizedImage;
        double scaleValue;
        OpenCLDevice openCLDevice;
        ComputeProgram computeProgram;
        ComputeKernel computeKernel;
        WriteableBitmap iwriteableBitmap, owriteableBitmap;


        readonly string cSource =
            @"
                kernel void scale( global uchar* input, int istride, int ibpp, 
                                global uchar* output, int ostride, int obpp, float scaleValue) 
                {
                    int ox = get_global_id(0);
                    int oy = get_global_id(1);

                    int ix = (int)((float)ox * ((float)100/scaleValue));
                    int iy = (int)((float)oy * ((float)100/scaleValue));

                    int iAddr = (istride*iy) + (ix*ibpp);
                    int oAddr = (ostride*oy) + (ox*obpp);

                    output[oAddr] = input[iAddr];
                    output[oAddr+1] = input[iAddr+1];
                    output[oAddr+2] = input[iAddr+2];
                    output[oAddr+3] = input[iAddr+3];
                }
                
            ";


        public OCLResize(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;

            computeProgram = new ComputeProgram(openCLDevice.ComputeContext, cSource);

            try
            {
                computeProgram.Build(null, null, null, IntPtr.Zero);
            }
            catch
            {
                MessageBox.Show(computeProgram.GetBuildLog(openCLDevice.ComputeDevice));
            }


            computeKernel = computeProgram.CreateKernel("scale");
            scaleValue = 100;
        }

        public void Start()
        {
            if (inputImage is null)
                return;

            if (scaleValue == 100)
            {
                ResizedImage = inputImage;
                return;
            }

            iwriteableBitmap = inputImage.Clone();
            int istride = iwriteableBitmap.BackBufferStride;
            int ibpp = iwriteableBitmap.Format.BitsPerPixel / 8;
            int iwidth = iwriteableBitmap.PixelWidth;
            int iheight = iwriteableBitmap.PixelHeight;

            int owidth = (int)((double)iwidth / (100d / scaleValue));
            int oheight = (int)((double)iheight / (100d / scaleValue));

            if (owriteableBitmap is null || owriteableBitmap.PixelWidth != owidth || owriteableBitmap.PixelHeight != oheight)
                owriteableBitmap = new WriteableBitmap(owidth, oheight, 96, 96, iwriteableBitmap.Format, null);
            else
                owriteableBitmap = owriteableBitmap.Clone();

            int ostride = owriteableBitmap.BackBufferStride;
            int obpp = owriteableBitmap.Format.BitsPerPixel / 8;

            iwriteableBitmap.Lock();
            owriteableBitmap.Lock();

            ComputeBuffer<byte> iBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, istride * iheight, iwriteableBitmap.BackBuffer);
            ComputeBuffer<byte> oBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.UseHostPointer, ostride * oheight, owriteableBitmap.BackBuffer);

            computeKernel.SetMemoryArgument(0, iBuffer);
            computeKernel.SetValueArgument(1, istride);
            computeKernel.SetValueArgument(2, ibpp);
            computeKernel.SetMemoryArgument(3, oBuffer);
            computeKernel.SetValueArgument(4, ostride);
            computeKernel.SetValueArgument(5, obpp);
            computeKernel.SetValueArgument(6, (float)scaleValue);

            openCLDevice.ComputeCommandQueue.Execute(computeKernel, new long[] { 0 }, new long[] { owidth, oheight }, null, null);

            if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
                openCLDevice.ComputeCommandQueue.Finish();

            else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu)
                openCLDevice.ComputeCommandQueue.Read(oBuffer, true, 0, ostride * oheight, owriteableBitmap.BackBuffer, null);

            iBuffer.Dispose();
            oBuffer.Dispose();

            owriteableBitmap.AddDirtyRect(new Int32Rect(0, 0, owidth, oheight));
            iwriteableBitmap.Unlock();
            owriteableBitmap.Unlock();
            owriteableBitmap.Freeze();
            ResizedImage = owriteableBitmap;
        }
    }
}
