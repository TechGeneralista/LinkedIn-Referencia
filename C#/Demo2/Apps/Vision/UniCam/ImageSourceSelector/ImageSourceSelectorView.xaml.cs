using Common;
using System.Windows;
using System.Windows.Controls;


namespace UniCamApp.ImageSourceSelector
{
    /// <summary>
    /// Interaction logic for ImageSourceSelectorView.xaml
    /// </summary>
    public partial class ImageSourceSelectorView : UserControl
    {
        public ImageSourceSelectorView()
        {
            InitializeComponent();
            DataContext = ObjectContainer.Get<ImageSourceSelectorViewModel>();
        }

        private void ScanButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<ImageSourceSelectorViewModel>().ScanButtonClick();
        private void StartButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<ImageSourceSelectorViewModel>().StartButtonClick();
        private void StopButtonClick(object sender, RoutedEventArgs e) => ObjectContainer.Get<ImageSourceSelectorViewModel>().StopButtonClick();
    }
}
