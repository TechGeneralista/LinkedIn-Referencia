using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Common.CustomControl.Internals
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


        Image child = null;
        Point origin;
        Point start;


        public void Initialize(UIElement element)
        {
            child = (Image)element;

            if (child.IsNotNull())
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                child.RenderTransform = group;
                child.RenderTransformOrigin = new Point(0.0, 0.0);
                MouseWheel += Zoom;
                MouseDown += StartPan;
                MouseUp += EndPan;
                MouseMove += Pan;
            }
        }

        private TranslateTransform GetTranslateTransform(UIElement element) => (TranslateTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is TranslateTransform);
        private ScaleTransform GetScaleTransform(UIElement element) => (ScaleTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is ScaleTransform);

        public void Reset()
        {
            if (child.IsNotNull())
            {
                var st = GetScaleTransform(child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                var tt = GetTranslateTransform(child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        private void Zoom(object sender, MouseWheelEventArgs e)
        {
            if (child != null)
            {
                if (!IsMouseOverImage())
                    return;

                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;

                if (e.Delta > 0 && (st.ScaleX > 200 || st.ScaleY > 200))
                    return;

                Point relative = e.GetPosition(child);

                double abosuluteX = relative.X * st.ScaleX + tt.X;
                double abosuluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += (zoom * st.ScaleX);
                st.ScaleY += (zoom * st.ScaleY);

                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;

                if (!(e.Delta > 0) && (st.ScaleX < 1 || st.ScaleY < 1))
                    Reset();
            }
        }

        private void StartPan(object sender, MouseButtonEventArgs e)
        {
            if (child.IsNotNull())
            {
                if (e.ChangedButton == MouseButton.Middle && e.MiddleButton == MouseButtonState.Pressed)
                {
                    if (!IsMouseOverImage())
                        return;

                    var tt = GetTranslateTransform(child);
                    start = e.GetPosition(this);
                    origin = new Point(tt.X, tt.Y);
                    Cursor = Cursors.Hand;
                    child.CaptureMouse();
                }
            }
        }

        private bool IsMouseOverImage()
        {
            Point mousePositionOnImageControl = Mouse.GetPosition(child);

            if (mousePositionOnImageControl.X < 0 || mousePositionOnImageControl.Y < 0 || mousePositionOnImageControl.X > child.ActualWidth || mousePositionOnImageControl.Y > child.ActualHeight)
                return false;

            return true;
        }

        private void EndPan(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.MiddleButton == MouseButtonState.Released)
            {
                if (child != null)
                {
                    child.ReleaseMouseCapture();
                    Cursor = Cursors.Arrow;
                }
            }
        }

        private void Pan(object sender, MouseEventArgs e)
        {
            if (child.IsNotNull())
            {
                if (child.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(child);
                    Vector v = start - e.GetPosition(this);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;
                }
            }
        }
    }
}
