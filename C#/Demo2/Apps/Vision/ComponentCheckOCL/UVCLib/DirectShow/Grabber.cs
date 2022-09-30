using UVCLib.DirectShow.Internals;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Diagnostics;

namespace UVCLib.DirectShow
{
    public class Grabber : ISampleGrabberCB
    {
        public event Action<WriteableBitmap> NewImageEvent, NewVideoImageEvent;


        public bool VideoStreamEnable;
        public bool MakeSnapshot;
        public int Width;
        public int Height;
        public bool CallBackMethodIsCalled;


        public int SampleCB(double sampleTime, IntPtr sample)
        {
            return 0;
        }

        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
        {
            if(VideoStreamEnable || MakeSnapshot)
            {
                WriteableBitmap capturedImage = CreateWriteableBitmap(buffer, bufferLen);

                if(MakeSnapshot)
                {
                    NewImageEvent?.Invoke(capturedImage);
                    MakeSnapshot = false;
                }

                if(VideoStreamEnable)
                    NewVideoImageEvent?.Invoke(capturedImage);
            }

            CallBackMethodIsCalled = true;
            return 0;
        }

        private unsafe WriteableBitmap CreateWriteableBitmap(IntPtr buffer, int bufferLen)
        {
            PixelFormat iPixelFormat = PixelFormats.Bgr24;
            int iStride = (Width * PixelFormats.Bgr24.BitsPerPixel + 7) / 8;
            int iBpp = PixelFormats.Bgr24.BitsPerPixel / 8;
            byte* iScan0 = (byte*)buffer.ToPointer();

            PixelFormat oPixelFormat = PixelFormats.Bgra32;
            WriteableBitmap writeableBitmap = new WriteableBitmap(Width, Height, 96, 96, oPixelFormat, null);
            int oStride = writeableBitmap.BackBufferStride;
            int oBpp = oPixelFormat.BitsPerPixel / 8;
            byte* oScan0 = (byte*)writeableBitmap.BackBuffer.ToPointer();
            int width1 = Width - 1;
            int height1 = Height - 1;

            writeableBitmap.Lock();

            Parallel.For(0, Height, y =>
            {
                for (int x = 0; x < Width; x++)
                {
                    byte* iAddress = null;

                    if (y == 0 && Height == 1080)
                        iAddress = iScan0 + ((y + 1) * iStride) + (x * iBpp);
                    else
                        iAddress = iScan0 + (y * iStride) + (x * iBpp);

                    byte* oAddress = oScan0 + ((height1 - y) * oStride) + ((width1 - x) * oBpp);

                    *(oAddress) = *(iAddress);
                    *(oAddress + 1) = *(iAddress + 1);
                    *(oAddress + 2) = *(iAddress + 2);
                    *(oAddress + 3) = 255;
                }
            });

            writeableBitmap.Unlock();
            writeableBitmap.Freeze();
            return writeableBitmap;
        }

        //private unsafe WriteableBitmap CreateWriteableBitmap(IntPtr buffer, int bufferLen)
        //{
        //    PixelFormat pixelFormat = PixelFormats.Bgr24;
        //    int stride = (Width * PixelFormats.Bgr24.BitsPerPixel + 7) / 8;

        //    BitmapSource bitmapSource = BitmapSource.Create
        //        (
        //            Width, Height - 10, 96, 96,
        //            pixelFormat,
        //            null,
        //            buffer,
        //            bufferLen,
        //            stride
        //        );

        //    bitmapSource = new TransformedBitmap(bitmapSource, new ScaleTransform(1, -1));
        //    bitmapSource = new FormatConvertedBitmap(bitmapSource, PixelFormats.Bgra32, null, 0);
        //    WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);
        //    writeableBitmap.Freeze();

        //    return writeableBitmap;
        //}
    }
}
