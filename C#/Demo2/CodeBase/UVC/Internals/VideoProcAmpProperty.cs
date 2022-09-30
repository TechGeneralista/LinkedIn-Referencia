using Common.NotifyProperty;
using System;
using UVC.DirectShow;


namespace UVC.Internals
{
    public enum VideoProcAmpProperties { Brightness = 0, Contrast, Hue, Saturation, Sharpness, Gamma, ColorEnable, WhiteBalance, BacklightCompensation, Gain }

    public class VideoProcAmpProperty : PropBase
    {
        readonly VideoProcAmpProperties propertyName;


        public VideoProcAmpProperty(VideoCaptureDevice videoCaptureDevice, VideoProcAmpProperties propertyName)
        {
            this.videoCaptureDevice = videoCaptureDevice;
            this.propertyName = propertyName;
            Name = propertyName.ToString();

            videoCaptureDevice.GetVideoPropertyRange
            (
                propertyName,
                out int minValue,
                out int maxValue,
                out int stepSize,
                out int defaultValue,
                out ControlFlags supportedControlModes
            );

            switch (supportedControlModes)
            {
                case ControlFlags.None:
                    IsSupported = false;
                    IsAutoManual = false;
                    IsAuto.Value = false;
                    return;

                case ControlFlags.Auto:
                    IsAutoManual = false;
                    IsAuto.Value = true;
                    break;

                case ControlFlags.Manual:
                    IsAutoManual = false;
                    IsAuto.Value = false;
                    Current.ValueChanged += (o,n) => Set();
                    break;

                case ControlFlags.Auto | ControlFlags.Manual:
                    IsAutoManual = true;
                    IsAuto.Value = true;
                    Current.ValueChanged += (o, n) => Set();
                    IsAuto.ValueChanged += (o, n) => Set();
                    break;
            }

            IsSupported = true;
            MinValue = minValue;
            MaxValue = maxValue;
            StepSize = stepSize;
            DefaultValue = defaultValue;

            videoCaptureDevice.GetVideoProperty(propertyName, out int currentValue, out ControlFlags controlFlag);
            Current.Value = currentValue;
        }

        private void Set() => videoCaptureDevice.SetVideoProperty(propertyName, Current.Value, IsAuto.Value ? ControlFlags.Auto : ControlFlags.Manual);
    }
}