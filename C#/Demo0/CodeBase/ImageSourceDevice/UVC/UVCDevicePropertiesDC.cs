using Common.Settings;
using ImageSourceDevice.UVC.DirectShow;
using ImageSourceDevice.UVC.DirectShow.Internals;
using ImageSourceDevice.UVC.Property;
using Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ImageSourceDevice.UVC
{
    public class UVCDevicePropertiesDC : IImageSourceDeviceProperties, ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public IImageSourceProperty[] ImageSourceProperties { get; }


        public UVCDevicePropertiesDC(LanguageDC languageDC, VideoCaptureDevice videoCaptureDevice)
        {
            LanguageDC = languageDC;
            List<IImageSourceProperty> temp = new List<IImageSourceProperty>();

            foreach(VideoProcAmpProperties vProp in Enum.GetValues(typeof(VideoProcAmpProperties)))
                temp.Add(new VideoProcAmpProperty(languageDC, videoCaptureDevice, vProp));

            foreach (CameraControlProperties cProp in Enum.GetValues(typeof(CameraControlProperties)))
                temp.Add(new CameraControlProperty(languageDC, videoCaptureDevice, cProp));

            ImageSourceProperties = temp.ToArray();
        }

        public void ResetAllToDefault()
        {
            foreach (IImageSourceProperty prop in ImageSourceProperties)
                prop.SetToDefault();
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(UVCDevicePropertiesDC));

            StringBuilder stringBuilder = new StringBuilder();
            foreach (IImageSourceProperty imageSourceProperty in ImageSourceProperties)
            {
                if (imageSourceProperty.IsSupported)
                {
                    stringBuilder.Append(imageSourceProperty.OriginalName);
                    imageSourceProperty.SaveSettings(settingsCollection);

                    if (imageSourceProperty != ImageSourceProperties.Last())
                        stringBuilder.Append(',');
                }
            }

            settingsCollection.KeyCreator.AddNew(nameof(ImageSourceProperties));
            settingsCollection.SetValue(stringBuilder.ToString());
            settingsCollection.KeyCreator.RemoveLast(2);
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(UVCDevicePropertiesDC));
            settingsCollection.KeyCreator.AddNew(nameof(ImageSourceProperties));
            string imageSourceProperties = settingsCollection.GetValue<string>();
            settingsCollection.KeyCreator.RemoveLast();
            string[] propertyNames = imageSourceProperties.Split(',');

            foreach (string s in propertyNames)
            {
                foreach (IImageSourceProperty imageSourceProperty in ImageSourceProperties)
                {
                    if (imageSourceProperty.OriginalName == s)
                    {
                        imageSourceProperty.LoadSettings(settingsCollection);
                        break;
                    }
                }
            }

            settingsCollection.KeyCreator.RemoveLast();
        }
    }
}
