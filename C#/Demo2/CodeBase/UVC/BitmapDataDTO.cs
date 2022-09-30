using System;
using System.Windows.Media;


namespace UVC
{
    public struct BitmapDataDTO
    {
        public readonly int Width;
        public readonly int Height;
        public readonly PixelFormat Format;
        public readonly int Stride;
        public readonly int BytePerPixel;
        public readonly IntPtr Buffer;
        public readonly int BufferLength;

        public unsafe BitmapDataDTO(int width, int height, PixelFormat format, IntPtr buffer, int bufferLength)
        {
            Width = width;
            Height = height;
            Format = format;
            Stride = bufferLength / height;
            BytePerPixel = format.BitsPerPixel / 8;
            Buffer = buffer;
            BufferLength = bufferLength;
        }
    }
}
