namespace ImageSourceDevice
{
    public interface IImageSourceDeviceProperties
    {
        IImageSourceProperty[] ImageSourceProperties { get; }

        void ResetAllToDefault();
    }
}
