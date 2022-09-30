using System;


namespace ImageCaptureDevice.UniversalVideoClass
{
    public class UniversalVideoClassOutput
    {
        public readonly int Width;
        public readonly int Height;
        public readonly IntPtr Buffer;
        public readonly int BufferLength;

        public UniversalVideoClassOutput(int width, int height, IntPtr buffer, int bufferLength)
        {
            Width = width;
            Height = height;
            Buffer = buffer;
            BufferLength = bufferLength;
        }
    }
}