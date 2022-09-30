using CommonLib.Components;
using ComponentCheckApp.Views.ShapeFinder;
using ImageProcessLib.OpenCL;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ComponentCheckApp.Views.EdgeDetector
{
    public class EdgeDetectorViewModel : ObservableProperty
    {
        public OCLMonochrome OCLMonochrome { get; private set; }
        public OCLMonochromeGaussianBlur OCLMonochromeGaussianBlur { get; private set; }
        public OCLMonochromeSobel OCLMonochromeSobel { get; private set; }
        public OCLMonochromeNonMaxSuppression OCLMonochromeNonMaxSuppression { get; private set; }
        public OCLMonochromeDoubleThreshold OCLMonochromeDoubleThreshold { get; private set; }
        public OCLMonochromeEdgeTrackingWithHysteresis OCLMonochromeEdgeTrackingWithHysteresis { get; private set; }
        public OCLMonochromeWeakPixelRemove OCLMonochromeWeakPixelRemove { get; private set; }


        public EdgeDetectorViewModel()
        {
            Application.Current.Resources[ResourceKeys.EdgeDetectorViewModelKey] = this;

            OpenCLDevice openCLDevice = (OpenCLDevice)Application.Current.Resources[ResourceKeys.OpenCLDeviceKey];
            OCLMonochrome = new OCLMonochrome(openCLDevice);
            OCLMonochromeGaussianBlur = new OCLMonochromeGaussianBlur(openCLDevice);
            OCLMonochromeSobel = new OCLMonochromeSobel(openCLDevice);
            OCLMonochromeNonMaxSuppression = new OCLMonochromeNonMaxSuppression(openCLDevice);
            OCLMonochromeDoubleThreshold = new OCLMonochromeDoubleThreshold(openCLDevice);
            OCLMonochromeEdgeTrackingWithHysteresis = new OCLMonochromeEdgeTrackingWithHysteresis(openCLDevice);
            OCLMonochromeWeakPixelRemove = new OCLMonochromeWeakPixelRemove(openCLDevice);

            OCLMonochromeGaussianBlur.BlurredImageChanged += OCLMonochromeGaussianBlur_BlurredImageChanged;
            OCLMonochromeDoubleThreshold.ThresholdChanged += OCLMonochromeDoubleThreshold_ThresholdChanged;
        }

        private void OCLMonochromeDoubleThreshold_ThresholdChanged()
        {
            OCLMonochromeDoubleThreshold.InputImage = OCLMonochromeNonMaxSuppression.OutputImage;
            OCLMonochromeEdgeTrackingWithHysteresis.Start(OCLMonochromeDoubleThreshold.OutputImage);
            OCLMonochromeWeakPixelRemove.Start(OCLMonochromeEdgeTrackingWithHysteresis.OutputImage);
        }

        private void OCLMonochromeGaussianBlur_BlurredImageChanged()
        {
            OCLMonochromeSobel.InputImage = OCLMonochromeGaussianBlur.BlurredImage;
            OCLMonochromeNonMaxSuppression.Start(OCLMonochromeSobel.OutputImage, OCLMonochromeSobel.Magnitude);
            OCLMonochromeDoubleThreshold_ThresholdChanged();
        }

        public void NewImage(WriteableBitmap image)
        {
            OCLMonochrome.InputImage = image;
            OCLMonochromeGaussianBlur.InputImage = OCLMonochrome.MonochromeImage;
            OCLMonochromeGaussianBlur_BlurredImageChanged();

            ((ShapeFinderViewModel)Application.Current.Resources[ResourceKeys.ShapeFinderViewModelKey]).NewImage(OCLMonochromeWeakPixelRemove.OutputImage);
        }
    }
}
