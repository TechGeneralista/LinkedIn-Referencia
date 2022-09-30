using Common;
using System.Windows;
using System.Windows.Controls;
using UniCamV2.Content.Model;


namespace UniCamV2.Content.View
{
    public partial class ImageSourceV : UserControl
    {
        ImageSourceDC dc => (ImageSourceDC)DataContext;

        public ImageSourceV()
        {
            InitializeComponent();
        }

        private void ScanButtonClick(object sender, RoutedEventArgs e) => dc.Scan();
        private void StartButtonClick(object sender, RoutedEventArgs e) => dc.Connect();
        private void StopButtonClick(object sender, RoutedEventArgs e) => dc.Disconnect();
    }
}
