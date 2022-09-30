using Common.NotifyProperty;
using Common.Settings;


namespace ImageSourceDevice
{
    public interface IImageSourceProperty : ICanSaveLoadSettings
    {
        string OriginalName { get; }
        INonSettableObservableProperty<string> Name { get; }
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
