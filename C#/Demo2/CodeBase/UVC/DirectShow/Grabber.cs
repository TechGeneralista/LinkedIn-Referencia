using UVC.DirectShow.Internals;
using System;
using System.Threading;
using System.Windows.Media;


namespace UVC.DirectShow
{
    public class Grabber : ISampleGrabberCB
    {
        public BitmapDataDTO Frame { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int MakeSnapshoot;


        public int SampleCB(double sampleTime, IntPtr sample)
        {
            return 0;
        }

        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
        {
            if (MakeSnapshoot == 0)
                return 0;

            Frame = new BitmapDataDTO(Width, Height, PixelFormats.Bgr24, buffer, bufferLen);
            Interlocked.Exchange(ref MakeSnapshoot, 0);

            return 0;
        }
    }
}
