namespace ImageCaptureDevice
{
    public interface IImageCaptureDeviceProperty
    {
        string OriginalName { get; }
        bool IsSupported { get; }
        bool IsAutoManual { get; }
        bool IsAuto { get; set; }
        int MinValue { get; }
        int DefaultValue { get; }
        int MaxValue { get; }
        int StepSize { get; }
        int Current { get; set; }

        void SetToDefault();
    }
}
