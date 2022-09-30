using System;


namespace ImageCaptureDevice.UniversalVideoClass.DirectShow.Internals
{
    [Flags]
    public enum ControlFlags { None = 0, Auto, Manual }
    public enum VideoProcAmpProperties { Brightness = 0, Contrast, Hue, Saturation, Sharpness, Gamma, ColorEnable, WhiteBalance, BacklightCompensation, Gain }
    public enum CameraControlProperties { Pan = 0, Tilt, Roll, Zoom, Exposure, Iris, Focus }
}
