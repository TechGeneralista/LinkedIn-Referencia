using System;


namespace ImageCaptureDevice
{
    public interface IImageCaptureDevice
    {
        event EventHandler<NewImageAvailableArgs> NewImageAvailable;
        event EventHandler SelectedResolutionChanged;

        string Guid { get; }
        string Name { get; }
        IImageCaptureDeviceCapability[] AvailableResolutions { get; }
        IImageCaptureDeviceCapability SelectedResolution { get; set; }


        void Start();
        void Stop();
    }
}
