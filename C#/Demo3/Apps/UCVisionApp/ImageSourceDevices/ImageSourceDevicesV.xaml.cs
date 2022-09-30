using Common;
using System.Windows;
using System.Windows.Controls;


namespace UCVisionApp.ImageSourceDevices
{
    /// <summary>
    /// Interaction logic for ImageSourcesV.xaml
    /// </summary>
    public partial class ImageSourceDevicesV : UserControl
    {
        public ImageSourceDevicesV()
        {
            InitializeComponent();
        }

        private void DisconnectButtonClick(object sender, RoutedEventArgs e) 
            => DataContext.CastTo<ImageSourceDevicesDC>().DisconnectButtonClick();
    }
}
