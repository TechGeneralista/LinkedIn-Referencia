using Common.CustomControl;
using System.Windows;
using System.Windows.Input;


namespace UniCamV2
{
    public partial class MainV : Window
    {
        MainDC dc => (MainDC)DataContext;

        public MainV()
        {
            InitializeComponent();
        }

        private void ShowImageSourceSelectContentButtonClick(object sender, RoutedEventArgs e) => dc.ShowImageSourceSelectContent();
        private void ShowImageOptimizationContentButtonClick(object sender, RoutedEventArgs e) => dc.ShowImageOptimizationContent();
        private void ShowTasksContentButtonClick(object sender, RoutedEventArgs e) => dc.ShowTasksContent();
        private void ImageViewControl_MouseDown(object sender, MouseButtonEventArgs e) => dc.DisplayMouseDown(((ImageViewControl)sender).MousePositionOnImage,e);
        private void ImageViewControl_MouseMove(object sender, MouseEventArgs e) => dc.DisplayMouseMove(((ImageViewControl)sender).MousePositionOnImage, ((ImageViewControl)sender).MouseMoveVectorOnImage,e);
        private void ImageViewControl_MouseUp(object sender, MouseButtonEventArgs e) => dc.DisplayMouseUp(((ImageViewControl)sender).MousePositionOnImage,e);
    }
}
