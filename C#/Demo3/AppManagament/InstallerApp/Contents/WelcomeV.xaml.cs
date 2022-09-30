using System.Windows;
using System.Windows.Controls;


namespace InstallerApp.Contents
{
    /// <summary>
    /// Interaction logic for WelcomeV.xaml
    /// </summary>
    public partial class WelcomeV : UserControl
    {
        public WelcomeV()
        {
            InitializeComponent();
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
            => ((WelcomeDC)DataContext).NextButtonClick();

        private void ShowPrivacyPolicyClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => ((WelcomeDC)DataContext).ShowPrivacyPolicyClick();
    }
}
