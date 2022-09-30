using System.Windows;
using System.Windows.Controls;


namespace MultiCamApp.Connection
{
    /// <summary>
    /// Interaction logic for ConnectionView.xaml
    /// </summary>
    public partial class ConnectionView : UserControl
    {
        public ConnectionView()
        {
            InitializeComponent();
        }

        private void ScanButtonClick(object sender, RoutedEventArgs e) => ((ConnectionViewModel)DataContext).ScanButtonClick();
    }
}
