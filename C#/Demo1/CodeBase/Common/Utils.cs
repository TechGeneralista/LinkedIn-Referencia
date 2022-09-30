using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Common
{
    public static class Utils
    {
        public static void InvokeIfNecessary(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                action.Invoke();
            else
                Application.Current.Dispatcher.Invoke(action);
        }

        public static WriteableBitmap GetNewWriteableBitmapBgra32(int width, int height)
            => new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

        public static void ShowErrorMessageBox(string message)
        {
            InvokeIfNecessary(() => 
            {
                MessageBox.Show(Application.Current.MainWindow, message, "Hiba:", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public static string CreateNewGuid(int length)
            => Guid.NewGuid().ToString().Replace("-", string.Empty).ToUpper().Substring(0, length);
    }
}
