using Common.NotifyProperty;
using ImageProcess.Buffer;
using ImageProcess.Templates;
using OpenCLWrapper;
using System;
using System.Windows.Media.Imaging;


namespace ImageProcess.Source
{
    public class ImageProcessSourceDC : IImageProcessSource
    {
        public Image2DBuffer ColorImageBuffer { get; private set; }
        public Image2DBuffer MonochromeImageBuffer => monochrome.Output;
        public INonSettableObservableProperty<WriteableBitmap> ColorImage { get; } = new ObservableProperty<WriteableBitmap>();
        public INonSettableObservableProperty<WriteableBitmap> MonochromeImage { get; } = new ObservableProperty<WriteableBitmap>();


        readonly Monochrome monochrome;
        readonly Action refreshMethod;


        public ImageProcessSourceDC(OpenCLAccelerator openCLAccelerator, Action refreshMethod)
        {
            monochrome = new Monochrome(openCLAccelerator);
            this.refreshMethod = refreshMethod;
        }

        public void Prepare(Image2DBuffer source)
        {
            ColorImageBuffer = source;
            ColorImage.ForceSet(ColorImageBuffer.Upload());
            monochrome.Convert(ColorImageBuffer);
            MonochromeImage.ForceSet(monochrome.Output.Upload());
        }

        public void Refresh() => refreshMethod?.Invoke();
    }
}
