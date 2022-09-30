using ImageCaptureDevice.UniversalVideoClass.DirectShow;
using ImageCaptureDevice.UniversalVideoClass.DirectShow.Internals;


namespace ImageCaptureDevice.UniversalVideoClass.CamVidProperties
{
    public class CameraControlProperty : PropertyBase, IImageCaptureDeviceProperty
    {
        readonly CameraControlProperties cameraControlProperty;


        public CameraControlProperty(VideoCaptureDevice videoCaptureDevice, CameraControlProperties cameraControlProperty)
        {
            this.videoCaptureDevice = videoCaptureDevice;
            this.cameraControlProperty = cameraControlProperty;
            OriginalName = cameraControlProperty.ToString();

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
            Current = currentValue;

            switch (supportedControlModes)
            {
                case ControlFlags.None:
                case ControlFlags.Auto:
                case ControlFlags.Manual:
                    break;

                case ControlFlags.Auto | ControlFlags.Manual:
                    IsAutoManual = true;

                    if (controlFlag == ControlFlags.Auto)
                        IsAuto = true;
                    else if (controlFlag == ControlFlags.Manual)
                        IsAuto = false;

                    break;
            }
        }

        protected override void SendToCamera() 
            => videoCaptureDevice.SetCameraProperty(cameraControlProperty, Current, IsAuto ? ControlFlags.Auto : ControlFlags.Manual);
    }
}
