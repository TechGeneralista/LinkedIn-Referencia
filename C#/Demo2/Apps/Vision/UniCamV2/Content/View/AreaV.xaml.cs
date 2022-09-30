using ImageProcess.ObjectDetection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniCamV2.Content.Model;


namespace UniCamV2.Content.View
{
    /// <summary>
    /// Interaction logic for AreaV.xaml
    /// </summary>
    public partial class AreaV : UserControl
    {
        AreaDC dc => (AreaDC)DataContext;

        public AreaV()
        {
            InitializeComponent();
        }

        private void RefreshReferenceImagesButtonClick(object sender, RoutedEventArgs e) => dc.UserContourSetup.RefreshReferenceImages();
        private void ShowColorReferenceImageClick(object sender, MouseButtonEventArgs e) => dc.ShowColorReferenceImage();
        private void ShowMonochromeReferenceImageClick(object sender, MouseButtonEventArgs e) => dc.ShowMonochromeReferenceImage();
        private void CaptureContourLinesButtonClick(object sender, RoutedEventArgs e) => dc.UserContourSetup.CaptureContourLinesButton();
        private void ClearLinesButtonClick(object sender, RoutedEventArgs e) => dc.UserContourSetup.ClearLines();
        private void RemoveSelectedLineButtonClick(object sender, RoutedEventArgs e) => dc.UserContourSetup.RemoveSelectedLine();
        private void ReplaceButtonClick(object sender, RoutedEventArgs e) => ((ContourLine)((Button)sender).DataContext).ExchangePoints();
    }
}
