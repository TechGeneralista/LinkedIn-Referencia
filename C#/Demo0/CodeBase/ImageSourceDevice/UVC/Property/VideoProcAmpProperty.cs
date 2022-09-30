using ImageSourceDevice.UVC.DirectShow;
using ImageSourceDevice.UVC.DirectShow.Internals;
using Language;


namespace ImageSourceDevice.UVC.Property
{
    public class VideoProcAmpProperty : PropertyBase
    {
        readonly LanguageDC languageDC;
        readonly VideoProcAmpProperties videoProcAmpProperty;


        public VideoProcAmpProperty(LanguageDC languageDC, VideoCaptureDevice videoCaptureDevice, VideoProcAmpProperties videoProcAmpProperty)
        {
            this.languageDC = languageDC;
            base.videoCaptureDevice = videoCaptureDevice;
            this.videoProcAmpProperty = videoProcAmpProperty;
            OriginalName = videoProcAmpProperty.ToString();
            SetName();

            videoCaptureDevice.GetVideoPropertyRange
            (
                videoProcAmpProperty,
                out int minValue,
                out int maxValue,
                out int stepSize,
                out int defaultValue,
                out ControlFlags supportedControlModes
            );

            IsSupported = true;
            MinValue = minValue;
            MaxValue = maxValue;
            StepSize = stepSize;
            DefaultValue = defaultValue;

            videoCaptureDevice.GetVideoProperty(videoProcAmpProperty, out int currentValue, out ControlFlags controlFlag);
            Current.CurrentValue = currentValue;

            switch (supportedControlModes)
            {
                case ControlFlags.None:
                case ControlFlags.Auto:
                    break;

                case ControlFlags.Manual:
                    Current.CurrentValueChanged += (o, n) => SendToCamera();
                    break;

                case ControlFlags.Auto | ControlFlags.Manual:
                    IsAutoManual = true;

                    if (controlFlag == ControlFlags.Auto)
                        IsAuto.CurrentValue = true;
                    else if (controlFlag == ControlFlags.Manual)
                        IsAuto.CurrentValue = false;

                    Current.CurrentValueChanged += (o, n) => SendToCamera();
                    IsAuto.CurrentValueChanged += (o, n) => SendToCamera();
                    break;
            }
        }

        private void SendToCamera() => videoCaptureDevice.SetVideoProperty(videoProcAmpProperty, Current.CurrentValue, IsAuto.CurrentValue ? ControlFlags.Auto : ControlFlags.Manual);

        private void SetName()
        {
            switch (videoProcAmpProperty)
            {
                case VideoProcAmpProperties.Brightness:
                    Name = languageDC.Brightness;
                    break;
                case VideoProcAmpProperties.Contrast:
                    Name = languageDC.Contrast;
                    break;
                case VideoProcAmpProperties.Hue:
                    Name = languageDC.Hue;
                    break;
                case VideoProcAmpProperties.Saturation:
                    Name = languageDC.Saturation;
                    break;
                case VideoProcAmpProperties.Sharpness:
                    Name = languageDC.Sharpness;
                    break;
                case VideoProcAmpProperties.Gamma:
                    Name = languageDC.Gamma;
                    break;
                case VideoProcAmpProperties.ColorEnable:
                    Name = languageDC.ColorEnable;
                    break;
                case VideoProcAmpProperties.WhiteBalance:
                    Name = languageDC.WhiteBalance;
                    break;
                case VideoProcAmpProperties.BacklightCompensation:
                    Name = languageDC.BacklightCompensation;
                    break;
                case VideoProcAmpProperties.Gain:
                    Name = languageDC.Gain;
                    break;
            }
        }
    }
}