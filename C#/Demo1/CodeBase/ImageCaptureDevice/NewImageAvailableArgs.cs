using ImageCaptureDevice.UniversalVideoClass;
using System;


namespace ImageCaptureDevice
{
    public class NewImageAvailableArgs : EventArgs
    {
        public object ImageData { get; }


        public NewImageAvailableArgs(int width, int height, IntPtr buffer, int bufferLength)
        {
            ImageData = new UniversalVideoClassOutput(width, height, buffer, bufferLength);
        }
    }
}