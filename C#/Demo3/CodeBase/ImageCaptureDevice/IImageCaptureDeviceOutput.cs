using System;


namespace ImageCaptureDevice
{
    public interface IImageCaptureDeviceOutput
    {
        int Width { get; }
        int Height { get; }
        IntPtr Buffer { get; }
        int BufferLength { get; }
        int BytePerPixel { get; }
        int Stride { get; }
    }
}
