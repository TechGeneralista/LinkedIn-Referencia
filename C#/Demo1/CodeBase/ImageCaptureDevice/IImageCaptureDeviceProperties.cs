using System.Collections.Generic;


namespace ImageCaptureDevice
{
    public interface IImageCaptureDeviceProperties
    {
        IEnumerable<IImageCaptureDeviceProperty> ImageCaptureDeviceProperties { get; }

        void ResetAllToDefault();
    }
}
