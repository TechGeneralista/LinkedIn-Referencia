using System.Windows;


namespace ImageProcess.ImageCaptureDevice.UniversalVideoClass
{
    public partial class UVCScannerV : Window
    {
        UVCScannerDC dc => (UVCScannerDC)DataContext;

        public UVCScannerV()
        {
            InitializeComponent();
        }

        private async void ScanButtonClick(object sender, RoutedEventArgs e)
            => await dc.ScanAsync();

        private void SelectButtonClick(object sender, RoutedEventArgs e)
            => dc.Select();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => ScanButtonClick(sender, e);
    }
}
