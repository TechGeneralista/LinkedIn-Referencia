using Common.NotifyProperty;


namespace ImageCaptureDevice
{
    public enum ImageSourceDeviceTypes { UVC }

    public interface IImageCaptureDevice
    {
        ImageSourceDeviceTypes Type { get; }
        string Id { get; }
        string Name { get; }
        IReadOnlyPropertyArray<IImageCaptureDeviceResolutionInfo> AvailableResolutions { get; }
        IProperty<IImageCaptureDeviceResolutionInfo> SelectedResolution { get; }
        IImageCaptureDeviceProperties Properties { get; }
        IImageCaptureDeviceOutput Output { get; }
        IReadOnlyProperty<bool> IsRun { get; }


        void Start();
        void Capture();
        void Stop();
    }
}
