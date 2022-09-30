using System;
using System.Collections.Generic;
using UVC.DirectShow;


namespace UVC.Internals
{
    public class DevicePropertiesDC
    {
        public IImageSourceProperty[] ImageSourceProperties { get; }


        public DevicePropertiesDC(VideoCaptureDevice videoCaptureDevice)
        {
            List<IImageSourceProperty> temp = new List<IImageSourceProperty>();

            foreach(VideoProcAmpProperties vProp in Enum.GetValues(typeof(VideoProcAmpProperties)))
                temp.Add(new VideoProcAmpProperty(videoCaptureDevice, vProp));

            foreach (CameraControlProperties cProp in Enum.GetValues(typeof(CameraControlProperties)))
                temp.Add(new CameraControlProperty(videoCaptureDevice, cProp));

            ImageSourceProperties = temp.ToArray();
        }

        internal void ResetAllToDefault()
        {
            foreach (IImageSourceProperty prop in ImageSourceProperties)
                prop.SetToDefault();
        }
    }
}
