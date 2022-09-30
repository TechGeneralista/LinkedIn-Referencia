using System;


namespace ImageCaptureDevice.Interfaces
{
    public interface IHasStartDevice
    {
        event Action<IImageCaptureDevice> StartDevice;
    }
}
