using System.Windows.Controls;


namespace Communication.TCP.Server.SingleClient
{
    /// <summary>
    /// Interaction logic for TCPServerSingleClientV.xaml
    /// </summary>
    public partial class TCPServerSingleClientV : UserControl
    {
        TCPServerSingleClientDC dc => (TCPServerSingleClientDC)DataContext;

        public TCPServerSingleClientV()
        {
            InitializeComponent();
        }

        private void TCPServerRestartButtonClick(object sender, System.Windows.RoutedEventArgs e) => dc.Restart();
    }
}
