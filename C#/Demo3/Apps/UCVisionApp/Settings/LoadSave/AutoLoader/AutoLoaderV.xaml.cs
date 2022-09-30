using Common;
using System.Windows;
using System.Windows.Controls;


namespace UCVisionApp.Settings.LoadSave.AutoLoader
{
    public partial class AutoLoaderV : UserControl
    {
        public AutoLoaderV()
        {
            InitializeComponent();
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e) => DataContext.CastTo<AutoLoaderDC>().Browse();
    }
}
