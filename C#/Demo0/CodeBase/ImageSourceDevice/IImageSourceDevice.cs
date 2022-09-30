using Common.Settings;


namespace ImageSourceDevice
{
    public enum ImageSourceDeviceTypes { UVC }

    public interface IImageSourceDevice
    {
        ImageSourceDeviceTypes Type { get; }
        string Resolution { get; }
        int FrameRate { get; }
        string Location { get; }
        string Address { get; }
        IImageSourceDeviceProperties Properties { get; }
        IImageSourceDeviceOutput Output { get; }


        void Start();
        void Capture();
        void Stop();
    }
}
