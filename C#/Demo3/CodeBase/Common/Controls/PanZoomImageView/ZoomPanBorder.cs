using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Common.Controls.PanZoomImageView
{
    public class ZoomPanBorder : Border
    {
        public override UIElement Child
        {
            get => base.Child;

            set
            {
                if (value.IsNotNull() && value != Child)
                    Initialize(value);

                base.Child = value;
            }
        }


        Border border;
        Image image = null;
        Point origin;
        Point start;
        ScaleTransform scaleTransform;
        TranslateTransform translateTransform;


        public void Initialize(UIElement element)
        {
            border = (Border)element;
            border.Loaded += Border_Loaded;
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            image = (Image)border.Child;

            if (image.IsNotNull())
            {
                TransformGroup group = new TransformGroup();
                scaleTransform = new ScaleTransform();
                translateTransform = new TranslateTransform();
                group.Children.Add(scaleTransform);
                group.Children.Add(translateTransform);
                border.RenderTransform = group;
                border.RenderTransformOrigin = new Point(0.0, 0.0);
                MouseWheel += Zoom;
                MouseDown += StartPan;
                MouseUp += EndPan;
                MouseMove += Pan;

                e.Handled = true;
            }
        }

        public void Reset()
        {
            if (border.IsNotNull() && image.IsNotNull())
            {
                scaleTransform.ScaleX = 1.0;
                scaleTransform.ScaleY = 1.0;

                translateTransform.X = 0.0;
                translateTransform.Y = 0.0;
            }
        }

        private void Zoom(object sender, MouseWheelEventArgs e)
        {
            if (image != null)
            {
                if (!IsMouseOverImage())
                    return;

                double zoom = e.Delta > 0 ? .2 : -.2;

                if (e.Delta > 0 && (scaleTransform.ScaleX > 200 || scaleTransform.ScaleY > 200))
                    return;

                Point relative = e.GetPosition(border);

                double abosuluteX = relative.X * scaleTransform.ScaleX + translateTransform.X;
                double abosuluteY = relative.Y * scaleTransform.ScaleY + translateTransform.Y;

                scaleTransform.ScaleX += (zoom * scaleTransform.ScaleX);
                scaleTransform.ScaleY += (zoom * scaleTransform.ScaleY);

                translateTransform.X = abosuluteX - relative.X * scaleTransform.ScaleX;
                translateTransform.Y = abosuluteY - relative.Y * scaleTransform.ScaleY;

                if (!(e.Delta > 0) && (scaleTransform.ScaleX < 1 || scaleTransform.ScaleY < 1))
                    Reset();

                e.Handled = true;
            }
        }

        private void StartPan(object sender, MouseButtonEventArgs e)
        {
            if (image.IsNotNull())
            {
                if (e.ChangedButton == MouseButton.Middle && e.MiddleButton == MouseButtonState.Pressed)
                {
                    if (!IsMouseOverImage())
                        return;

                    start = e.GetPosition(this);
                    origin = new Point(translateTransform.X, translateTransform.Y);
                    Cursor = Cursors.Hand;
                    image.CaptureMouse();

                    e.Handled = true;
                }
            }
        }

        private bool IsMouseOverImage()
        {
            Point mousePositionOnImageControl = Mouse.GetPosition(image);

            if (mousePositionOnImageControl.X < 0 || mousePositionOnImageControl.Y < 0 || mousePositionOnImageControl.X > image.ActualWidth || mousePositionOnImageControl.Y > image.ActualHeight)
                return false;

            return true;
        }

        private void EndPan(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.MiddleButton == MouseButtonState.Released)
            {
                if (image != null)
                {
                    image.ReleaseMouseCapture();
                    Cursor = Cursors.Arrow;

                    e.Handled = true;
                }
            }
        }

        private void Pan(object sender, MouseEventArgs e)
        {
            if (image.IsNotNull())
            {
                if (image.IsMouseCaptured)
                {
                    Vector v = start - e.GetPosition(this);
                    translateTransform.X = origin.X - v.X;
                    translateTransform.Y = origin.Y - v.Y;

                    e.Handled = true;
                }
            }
        }
    }
}
