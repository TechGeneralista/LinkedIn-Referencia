using System;
using System.Windows.Media.Imaging;


namespace ImageSourceDevice
{
    public enum ImageSourceDeviceOutputTypes { IntPtr, WriteableBitmap }

    public interface IImageSourceDeviceOutput
    {
        ImageSourceDeviceOutputTypes Type { get; }

        int Width { get; }
        int Height { get; }
        IntPtr Buffer { get; }
        int BufferLength { get; }
        int BytePerPixel { get; }
        int Stride { get; }
        WriteableBitmap Bitmap { get; }
    }
}
