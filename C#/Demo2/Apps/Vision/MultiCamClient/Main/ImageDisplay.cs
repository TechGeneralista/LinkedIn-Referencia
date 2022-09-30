using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace MultiCamApp.Main
{
    public class ImageDisplay : ObservableProperty
    {
        public BitmapSource Image
        {
            get => image;
            set
            {
                if (value == null)
                    SetField(blackImage, ref image);
                else
                    SetField(value, ref image);
            }
        }


        BitmapSource image;
        BitmapSource blackImage => new WriteableBitmap(16, 9, 96, 96, PixelFormats.Bgr24, null);


        public ImageDisplay()
        {
            image = blackImage;
        }
    }
}