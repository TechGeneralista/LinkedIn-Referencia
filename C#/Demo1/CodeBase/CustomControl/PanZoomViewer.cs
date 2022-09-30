using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace CustomControl
{
    public class PanZoomViewer : ContentControl
    {
        static PanZoomViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PanZoomViewer), new FrameworkPropertyMetadata(typeof(PanZoomViewer)));
        }

        public event EventHandler NotPanned;

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(double), typeof(PanZoomViewer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(XChangedCallback)));

        private static void XChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PanZoomViewer panZoomViewer &&
                e.NewValue is double v)
            {
                panZoomViewer.tt.X = v;
            }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(double), typeof(PanZoomViewer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(YChangedCallback)));

        private static void YChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PanZoomViewer panZoomViewer &&
                e.NewValue is double v)
            {
                panZoomViewer.tt.Y = v;
            }
        }

        public double ZoomSpeed
        {
            get { return (double)GetValue(ZoomSpeedProperty); }
            set { SetValue(ZoomSpeedProperty, value); }
        }
        public static readonly DependencyProperty ZoomSpeedProperty = DependencyProperty.Register(nameof(ZoomSpeed), typeof(double), typeof(PanZoomViewer), new PropertyMetadata(.2));

        public double ScaleMinimum
        {
            get { return (double)GetValue(ScaleMinimumProperty); }
            set { SetValue(ScaleMinimumProperty, value); }
        }
        public static readonly DependencyProperty ScaleMinimumProperty = DependencyProperty.Register(nameof(ScaleMinimum), typeof(double), typeof(PanZoomViewer), new PropertyMetadata(.5));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(PanZoomViewer), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ScaleChangedCallback), new CoerceValueCallback(ScaleCoerceCallback)));

        private static void ScaleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is PanZoomViewer panZoomViewer &&
               e.NewValue is double v)
            {
                panZoomViewer.st.ScaleY = panZoomViewer.st.ScaleX = v;
            }
        }

        private static object ScaleCoerceCallback(DependencyObject d, object baseValue)
        {
            if (d is PanZoomViewer panZoomViewer && baseValue is double v)
            {
                if (v > panZoomViewer.ScaleMaximum)
                    return panZoomViewer.ScaleMaximum;

                if (v < panZoomViewer.ScaleMinimum)
                    return panZoomViewer.ScaleMinimum;
            }

            return baseValue;
        }

        public double ScaleMaximum
        {
            get { return (double)GetValue(ScaleMaximumProperty); }
            set { SetValue(ScaleMaximumProperty, value); }
        }
        public static readonly DependencyProperty ScaleMaximumProperty = DependencyProperty.Register(nameof(ScaleMaximum), typeof(double), typeof(PanZoomViewer), new PropertyMetadata(2d));

        public MouseButton PanMouseButton
        {
            get { return (MouseButton)GetValue(PanMouseButtonProperty); }
            set { SetValue(PanMouseButtonProperty, value); }
        }
        public static readonly DependencyProperty PanMouseButtonProperty = DependencyProperty.Register(nameof(PanMouseButton), typeof(MouseButton), typeof(PanZoomViewer), new PropertyMetadata(MouseButton.Left));

        public MouseButton ClearSelectionMouseButton
        {
            get { return (MouseButton)GetValue(ClearSelectionMouseButtonProperty); }
            set { SetValue(ClearSelectionMouseButtonProperty, value); }
        }
        public static readonly DependencyProperty ClearSelectionMouseButtonProperty = DependencyProperty.Register(nameof(ClearSelectionMouseButton), typeof(MouseButton), typeof(PanZoomViewer), new PropertyMetadata(MouseButton.Left));


        readonly TransformGroup tg;
        readonly TranslateTransform tt;
        readonly ScaleTransform st;
        Viewbox viewbox;
        Border border;
        Point startMousePosition, endMousePosition, origin;


        public PanZoomViewer()
        {
            Focusable = false;

            st = new ScaleTransform();
            tt = new TranslateTransform();
            tg = new TransformGroup();
            tg.Children.Add(st);
            tg.Children.Add(tt);
        }


        public override void OnApplyTemplate()
        {
            Viewbox viewbox = GetTemplateChild("viewBox") as Viewbox;
            Border border = GetTemplateChild("border") as Border;

            if (viewbox != null && border != null)
            {
                this.viewbox = viewbox;
                this.border = border;

                viewbox.RenderTransform = tg;

                border.MouseDown += Border_MouseDown;
                border.MouseMove += Border_MouseMove;
                border.MouseUp += Border_MouseUp;
                border.MouseWheel += Border_MouseWheel;

                Reset();
            }
        }

        private void Border_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(viewbox != null && border != null)
            {
                double zoom = e.Delta > 0 ? ZoomSpeed : -ZoomSpeed;

                Point relative = e.GetPosition(viewbox);
                double absoluteX = relative.X * Scale + X;
                double absoluteY = relative.Y * Scale + Y;

                Scale += zoom * Scale;

                X = absoluteX - relative.X * Scale;
                Y = absoluteY - relative.Y * Scale;

                e.Handled = true;
            }
        }

        private void Reset()
        {
            if (st != null)
                st.ScaleY = st.ScaleX = 1;

            if (tt != null)
                Y = X = 0;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startMousePosition = e.GetPosition(border);

            if (e.ChangedButton == PanMouseButton && e.ButtonState == MouseButtonState.Pressed)
            {
                if(viewbox != null && border != null && !viewbox.IsMouseCaptured)
                {
                    origin = new Point(X, Y);
                    Cursor = Cursors.Hand;
                    viewbox.CaptureMouse();
                    e.Handled = true;
                }
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if(viewbox != null && border != null)
            {
                if(viewbox.IsMouseCaptured)
                {
                    Vector v = startMousePosition - e.GetPosition(border);
                    X = origin.X - v.X;
                    Y = origin.Y - v.Y;
                    e.Handled = true;
                }
            }
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            endMousePosition = e.GetPosition(border);

            if (e.ChangedButton == PanMouseButton && e.ButtonState == MouseButtonState.Released)
            {
                if (viewbox != null && border != null && viewbox.IsMouseCaptured)
                {
                    viewbox.ReleaseMouseCapture();
                    Cursor = Cursors.Arrow;
                    e.Handled = true;
                }
            }

            if (e.ChangedButton == ClearSelectionMouseButton && e.ButtonState == MouseButtonState.Released && startMousePosition == endMousePosition)
            {
                NotPanned?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
