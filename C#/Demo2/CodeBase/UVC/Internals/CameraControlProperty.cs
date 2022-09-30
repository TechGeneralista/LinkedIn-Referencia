using Common.NotifyProperty;
using UVC.DirectShow;


namespace UVC.Internals
{
    public enum CameraControlProperties { Pan = 0, Tilt, Roll, Zoom, Exposure, Iris, Focus }

    public class CameraControlProperty : PropBase
    {
        readonly CameraControlProperties propertyName;


        public CameraControlProperty(VideoCaptureDevice videoCaptureDevice, CameraControlProperties propertyName)
        {
            base.videoCaptureDevice = videoCaptureDevice;
            this.propertyName = propertyName;
            Name = propertyName.ToString();

            videoCaptureDevice.GetCameraPropertyRange
            (
                propertyName,
                out int minValue,
                out int maxValue,
                out int stepSize,
                out int defaultValue,
                out ControlFlags supportedControlModes
            );

            switch(supportedControlModes)
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
                    Current.ValueChanged += (o,n) => Set();
                    IsAuto.ValueChanged += (o,n) => Set();
                    break;
            }

            IsSupported = true;
            MinValue = minValue;
            MaxValue = maxValue;
            StepSize = stepSize;
            DefaultValue = defaultValue;

            videoCaptureDevice.GetCameraProperty(propertyName, out int currentValue, out ControlFlags controlFlag);
            Current.Value = currentValue;
        }

        private void Set() => videoCaptureDevice.SetCameraProperty(propertyName, Current.Value, IsAuto.Value ? ControlFlags.Auto : ControlFlags.Manual);
    }
}
