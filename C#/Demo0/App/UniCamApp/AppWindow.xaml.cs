using CustomControl.ImageViewControl;
using System.Windows;
using System.Windows.Input;


namespace UniCamApp
{
    /// <summary>
    /// Interaction logic for AppWindow.xaml
    /// </summary>
    public partial class AppWindow : Window
    {
        public AppWindow()
        {
            InitializeComponent();
        }

        private void ImageViewControlMouseDown(Point downPosition, MouseButtonEventArgs e) => (DataContext as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseDown(downPosition,e);
        private void ImageViewControlMouseMove(Vector moveVector, Point movePosition, MouseEventArgs e) => (DataContext as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseMove(moveVector, movePosition, e);
        private void ImageViewControlMouseUp(Point upPosition, MouseButtonEventArgs e) => (DataContext as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseUp(upPosition, e);
    }
}
