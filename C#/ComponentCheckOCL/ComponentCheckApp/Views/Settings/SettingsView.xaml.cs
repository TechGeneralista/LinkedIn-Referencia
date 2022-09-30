using System.Windows.Controls;

namespace ComponentCheckApp.Views.Settings
{
    /// <summary>
    /// Interaction logic for ImageBackupView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        SettingsViewModel vm => (SettingsViewModel)DataContext;

        public SettingsView()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }

        private void OpenFolderButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.OpenFolderButtonClick();
        }

        private void BrowseButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.BrowseButtonClick();
        }

        private void DefaultButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.DefaultButtonClick();
        }
    }
}
