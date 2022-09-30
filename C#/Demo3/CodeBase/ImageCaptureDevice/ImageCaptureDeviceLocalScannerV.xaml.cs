using Common;
using Common.Interfaces;
using System.Windows;
using System.Windows.Controls;


namespace ImageCaptureDevice
{
    public partial class ImageCaptureDeviceLocalScannerV : UserControl
    {
        public ImageCaptureDeviceLocalScannerV()
        {
            InitializeComponent();
        }

        private void ScanButtonClick(object sender, RoutedEventArgs e) 
            => DataContext.CastTo<ICanScan>().ScanButtonClick();

        private void ConnectButtonClick(object sender, RoutedEventArgs e) 
            => DataContext.CastTo<ICanConnect>().ConnectButtonClick();
    }
}
