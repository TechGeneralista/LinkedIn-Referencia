using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace SmartVisionClientApp.Common
{
    public static class Utils
    {
        public static void ShowErrorAndShutdown(string msg, Exception ex)
        {
            MessageBox.Show(msg + ": " + ex.Message, "Hiba:", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown(-1);
        }

        public static void ShowError(string msg) => MessageBox.Show(msg, "Hiba:", MessageBoxButton.OK, MessageBoxImage.Error);

        public static BitmapSource GetBlackImage()
        {
            WriteableBitmap blackImage = new WriteableBitmap(4, 3, 96, 96, PixelFormats.Bgr24, null);
            blackImage.Freeze();
            return blackImage;
        }
    }
}
