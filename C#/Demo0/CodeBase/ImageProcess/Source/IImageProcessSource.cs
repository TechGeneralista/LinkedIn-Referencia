using Common.NotifyProperty;
using ImageProcess.Buffer;
using System.Windows.Media.Imaging;


namespace ImageProcess.Source
{
    public interface IImageProcessSource
    {
        Image2DBuffer ColorImageBuffer { get; }
        Image2DBuffer MonochromeImageBuffer { get; }
        INonSettableObservableProperty<WriteableBitmap> ColorImage { get; }
        INonSettableObservableProperty<WriteableBitmap> MonochromeImage { get; }

        void Refresh();
    }
}