using Common.NotifyProperty;


namespace UVC
{
    public interface IImageSourceProperty
    {
        string Name { get; }
        bool IsSupported { get; }
        bool IsAutoManual { get; }
        ISettableObservableProperty<bool> IsAuto { get; }
        int MinValue { get; }
        int DefaultValue { get; }
        int MaxValue { get; }
        int StepSize { get; }
        ISettableObservableProperty<int> Current { get; }

        void SetToDefault();
    }
}
