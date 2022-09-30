using SmartVisionClientApp.Common;
using System.Windows;



namespace SmartVisionClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ObjectContainer.Get<MainWindowViewModel>();
        }

        private void CameraButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<MainWindowViewModel>().CameraButtonClick();

        private void ImageOptimizationButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<MainWindowViewModel>().ImageOptimizationButtonClick();
    }
}
