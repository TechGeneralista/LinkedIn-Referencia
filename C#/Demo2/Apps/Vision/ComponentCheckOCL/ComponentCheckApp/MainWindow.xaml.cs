using System.Windows;


namespace ComponentCheckApp
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

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            vm.StartAsync();
        }

        private void HelpButtonClick(object sender, RoutedEventArgs e)
        {
            vm.HelpButtonClick();
        }
    }
}
