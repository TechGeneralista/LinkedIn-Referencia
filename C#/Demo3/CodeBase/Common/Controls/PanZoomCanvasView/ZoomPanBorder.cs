using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Common.Controls.PanZoomCanvasView
{
    public class ZoomPanBorder : Border
    {
        public double ScaleMinimum { get; set; } = 0.05;
        public double ScaleMaximum { get; set; } = 2;

        public override UIElement Child
        {
            get => base.Child;

            set
            {
                if (value.IsNotNull() && value != Child)
                {
                    itemsControl = (ItemsControl)value;
                    itemsControl.Loaded += ItemsControl_Loaded;
                }

                base.Child = value;
            }
        }


        ItemsControl itemsControl;
        Point origin;
        Point start;
        ScaleTransform scaleTransform;
        TranslateTransform translateTransform;
        FrameworkElement panel;


        private void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (itemsControl.IsNotNull())
            {
                TransformGroup group = new TransformGroup();
                scaleTransform = new ScaleTransform();
                translateTransform = new TranslateTransform();
                group.Children.Add(scaleTransform);
                group.Children.Add(translateTransform);
                itemsControl.RenderTransform = group;
                itemsControl.RenderTransformOrigin = new Point(0.0, 0.0);
                MouseWheel += Zoom;
                MouseDown += StartPan;
                MouseUp += EndPan;
                MouseMove += Pan;
                e.Handled = true;
            }
        }

        public void Reset()
        {
            if (itemsControl.IsNotNull())
            {
                scaleTransform.ScaleX = ScaleMinimum;
                scaleTransform.ScaleY = ScaleMinimum;
                translateTransform.X = (ActualWidth / 2) - ((panel.ActualWidth / 2) * scaleTransform.ScaleX);
                translateTransform.Y = (ActualHeight / 2) - ((panel.ActualHeight / 2) * scaleTransform.ScaleY);
            }
        }

        internal void RegisterPanel(FrameworkElement panel)
        {
            this.panel = panel;
            Reset();
        }

        private void Zoom(object sender, MouseWheelEventArgs e)
        {
            if (itemsControl.IsNotNull())
            {
                double zoom = e.Delta > 0 ? .2 : -.2;

                if (e.Delta > 0 && (scaleTransform.ScaleX > ScaleMaximum || scaleTransform.ScaleY > ScaleMaximum))
                    return;

                Point relative = e.GetPosition(itemsControl);

                double abosuluteX = relative.X * scaleTransform.ScaleX + translateTransform.X;
                double abosuluteY = relative.Y * scaleTransform.ScaleY + translateTransform.Y;

                scaleTransform.ScaleX += (zoom * scaleTransform.ScaleX);
                scaleTransform.ScaleY += (zoom * scaleTransform.ScaleY);

                translateTransform.X = abosuluteX - relative.X * scaleTransform.ScaleX;
                translateTransform.Y = abosuluteY - relative.Y * scaleTransform.ScaleY;

                if (!(e.Delta > 0) && (scaleTransform.ScaleX < ScaleMinimum || scaleTransform.ScaleY < ScaleMinimum))
                    Reset();

                e.Handled = true;
            }
        }

        private void StartPan(object sender, MouseButtonEventArgs e)
        {
            if (itemsControl.IsNotNull() && e.ChangedButton == MouseButton.Middle && e.MiddleButton == MouseButtonState.Pressed)
            {
                start = e.GetPosition(this);
                origin = new Point(translateTransform.X, translateTransform.Y);
                Cursor = Cursors.Hand;
                itemsControl.CaptureMouse();
                e.Handled = true;
            }
        }

        private void EndPan(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.MiddleButton == MouseButtonState.Released)
            {
                if (itemsControl.IsNotNull())
                {
                    itemsControl.ReleaseMouseCapture();
                    Cursor = Cursors.Arrow;
                    e.Handled = true;
                }
            }
        }

        private void Pan(object sender, MouseEventArgs e)
        {
            if (itemsControl.IsNotNull() && itemsControl.IsMouseCaptured)
            {
                Vector v = start - e.GetPosition(this);
                translateTransform.X = origin.X - v.X;
                translateTransform.Y = origin.Y - v.Y;
                e.Handled = true;
            }
        }
    }
}
