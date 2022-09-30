using System;
using System.Threading;
using ImageSourceDevice.UVC.DirectShow.Internals;


namespace ImageSourceDevice.UVC.DirectShow
{
    public class Grabber : ISampleGrabberCB
    {
        public AutoResetEvent AutoResetEvent { get; } = new AutoResetEvent(false);
        public int Width { get; private set; }
        public int Height { get; private set; }
        public IImageSourceDeviceOutput Output { get; private set; }


        public int SampleCB(double sampleTime, IntPtr sample)
        {
            return 0;
        }

        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
        {
            Output = new UVCDeviceOutput(Width, Height, buffer, bufferLen, 3);
            AutoResetEvent.Set();
            return 0;
        }

        internal void SetFrameSize(BitmapInfoHeader bmiHeader)
        {
            Width = bmiHeader.Width;
            Height = bmiHeader.Height;
        }
    }
}
