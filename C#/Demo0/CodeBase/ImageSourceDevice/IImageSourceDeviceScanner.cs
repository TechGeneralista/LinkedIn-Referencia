namespace ImageSourceDevice
{
    public interface IImageSourceDeviceScanner
    {
        IImageSourceDevice[] AvailableDevices { get; }

        void Scan();
    }
}