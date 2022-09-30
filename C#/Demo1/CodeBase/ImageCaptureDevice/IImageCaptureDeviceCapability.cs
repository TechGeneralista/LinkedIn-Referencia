using System.Drawing;


namespace ImageCaptureDevice
{
    public interface IImageCaptureDeviceCapability
    {
        int Index { get; }
        Size FrameSize { get; }
        int AverageFrameRate { get; }
        int PixelsCount { get; }
        string FrameSizeString { get; }
    }
}
