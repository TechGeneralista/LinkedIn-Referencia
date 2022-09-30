using Common.Language;
using ImageCaptureDevice.UVC.DirectShow;
using ImageCaptureDevice.UVC.DirectShow.Internals;


namespace ImageCaptureDevice.UVC.Property
{
    public class CameraControlProperty : PropertyBase
    {
        readonly LanguageDC languageDC;
        readonly CameraControlProperties cameraControlProperty;


        public CameraControlProperty(LanguageDC languageDC, VideoCaptureDevice videoCaptureDevice, CameraControlProperties cameraControlProperty)
        {
            this.languageDC = languageDC;
            base.videoCaptureDevice = videoCaptureDevice;
            this.cameraControlProperty = cameraControlProperty;
            OriginalName = cameraControlProperty.ToString();
            SetName();

            videoCaptureDevice.GetCameraPropertyRange
            (
                cameraControlProperty,
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

            videoCaptureDevice.GetCameraProperty(cameraControlProperty, out int currentValue, out ControlFlags controlFlag);
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

        private void SendToCamera() => videoCaptureDevice.SetCameraProperty(cameraControlProperty, Current.Value, IsAuto.Value ? ControlFlags.Auto : ControlFlags.Manual);

        private void SetName()
        {
            switch(cameraControlProperty)
            {
                case CameraControlProperties.Pan:
                    Name = languageDC.Pan;
                    break;
                case CameraControlProperties.Tilt:
                    Name = languageDC.Tilt;
                    break;
                case CameraControlProperties.Roll:
                    Name = languageDC.Roll;
                    break;
                case CameraControlProperties.Zoom:
                    Name = languageDC.Zoom;
                    break;
                case CameraControlProperties.Exposure:
                    Name = languageDC.Exposure;
                    break;
                case CameraControlProperties.Iris:
                    Name = languageDC.Iris;
                    break;
                case CameraControlProperties.Focus:
                    Name = languageDC.Focus;
                    break;
            }
        }
    }
}
