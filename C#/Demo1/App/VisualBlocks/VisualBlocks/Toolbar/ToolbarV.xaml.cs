using System.Windows;
using System.Windows.Controls;


namespace VisualBlocks.Toolbar
{
    public partial class ToolbarV : UserControl
    {
        ToolbarDC dc => (ToolbarDC)DataContext;

        public ToolbarV()
        {
            InitializeComponent();
        }

        private void NewButtonClick(object sender, RoutedEventArgs e)
            => dc.NewButtonClick();

        private void OpenButtonClick(object sender, RoutedEventArgs e)
            => dc.OpenButtonClick();

        private void SaveButtonClick(object sender, RoutedEventArgs e)
            => dc.SaveButtonClick();

        private void SaveAsButtonClick(object sender, RoutedEventArgs e)
            => dc.SaveAsButtonClick();

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
            => dc.SettingsButtonClick();

        private void CloseButtonClick(object sender, RoutedEventArgs e)
            => dc.CloseButtonClick();

        private void ResetViewButtonClick(object sender, RoutedEventArgs e)
            => dc.ResetViewButtonClick();
    }
}
