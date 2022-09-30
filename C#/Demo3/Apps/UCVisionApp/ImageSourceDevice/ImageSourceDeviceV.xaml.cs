using System.Windows.Controls;


namespace UCVisionApp.ImageSourceDevice
{
    /// <summary>
    /// Interaction logic for ImageSourceDeviceV.xaml
    /// </summary>
    public partial class ImageSourceDeviceV : UserControl
    {
        public ImageSourceDeviceV()
        {
            InitializeComponent();
        }

        private void AddNewInspectionButtonClick(object sender, System.Windows.RoutedEventArgs e)
            => ((ImageSourceDeviceDC)DataContext).AddNewInspectionButtonClick();

        private void RemoveSelectedInspectionButtonClick(object sender, System.Windows.RoutedEventArgs e)
            => ((ImageSourceDeviceDC)DataContext).RemoveSelectedInspectionButtonClick();

        private void RemoveAllInspectionsButtonClick(object sender, System.Windows.RoutedEventArgs e)
            => ((ImageSourceDeviceDC)DataContext).RemoveAllInspectionsButtonClick();
    }
}
