using System.Windows;
using System.Windows.Controls;


namespace UniCamApp.Source
{
    /// <summary>
    /// Interaction logic for SourceV.xaml
    /// </summary>
    public partial class SourceV : UserControl
    {
        public SourceV()
        {
            InitializeComponent();
        }

        private void ScanUVCDevicesButtonClick(object sender, RoutedEventArgs e) => ((SourceDC)DataContext).ScanUVCDevicesButtonClick();
        private void CreateInstanceWithSelectedUVCDeviceButtonClick(object sender, RoutedEventArgs e) => ((SourceDC)DataContext).CreateInstanceWithSelectedUVCDeviceButtonClick();
        private void RemoveSelectedInstanceButtonClick(object sender, RoutedEventArgs e) => ((SourceDC)DataContext).RemoveSelectedInstanceButtonClick();
    }
}
