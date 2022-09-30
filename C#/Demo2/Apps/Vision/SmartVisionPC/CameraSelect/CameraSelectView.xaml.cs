using SmartVisionClientApp.Common;
using System.Windows;
using System.Windows.Controls;


namespace SmartVisionClientApp.CameraSelect
{
    /// <summary>
    /// Interaction logic for CameraSelectView.xaml
    /// </summary>
    public partial class CameraSelectView : UserControl
    {
        public CameraSelectView()
        {
            InitializeComponent();
            DataContext = ObjectContainer.Get<CameraSelectViewModel>();
        }

        private void RefreshAvailableCamerasButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<CameraSelectViewModel>().RefreshAvailableCamerasButtonClick();
        private void ConnectButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<CameraSelectViewModel>().ConnectButtonClick();
        private void DisconnectButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<CameraSelectViewModel>().DisconnectButtonClick();
    }
}
