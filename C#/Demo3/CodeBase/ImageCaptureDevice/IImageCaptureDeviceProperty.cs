using Common.NotifyProperty;


namespace ImageCaptureDevice
{
    public interface IImageCaptureDeviceProperty
    {
        string OriginalName { get; }
        IReadOnlyProperty<string> Name { get; }
        bool IsSupported { get; }
        bool IsAutoManual { get; }
        IProperty<bool> IsAuto { get; }
        int MinValue { get; }
        int DefaultValue { get; }
        int MaxValue { get; }
        int StepSize { get; }
        IProperty<int> Current { get; }

        void SetToDefault();
    }
}
