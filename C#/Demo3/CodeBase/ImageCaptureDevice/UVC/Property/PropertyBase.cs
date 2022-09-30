using Common.NotifyProperty;
using Common.Settings;
using ImageCaptureDevice.UVC.DirectShow;


namespace ImageCaptureDevice.UVC.Property
{
    public class PropertyBase : IImageCaptureDeviceProperty, ICanSaveLoadSettings
    {
        public string OriginalName { get; protected set; }
        public IReadOnlyProperty<string> Name { get; protected set; }
        public bool IsSupported { get; protected set; }
        public bool IsAutoManual { get; protected set; }
        public int MinValue { get; protected set; }
        public int DefaultValue { get; protected set; }
        public int MaxValue { get; protected set; }
        public int StepSize { get; protected set; }

        public IProperty<bool> IsAuto { get; } = new Property<bool>();
        public IProperty<int> Current { get; } = new Property<int>();


        protected VideoCaptureDevice videoCaptureDevice;

        public void SetToDefault()
        {
            if (!IsSupported)
                return;

            if (IsAutoManual)
                IsAuto.Value = true;

            Current.Value = DefaultValue;
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(OriginalName);

            if(IsSupported)
            {
                if (IsAutoManual)
                    settingsCollection.SetProperty(IsAuto.Value, nameof(IsAuto));

                settingsCollection.SetProperty(Current.Value, nameof(Current));
            }

            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(OriginalName);

            if (IsSupported)
            {
                if (IsAutoManual)
                    IsAuto.Value = settingsCollection.GetProperty<bool>(nameof(IsAuto));

                Current.Value = settingsCollection.GetProperty<int>(nameof(Current));
            }

            settingsCollection.ExitPoint();
        }
    }
}
