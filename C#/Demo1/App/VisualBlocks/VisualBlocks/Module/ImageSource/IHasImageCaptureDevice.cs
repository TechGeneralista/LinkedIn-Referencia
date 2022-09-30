using ImageCaptureDevice;


namespace VisualBlocks.Module.ImageSource
{
    internal interface IHasImageCaptureDevice
    {
        IImageCaptureDevice ImageCaptureDevice { get; }
    }
}
