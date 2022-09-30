using Common;
using InstallerApp.Items;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace InstallerApp.Contents
{
    /// <summary>
    /// Interaction logic for AppSelectorV.xaml
    /// </summary>
    public partial class AppSelectorV : UserControl
    {
        public AppSelectorV()
        {
            InitializeComponent();
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<AppSelectorDC>().BackButtonClick();

        private void NextButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<AppSelectorDC>().NextButtonClick();

        private void ShowMainApplicationEndUserLicenseAgreementClick(object sender, MouseButtonEventArgs e)
            => sender.CastTo<TextBlock>()?.DataContext.CastTo<MainApplicationItem>().ShowMainApplicationEndUserLicenseAgreementClick();

        private void ShowToolApplicationEndUserLicenseAgreementClick(object sender, MouseButtonEventArgs e)
            => sender.CastTo<TextBlock>()?.DataContext.CastTo<ToolApplicationItem>().ShowToolApplicationEndUserLicenseAgreementClick();
    }
}
