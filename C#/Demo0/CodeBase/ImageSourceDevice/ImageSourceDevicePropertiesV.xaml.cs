using System.Windows;
using System.Windows.Controls;


namespace ImageSourceDevice
{
    /// <summary>
    /// Interaction logic for DevicePropertiesView.xaml
    /// </summary>
    public partial class ImageSourceDevicePropertiesV : UserControl
    {
        public ImageSourceDevicePropertiesV()
        {
            InitializeComponent();
        }

        private void SetAllToDefaultButtonClick(object sender, RoutedEventArgs e) => ((IImageSourceDeviceProperties)DataContext).ResetAllToDefault();
    }
}
