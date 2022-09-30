using UVCLib.DirectShow;
using UVCLib.Components.Internals;
using System.Windows.Media.Imaging;
using CommonLib.Components;


namespace UVCLib.Components
{
    public class PropertiesModel : ObservableProperty
    {
        public VideoProcAmpProperty Brightness { get; private set; }
        public VideoProcAmpProperty Contrast { get; private set; }
        public VideoProcAmpProperty Hue { get; private set; }
        public VideoProcAmpProperty Saturation { get; private set; }
        public VideoProcAmpProperty Sharpness { get; private set; }
        public VideoProcAmpProperty Gamma { get; private set; }
        public VideoProcAmpProperty ColorEnable { get; private set; }
        public VideoProcAmpProperty WhiteBalance { get; private set; }
        public VideoProcAmpProperty BacklightCompensation { get; private set; }
        public VideoProcAmpProperty Gain { get; private set; }

        public CameraControlProperty Pan { get; private set; }
        public CameraControlProperty Tilt { get; private set; }
        public CameraControlProperty Roll { get; private set; }
        public CameraControlProperty Zoom { get; private set; }
        public CameraControlProperty Exposure { get; private set; }
        public CameraControlProperty Iris { get; private set; }
        public CameraControlProperty Focus { get; private set; }
        public WriteableBitmap Frame
        {
            get => frame;
            set => SetField(value, ref frame);
        }


        WriteableBitmap frame;


        public PropertiesModel(VideoCaptureDevice videoCaptureDevice)
        {
            Brightness = new VideoProcAmpProperty(videoCaptureDevice, VideoProcAmpProperties.Brightness);
            Contrast = new VideoProcAmpProperty(videoCaptureDevice, VideoProcAmpProperties.Contrast);
            Hue = new VideoProcAmpProperty(videoCaptureDevice, VideoProcAmpProperties.Hue);
            Saturation = new VideoProcAmpProperty(videoCaptureDevice, VideoProcAmpProperties.Saturation);
            Sharpness = new VideoProcAmpProperty(videoCaptureDevice, VideoProcAmpProperties.Sharpness);
            Gamma = new VideoProcAmpProperty(videoCaptureDevice, VideoProcAmpProperties.Gamma);
            ColorEnable = new VideoProcAmpProperty(videoCaptureDevice, VideoProcAmpProperties.ColorEnable);
            WhiteBalance = new VideoProcAmpProperty(videoCaptureDevice, VideoProcAmpProperties.WhiteBalance);
            BacklightCompensation = new VideoProcAmpProperty(videoCaptureDevice, VideoProcAmpProperties.BacklightCompensation);
            Gain = new VideoProcAmpProperty(videoCaptureDevice, VideoProcAmpProperties.Gain);

            Pan = new CameraControlProperty(videoCaptureDevice, CameraControlProperties.Pan);
            Tilt = new CameraControlProperty(videoCaptureDevice, CameraControlProperties.Tilt);
            Roll = new CameraControlProperty(videoCaptureDevice, CameraControlProperties.Roll);
            Zoom = new CameraControlProperty(videoCaptureDevice, CameraControlProperties.Zoom);
            Exposure = new CameraControlProperty(videoCaptureDevice, CameraControlProperties.Exposure);
            Iris = new CameraControlProperty(videoCaptureDevice, CameraControlProperties.Iris);
            Focus = new CameraControlProperty(videoCaptureDevice, CameraControlProperties.Focus);
        }


        public void SetDefaultValues()
        {
            Brightness.SetDefaultValue();
            Contrast.SetDefaultValue();
            Hue.SetDefaultValue();
            Saturation.SetDefaultValue();
            Sharpness.SetDefaultValue();
            Gamma.SetDefaultValue();
            ColorEnable.SetDefaultValue();
            WhiteBalance.SetDefaultValue();
            BacklightCompensation.SetDefaultValue();
            Gain.SetDefaultValue();

            Pan.SetDefaultValue();
            Tilt.SetDefaultValue();
            Roll.SetDefaultValue();
            Zoom.SetDefaultValue();
            Exposure.SetDefaultValue();
            Iris.SetDefaultValue();
            Focus.SetDefaultValue();
        }
    }
}
