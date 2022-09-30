using System.Windows;


namespace Common.License
{
    /// <summary>
    /// Interaction logic for LicenseV.xaml
    /// </summary>
    public partial class LicenseV : Window
    {
        public LicenseV()
        {
            InitializeComponent();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
            => Close();
    }
}
