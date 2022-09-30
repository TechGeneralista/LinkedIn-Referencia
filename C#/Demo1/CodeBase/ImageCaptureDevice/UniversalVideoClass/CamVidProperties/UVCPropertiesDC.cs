using System;
using System.Collections.Generic;
using ImageCaptureDevice.UniversalVideoClass.DirectShow;
using ImageCaptureDevice.UniversalVideoClass.DirectShow.Internals;


namespace ImageCaptureDevice.UniversalVideoClass.CamVidProperties
{
    public class UVCPropertiesDC : IImageCaptureDeviceProperties
    {
        public IEnumerable<IImageCaptureDeviceProperty> ImageCaptureDeviceProperties { get; }


        public UVCPropertiesDC(VideoCaptureDevice videoCaptureDevice)
        {
            List<IImageCaptureDeviceProperty> properties = new List<IImageCaptureDeviceProperty>();

            foreach(VideoProcAmpProperties vProp in Enum.GetValues(typeof(VideoProcAmpProperties)))
                properties.Add(new VideoProcAmpProperty(videoCaptureDevice, vProp));

            foreach (CameraControlProperties cProp in Enum.GetValues(typeof(CameraControlProperties)))
                properties.Add(new CameraControlProperty(videoCaptureDevice, cProp));

            ImageCaptureDeviceProperties = properties;
        }

        public void ResetAllToDefault()
        {
            foreach (IImageCaptureDeviceProperty prop in ImageCaptureDeviceProperties)
                prop.SetToDefault();
        }
    }
}
