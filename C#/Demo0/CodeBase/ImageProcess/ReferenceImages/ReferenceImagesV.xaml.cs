using System.Windows.Controls;


namespace ImageProcess.ReferenceImages
{
    /// <summary>
    /// Interaction logic for ImageProcessSourceV.xaml
    /// </summary>
    public partial class ReferenceImagesV : UserControl
    {
        public ReferenceImagesV()
        {
            InitializeComponent();
        }

        private void RefreshReferenceImagesButtonClick(object sender, System.Windows.RoutedEventArgs e) => ((ReferenceImagesDC)DataContext).Refresh();
        private void ShowColorReferenceImageButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => ((ReferenceImagesDC)DataContext).ShowColorImage();
        private void ShowMonochromeReferenceImageButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => ((ReferenceImagesDC)DataContext).ShowMonochromeImage();
    }
}
