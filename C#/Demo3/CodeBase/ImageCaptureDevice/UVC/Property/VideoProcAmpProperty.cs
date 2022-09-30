using Common.Language;
using ImageCaptureDevice.UVC.DirectShow;
using ImageCaptureDevice.UVC.DirectShow.Internals;


namespace ImageCaptureDevice.UVC.Property
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
            Current.Value = currentValue;

            switch (supportedControlModes)
            {
                case ControlFlags.None:
                case ControlFlags.Auto:
                    break;

                case ControlFlags.Manual:
                    Current.OnValueChanged += (o, n) => SendToCamera();
                    break;

                case ControlFlags.Auto | ControlFlags.Manual:
                    IsAutoManual = true;

                    if (controlFlag == ControlFlags.Auto)
                        IsAuto.Value = true;
                    else if (controlFlag == ControlFlags.Manual)
                        IsAuto.Value = false;

                    Current.OnValueChanged += (o, n) => SendToCamera();
                    IsAuto.OnValueChanged += (o, n) => SendToCamera();
                    break;
            }
        }

        private void SendToCamera() => videoCaptureDevice.SetVideoProperty(videoProcAmpProperty, Current.Value, IsAuto.Value ? ControlFlags.Auto : ControlFlags.Manual);

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