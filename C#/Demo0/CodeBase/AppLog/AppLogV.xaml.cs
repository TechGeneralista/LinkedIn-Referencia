using System.Windows;
using System.Windows.Controls;


namespace AppLog
{
    /// <summary>
    /// Interaction logic for AppLogV.xaml
    /// </summary>
    public partial class AppLogV : UserControl
    {
        public AppLogV()
        {
            InitializeComponent();
        }

        private void ViewLogButtonClick(object sender, RoutedEventArgs e) => ((AppLogDC)((Button)sender).DataContext).ShowLogView();
    }
}
