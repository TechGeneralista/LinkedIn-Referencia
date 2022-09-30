using System.Windows;
using System.Windows.Controls;


namespace UniCamApp.Content.Settings.AutoLoader
{
    public partial class AutoLoaderV : UserControl
    {
        public AutoLoaderV()
        {
            InitializeComponent();
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e) => (DataContext as AutoLoaderDC).Browse();
    }
}
