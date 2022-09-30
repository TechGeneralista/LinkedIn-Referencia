using System.Windows;


namespace MultiCamApp.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectionButtonClick(object sender, RoutedEventArgs e) => ((MainWindowViewModel)DataContext).Content.ShowConnection();
    }
}
