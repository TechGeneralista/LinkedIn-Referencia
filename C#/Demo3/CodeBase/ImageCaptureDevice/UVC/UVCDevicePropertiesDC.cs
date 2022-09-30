using Common.Language;
using Common.Settings;
using ImageCaptureDevice.UVC.DirectShow;
using ImageCaptureDevice.UVC.DirectShow.Internals;
using ImageCaptureDevice.UVC.Property;
using System;
using System.Collections.Generic;


namespace ImageCaptureDevice.UVC
{
    public class UVCDevicePropertiesDC : IImageCaptureDeviceProperties, ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public IImageCaptureDeviceProperty[] ImageCaptureDeviceProperties { get; }


        public UVCDevicePropertiesDC(LanguageDC languageDC, VideoCaptureDevice videoCaptureDevice)
        {
            LanguageDC = languageDC;
            List<IImageCaptureDeviceProperty> temp = new List<IImageCaptureDeviceProperty>();

            foreach(VideoProcAmpProperties vProp in Enum.GetValues(typeof(VideoProcAmpProperties)))
                temp.Add(new VideoProcAmpProperty(languageDC, videoCaptureDevice, vProp));

            foreach (CameraControlProperties cProp in Enum.GetValues(typeof(CameraControlProperties)))
                temp.Add(new CameraControlProperty(languageDC, videoCaptureDevice, cProp));

            ImageCaptureDeviceProperties = temp.ToArray();
        }

        public void ResetAllToDefault()
        {
            foreach (IImageCaptureDeviceProperty prop in ImageCaptureDeviceProperties)
                prop.SetToDefault();
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);

            foreach (IImageCaptureDeviceProperty imageCaptureDeviceProperty in ImageCaptureDeviceProperties)
                ((ICanSaveLoadSettings)imageCaptureDeviceProperty).SaveSettings(settingsCollection);

            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);

            foreach (IImageCaptureDeviceProperty imageCaptureDeviceProperty in ImageCaptureDeviceProperties)
                ((ICanSaveLoadSettings)imageCaptureDeviceProperty).LoadSettings(settingsCollection);

            settingsCollection.ExitPoint();
        }
    }
}
