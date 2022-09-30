using System;
using System.Windows.Media.Imaging;


namespace ImageSourceDevice.UVC
{
    public class UVCDeviceOutput : IImageSourceDeviceOutput
    {
        public ImageSourceDeviceOutputTypes Type { get; } = ImageSourceDeviceOutputTypes.IntPtr;

        public int Width { get; }
        public int Height { get; }
        public IntPtr Buffer { get; }
        public int BufferLength { get; }
        public int BytePerPixel { get; }
        public int Stride { get; }

        public WriteableBitmap Bitmap { get; }


        public UVCDeviceOutput(int width, int height, IntPtr buffer, int bufferLength, int bytePerPixel)
        {
            Width = width;
            Height = height;
            Buffer = buffer;
            BufferLength = bufferLength;
            BytePerPixel = bytePerPixel;
            Stride = bufferLength / height;
        }
    }
}
