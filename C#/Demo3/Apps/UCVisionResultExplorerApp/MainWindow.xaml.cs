using Common;
using System.Windows;


namespace UCVisionResultExplorerApp
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Maximized;
    }
}
