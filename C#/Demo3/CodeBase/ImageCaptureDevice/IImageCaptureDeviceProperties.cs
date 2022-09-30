namespace ImageCaptureDevice
{
    public interface IImageCaptureDeviceProperties
    {
        IImageCaptureDeviceProperty[] ImageCaptureDeviceProperties { get; }

        void ResetAllToDefault();
    }
}
