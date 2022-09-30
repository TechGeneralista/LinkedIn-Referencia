using System;
using ImageCaptureDevice.UniversalVideoClass.DirectShow.Internals;


namespace ImageCaptureDevice.UniversalVideoClass.DirectShow
{
    public class Grabber : ISampleGrabberCB
    {
        public event EventHandler<NewImageAvailableArgs> NewImageAvailable;


        int width;
        int height;


        public int SampleCB(double sampleTime, IntPtr sample)
        {
            return 0;
        }

        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
        {
            NewImageAvailable?.Invoke(this, new NewImageAvailableArgs(width, height, buffer, bufferLen));
            return 0;
        }

        internal void SetFrameSize(BitmapInfoHeader bmiHeader)
        {
            width = bmiHeader.Width;
            height = bmiHeader.Height;
        }
    }
}
