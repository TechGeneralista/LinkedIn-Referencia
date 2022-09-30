using ComponentCheckApp.Views.Settings;
using ComponentCheckApp.Views.Resize;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using ComponentCheckApp.Views.ShapeFinder;
using System.Windows.Media;
using CommonLib;


namespace ComponentCheckApp.Views.DeviceSelect
{
    public class DeviceSelectViewModel
    {
        public DeviceSelectModel DeviceSelectModel { get; private set; }
        public DeviceSelectUIControlsModel DeviceSelectUIControls { get; private set; }


        public DeviceSelectViewModel()
        {
            Application.Current.Resources[ResourceKeys.DeviceSelectViewModelKey] = this;

            DeviceSelectModel = new DeviceSelectModel();
            DeviceSelectUIControls = new DeviceSelectUIControlsModel();

            DeviceSelectModel.NewImage += DeviceSelectModel_NewImage;
            DeviceSelectModel.StatusChanged += DeviceSelectUIControls.SetUIControlsByStatus;
            DeviceSelectModel.StatusChanged += DeviceSelectModel_StatusChanged;
        }

        private void DeviceSelectModel_StatusChanged(DeviceSelectModelStatus status)
        {
            MainViewModel mainViewModel = (MainViewModel)Application.Current.Resources[ResourceKeys.MainViewModelKey];

            switch (status)
            {
                case DeviceSelectModelStatus.DeviceConnected:
                    mainViewModel.StartButtonIsEnable = true;
                    break;

                case DeviceSelectModelStatus.DeviceDisconnected:
                    mainViewModel.StartButtonIsEnable = false;
                    break;
            }
        }

        public void Start()
        {
            DeviceSelectModel.ConnectedUVCDevice.Capture();
        }

        private void DeviceSelectModel_NewImage(WriteableBitmap image)
        {
            ResizeViewModel resizeViewModel = (ResizeViewModel)Application.Current.Resources[ResourceKeys.ResizeViewModelKey];

            try
            {
                resizeViewModel.NewImage(image);
            }

            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            finally
            {
                MainViewModel mainViewModel = (MainViewModel)Application.Current.Resources[ResourceKeys.MainViewModelKey];

                ShapeFinderViewModel svm = (ShapeFinderViewModel)Application.Current.Resources[ResourceKeys.ShapeFinderViewModelKey];

                if(svm.Items.Count > 0)
                {
                    if (svm.Items[0].BorderColor == Colors.Green.ToString())
                        ((SettingsViewModel)Application.Current.Resources[ResourceKeys.SettingsViewModelKey]).SaveImage(image, "_ok");
                    else
                        ((SettingsViewModel)Application.Current.Resources[ResourceKeys.SettingsViewModelKey]).SaveImage(image, "_nok");
                }

                mainViewModel.End();
            }
        }
    }
}
