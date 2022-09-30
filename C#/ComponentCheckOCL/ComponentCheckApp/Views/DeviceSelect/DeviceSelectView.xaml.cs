using System.Windows;
using System.Windows.Controls;


namespace ComponentCheckApp.Views.DeviceSelect
{
    /// <summary>
    /// Interaction logic for UVCDeviceSelectControl.xaml
    /// </summary>
    public partial class DeviceSelectView : UserControl
    {
        DeviceSelectViewModel vm => (DeviceSelectViewModel)DataContext;

        public DeviceSelectView()
        {
            InitializeComponent();
            DataContext = new DeviceSelectViewModel();
        }

        private void ScanButtonClick(object sender, RoutedEventArgs e)
        {
            vm.DeviceSelectModel.ScanAsync();
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            vm.DeviceSelectModel.ConnectAsync();
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            vm.DeviceSelectModel.ShowSettings();
        }

        private void DisconnectButtonClick(object sender, RoutedEventArgs e)
        {
            vm.DeviceSelectModel.DisconnectAsync();
        }
    }
}
