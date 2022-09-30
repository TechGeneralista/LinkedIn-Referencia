using System;
using System.Windows.Media.Imaging;



namespace CommonLib.Interfaces
{
    public interface IVideoCaptureDevice
    {
        event Action<WriteableBitmap> NewImageEvent;
        event Action ImagePropertiesChangedEvent;

        string Name { get; }
        string Resolution { get; }
        int Brightness { get; set; }
        int Contrast { get; set; }
        int Hue { get; set; }
        int Saturation { get; set; }
        int Sharpness { get; set; }
        int Gamma { get; set; }
        int ColorEnable { get; set; }
        int WhiteBalance { get; set; }
        int BacklightCompensation { get; set; }
        int Gain { get; set; }
        int Pan { get; set; }
        int Tilt { get; set; }
        int Roll { get; set; }
        int Zoom { get; set; }
        int Exposure { get; set; }
        int Iris { get; set; }
        int Focus { get; set; }
        bool IsRunning { get; }

        void Connect();
        void ShowImageProperties();
        void Capture();
        void Disconnect();
    }
}
