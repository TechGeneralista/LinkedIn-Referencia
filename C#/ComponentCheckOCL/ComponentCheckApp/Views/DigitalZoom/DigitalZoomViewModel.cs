using CommonLib.Components;
using CommonLib.Types;
using ComponentCheckApp.Views.EdgeDetector;
using ImageProcessLib.Components;
using ImageProcessLib.OpenCL;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ComponentCheckApp.Views.DigitalZoom
{
    public class DigitalZoomViewModel : ObservableProperty
    {
        public OCLDraw OCLDraw { get; private set; }
        public SelectionRectangle SelectionRectangle { get; }
        public OCLCrop OCLCrop { get; private set; }


        public DigitalZoomViewModel()
        {
            Application.Current.Resources[ResourceKeys.DigitalZoomViewModelKey] = this;

            SelectionRectangle = new SelectionRectangle();
            SelectionRectangle.PositionOrSizeChanged += Refresh;

            OpenCLDevice openCLDevice = (OpenCLDevice)Application.Current.Resources[ResourceKeys.OpenCLDeviceKey];
            OCLCrop = new OCLCrop(openCLDevice);
            OCLDraw = new OCLDraw(openCLDevice);
        }

        public void NewImage(WriteableBitmap image)
        {
            SelectionRectangle.CheckPosition(new Int32Size(image.PixelWidth, image.PixelHeight));
            OCLCrop.InputImage = image;
            OCLDraw.InputImage = image;
            Refresh();
            ((EdgeDetectorViewModel)Application.Current.Resources[ResourceKeys.EdgeDetectorViewModelKey]).NewImage(OCLCrop.CroppedImage);
        }

        private void Refresh()
        {
            OCLCrop.Start(SelectionRectangle.Int32Rect);

            OCLDraw.Start();
            OCLDraw.Rectangle(SelectionRectangle.Int32Rect, Color.FromArgb(128, 255, 0, 0));
            OCLDraw.End();
        }
    }
}