using Common;
using System.Windows;
using System.Windows.Controls;


namespace ImageCaptureDevice
{
    /// <summary>
    /// Interaction logic for DevicePropertiesView.xaml
    /// </summary>
    public partial class ImageCaptureDevicePropertiesV : UserControl
    {
        public ImageCaptureDevicePropertiesV()
        {
            InitializeComponent();
        }

        private void SetAllToDefaultButtonClick(object sender, RoutedEventArgs e) 
            => DataContext.CastTo<IImageCaptureDevice>().Properties.ResetAllToDefault();
    }
}
