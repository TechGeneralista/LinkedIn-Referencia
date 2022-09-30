using System.Windows;

namespace BalanceApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel vm => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void ZeroButtonClick(object sender, RoutedEventArgs e)
        {
            vm.Balance.Zero();
        }

        private void CalibrateToButtonClick(object sender, RoutedEventArgs e)
        {
            vm.Balance.Calibrate();
        }
    }
}
