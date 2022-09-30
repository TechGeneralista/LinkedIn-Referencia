using CommonLib.Components;
using ComponentCheckApp.Views.DigitalZoom;
using ImageProcessLib;
using ImageProcessLib.OpenCL;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ComponentCheckApp.Views.Resize
{
    public class ResizeViewModel : ObservableProperty
    {
        public OCLResize OCLResize { get; private set; }


        public ResizeViewModel()
        {
            Application.Current.Resources[ResourceKeys.ResizeViewModelKey] = this;

            OpenCLDevice openCLDevice = (OpenCLDevice)Application.Current.Resources[ResourceKeys.OpenCLDeviceKey];
            OCLResize = new OCLResize(openCLDevice);
        }

        public void NewImage(WriteableBitmap image)
        {
            OCLResize.InputImage = image;
            ((DigitalZoomViewModel)Application.Current.Resources[ResourceKeys.DigitalZoomViewModelKey]).NewImage(OCLResize.ResizedImage);
        }
    }
}
