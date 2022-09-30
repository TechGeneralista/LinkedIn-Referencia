namespace ImageCaptureDevice
{
    public interface IImageCaptureDeviceResolutionInfo
    {
        int Width { get; }
        int Height { get; }
        int FrameRate { get; }
        int Pixels { get; }
        string Text { get; }
    }
}
