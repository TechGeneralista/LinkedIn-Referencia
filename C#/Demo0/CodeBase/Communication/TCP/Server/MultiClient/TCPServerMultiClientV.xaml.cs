using System.Windows.Controls;


namespace Communication.TCP.Server.MultiClient
{
    /// <summary>
    /// Interaction logic for TCPServerSingleClientV.xaml
    /// </summary>
    public partial class TCPServerMultiClientV : UserControl
    {
        TCPServerMultiClientDC dc => (TCPServerMultiClientDC)DataContext;

        public TCPServerMultiClientV()
        {
            InitializeComponent();
        }

        private void TCPServerRestartButtonClick(object sender, System.Windows.RoutedEventArgs e) => dc.Restart();
    }
}
