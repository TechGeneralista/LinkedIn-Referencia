using Common.NotifyProperty;
using Common.Settings;
using ImageSourceDevice.UVC.DirectShow;


namespace ImageSourceDevice.UVC.Property
{
    public class PropertyBase : IImageSourceProperty
    {
        public string OriginalName { get; protected set; }
        public INonSettableObservableProperty<string> Name { get; protected set; }
        public bool IsSupported { get; protected set; }
        public bool IsAutoManual { get; protected set; }
        public int MinValue { get; protected set; }
        public int DefaultValue { get; protected set; }
        public int MaxValue { get; protected set; }
        public int StepSize { get; protected set; }

        public ISettableObservableProperty<bool> IsAuto { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<int> Current { get; } = new ObservableProperty<int>();


        protected VideoCaptureDevice videoCaptureDevice;

        public void SetToDefault()
        {
            if (!IsSupported)
                return;

            if (IsAutoManual)
                IsAuto.CurrentValue = true;

            Current.CurrentValue = DefaultValue;
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(OriginalName);
            settingsCollection.KeyCreator.AddNew(nameof(IsAutoManual));
            settingsCollection.SetValue(IsAutoManual);
            settingsCollection.KeyCreator.ReplaceLast(nameof(IsAuto));
            settingsCollection.SetValue(IsAuto.CurrentValue);
            settingsCollection.KeyCreator.ReplaceLast(nameof(Current));
            settingsCollection.SetValue(Current.CurrentValue);
            settingsCollection.KeyCreator.RemoveLast(2);
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(OriginalName);
            settingsCollection.KeyCreator.AddNew(nameof(IsAutoManual));
            IsAutoManual = settingsCollection.GetValue<bool>();

            if (IsAutoManual)
            {
                settingsCollection.KeyCreator.ReplaceLast(nameof(IsAuto));
                IsAuto.CurrentValue = settingsCollection.GetValue<bool>();
            }

            settingsCollection.KeyCreator.ReplaceLast(nameof(Current));
            Current.CurrentValue = settingsCollection.GetValue<int>();
            settingsCollection.KeyCreator.RemoveLast(2);
        }
    }
}
