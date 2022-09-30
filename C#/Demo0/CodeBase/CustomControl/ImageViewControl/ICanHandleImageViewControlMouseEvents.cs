using System.Windows;
using System.Windows.Input;


namespace CustomControl.ImageViewControl
{
    public interface ICanHandleImageViewControlMouseEvents
    {
        void ImageViewControlMouseDown(Point downPosition, MouseButtonEventArgs e);
        void ImageViewControlMouseMove(Vector moveVector, Point movePosition, MouseEventArgs e);
        void ImageViewControlMouseUp(Point upPosition, MouseButtonEventArgs e);
    }
}
