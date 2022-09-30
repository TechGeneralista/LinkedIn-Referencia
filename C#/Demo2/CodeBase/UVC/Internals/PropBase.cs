using UVC.DirectShow;
using System;
using Common.NotifyProperty;


namespace UVC.Internals
{
    [Flags]
    public enum ControlFlags { None = 0, Auto, Manual }

    public class PropBase : IImageSourceProperty
    {
        public string Name { get; protected set; }
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
                IsAuto.Value = true;

            Current.Value = DefaultValue;
        }
    }
}
