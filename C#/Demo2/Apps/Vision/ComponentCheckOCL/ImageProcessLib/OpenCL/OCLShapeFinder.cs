using CommonLib;
using CommonLib.Components;
using ImageProcessLib.OpenCL.Compute;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ImageProcessLib.OpenCL
{
    public class OCLShapeFinder : ObservableProperty
    {
        public Point Position
        {
            get => position;
            set => SetField(value, ref position);
        }

        public int MatchedPixelsCount
        {
            get => matchedPixelsCount;
            set => SetField(value, ref matchedPixelsCount);
        }


        Point position;
        int matchedPixelsCount;
        OpenCLDevice openCLDevice;
        static ComputeKernel computeKernel;

        readonly string contourFindSource =
            @"
                
                int getIAddr(int stride, int offsetX, int offsetY, int x, int y, int bpp)
                {
                    return (stride * (offsetY + y)) + (bpp * (offsetX + x));
                }

                int valid(global uchar* input, int stride, int offsetX, int offsetY, int x, int y, int bpp)
                {
                    if( input[getIAddr(stride, offsetX, offsetY, x  , y  , bpp)] == 255 ||
                        input[getIAddr(stride, offsetX, offsetY, x-1, y  , bpp)] == 255 ||
                        input[getIAddr(stride, offsetX, offsetY, x+1, y  , bpp)] == 255 ||
                        input[getIAddr(stride, offsetX, offsetY, x  , y-1, bpp)] == 255 ||
                        input[getIAddr(stride, offsetX, offsetY, x  , y+1, bpp)] == 255 ||
                        input[getIAddr(stride, offsetX, offsetY, x+1, y+1, bpp)] == 255 ||
                        input[getIAddr(stride, offsetX, offsetY, x-1, y-1, bpp)] == 255 ||
                        input[getIAddr(stride, offsetX, offsetY, x+1, y-1, bpp)] == 255 ||
                        input[getIAddr(stride, offsetX, offsetY, x-1, y+1, bpp)] == 255)
                        return 1;

                    return 0;
                }

                kernel void find_contour(  global uchar* inp, int istride, int ibpp,
                                                  global uchar* ref, int rstride, int rbpp, int rwidth, int rheight,
                                                  global int* match, int mWidth)
                {
                    int iox = get_global_id(0);
                    int ioy = get_global_id(1);

                    int maddr = ( mWidth * ioy ) + iox;
                    
                    for(int ry=0;ry<rheight;ry++)
                    {
                        for(int rx=0;rx<rwidth;rx++)
                        {
                            int raddr = (rstride * ry) + (rbpp * rx);

                            if(ref[raddr] == 255 && valid(inp, istride, iox, ioy, rx, ry, ibpp))
                                atomic_inc(&match[maddr]);
                        }
                    }
                }
                
            ";


        public OCLShapeFinder(OpenCLDevice openCLDevice)
        {
            this.openCLDevice = openCLDevice;

            if (computeKernel.IsNull())
            {
                ComputeProgram computeProgram = new ComputeProgram(openCLDevice.ComputeContext, contourFindSource);
                
                try
                {
                    computeProgram.Build(null, null, null, IntPtr.Zero);
                }
                catch
                {
                    Debug.WriteLine(computeProgram.GetBuildLog(openCLDevice.ComputeDevice));
                }

                computeKernel = computeProgram.CreateKernel("find_contour");
                computeProgram.Dispose();
            }
        }

        public unsafe void Start(WriteableBitmap referenceImage, WriteableBitmap inputImage)
        {
            if (inputImage is null || referenceImage is null)
            {
                return;
            }

            if (referenceImage.PixelWidth >= inputImage.PixelWidth || referenceImage.PixelHeight >= inputImage.PixelHeight)
            {
                return;
            }

            WriteableBitmap inputBitmap = inputImage.Clone();
            int iwidth = inputBitmap.PixelWidth;
            int iheight = inputBitmap.PixelHeight;
            int istride = inputBitmap.BackBufferStride;
            int ibpp = inputBitmap.Format.BitsPerPixel / 8;

            WriteableBitmap referenceBitmap = referenceImage.Clone();
            int rwidth = referenceBitmap.PixelWidth;
            int rheight = referenceBitmap.PixelHeight;
            int rstride = referenceBitmap.BackBufferStride;
            int rbpp = referenceBitmap.Format.BitsPerPixel / 8;

            int maximumXoffset = iwidth - rwidth;
            int maximumYoffset = iheight - rheight;

            int[] matchResultArray = new int[maximumXoffset * maximumYoffset];
            GCHandle pinnedMatchResultArray = GCHandle.Alloc(matchResultArray, GCHandleType.Pinned);
            IntPtr ptr = pinnedMatchResultArray.AddrOfPinnedObject();

            inputBitmap.Lock();
            referenceBitmap.Lock();

            ComputeBuffer<byte> iBuffer = null;
            ComputeBuffer<byte> rBuffer = null;
            ComputeBuffer<int> mrBuffer = null;

            if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
            {
                iBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, istride * iheight, inputBitmap.BackBuffer);
                rBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, rstride * rheight, referenceBitmap.BackBuffer);
                mrBuffer = new ComputeBuffer<int>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, matchResultArray.Length, ptr);
            }

            else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu)
            {
                iBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, istride * iheight, inputBitmap.BackBuffer);
                rBuffer = new ComputeBuffer<byte>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, rstride * rheight, referenceBitmap.BackBuffer);
                mrBuffer = new ComputeBuffer<int>(openCLDevice.ComputeContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, matchResultArray.Length, ptr);
            }

            computeKernel.SetMemoryArgument(0, iBuffer);
            computeKernel.SetValueArgument(1, istride);
            computeKernel.SetValueArgument(2, ibpp);
            computeKernel.SetMemoryArgument(3, rBuffer);
            computeKernel.SetValueArgument(4, rstride);
            computeKernel.SetValueArgument(5, rbpp);
            computeKernel.SetValueArgument(6, rwidth);
            computeKernel.SetValueArgument(7, rheight);
            computeKernel.SetMemoryArgument(8, mrBuffer);
            computeKernel.SetValueArgument(9, maximumXoffset);
            openCLDevice.ComputeCommandQueue.Execute(computeKernel, null, new long[] { maximumXoffset, maximumYoffset }, null, null);

            if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Cpu)
                openCLDevice.ComputeCommandQueue.Finish();

            else if (openCLDevice.ComputeCommandQueue.Device.Type == ComputeDeviceTypes.Gpu)
                openCLDevice.ComputeCommandQueue.Read(mrBuffer, true, 0, matchResultArray.Length, ptr, null);

            pinnedMatchResultArray.Free();
            iBuffer.Dispose();
            rBuffer.Dispose();
            mrBuffer.Dispose();

            inputBitmap.Unlock();
            referenceBitmap.Unlock();

            int maxValue = 0;
            int maxIndex = 0;

            for (int i = 0; i < matchResultArray.Length; i++)
            {
                if (matchResultArray[i] > maxValue)
                {
                    maxValue = matchResultArray[i];
                    maxIndex = i;
                }
            }

            MatchedPixelsCount = maxValue;
            int row = maxIndex / maximumXoffset;
            Position = new Point(maxIndex - (row * maximumXoffset), row);
        }
    }
}
