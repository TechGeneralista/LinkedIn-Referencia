using ImageCaptureDevice.UniversalVideoClass.DirectShow;
using ImageCaptureDevice.UniversalVideoClass.DirectShow.Internals;


namespace ImageCaptureDevice.UniversalVideoClass.CamVidProperties
{
    public class VideoProcAmpProperty : PropertyBase, IImageCaptureDeviceProperty
    {
        readonly VideoProcAmpProperties videoProcAmpProperty;


        public VideoProcAmpProperty(VideoCaptureDevice videoCaptureDevice, VideoProcAmpProperties videoProcAmpProperty)
        {
            this.videoCaptureDevice = videoCaptureDevice;
            this.videoProcAmpProperty = videoProcAmpProperty;
            OriginalName = videoProcAmpProperty.ToString();

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
            => videoCaptureDevice.SetVideoProperty(videoProcAmpProperty, Current, IsAuto ? ControlFlags.Auto : ControlFlags.Manual);
    }
}