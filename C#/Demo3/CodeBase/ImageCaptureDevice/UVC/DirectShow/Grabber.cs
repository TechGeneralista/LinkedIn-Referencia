using System;
using System.Threading;
using ImageCaptureDevice.UVC.DirectShow.Internals;


namespace ImageCaptureDevice.UVC.DirectShow
{
    public class Grabber : ISampleGrabberCB, IImageCaptureDeviceOutput
    {
        public AutoResetEvent CaptureOccurred { get; } = new AutoResetEvent(false);
        public int Width { get; private set; }
        public int Height { get; private set; }
        public IntPtr Buffer { get; private set; }
        public int BufferLength { get; private set; }
        public int BytePerPixel { get; private set; }
        public int Stride { get; private set; }


        public int SampleCB(double sampleTime, IntPtr sample)
        {
            return 0;
        }

        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
        {
            Buffer = buffer;
            BufferLength = bufferLen;
            Stride = bufferLen / Height;
            BytePerPixel = Stride / Width;

            CaptureOccurred.Set();
            return 0;
        }

        internal void SetFrameSize(BitmapInfoHeader bmiHeader)
        {
            Width = bmiHeader.Width;
            Height = bmiHeader.Height;
        }
    }
}
