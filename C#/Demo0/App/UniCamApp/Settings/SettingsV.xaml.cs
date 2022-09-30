using System.Windows.Controls;


namespace UniCamApp.Settings
{
    /// <summary>
    /// Interaction logic for SettingsV.xaml
    /// </summary>
    public partial class SettingsV : UserControl
    {
        SettingsDC dc => (SettingsDC)DataContext;

        public SettingsV()
        {
            InitializeComponent();
        }

        private void OpenButtonClick(object sender, System.Windows.RoutedEventArgs e) => dc.OpenButton();
        private void SaveButtonClick(object sender, System.Windows.RoutedEventArgs e) => dc.SaveButton();
    }
}
