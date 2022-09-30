using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Common.Controls.PanZoomImageView
{
    public partial class PanZoomImageViewV : UserControl
    {
        new public event Action<Point, MouseButtonEventArgs> MouseDown;
        new public event Action<Vector, Point, MouseEventArgs> MouseMove;
        new public event Action<Point, MouseButtonEventArgs> MouseUp;


        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(PanZoomImageViewV), new FrameworkPropertyMetadata(null, (d,e) => ((PanZoomImageViewV)d).image.Source = e.NewValue as ImageSource));


        Point previousMousePositionOnImage;


        public PanZoomImageViewV()
        {
            InitializeComponent();

            image.MouseDown += Image_MouseDown;
            image.MouseMove += Image_MouseMove;
            image.MouseUp += Image_MouseUp;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed) ||
                    (e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Pressed))
            {
                Point mousePositionOnImage = previousMousePositionOnImage = GetMousePositionOnImage();
                MouseDown?.Invoke(mousePositionOnImage, e);
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                Point positionOnImage = GetMousePositionOnImage();
                Vector moveVector = positionOnImage - previousMousePositionOnImage;
                previousMousePositionOnImage = positionOnImage;
                MouseMove?.Invoke(moveVector, positionOnImage, e);
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released) ||
                    (e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Released))
            {
                Point mousePositionOnImage = previousMousePositionOnImage = GetMousePositionOnImage();
                MouseUp?.Invoke(mousePositionOnImage, e);
            }
        }

        private Point GetMousePositionOnImage()
        {
            Point mousePositionOnImageControl = Mouse.GetPosition(image);
            double xScale = mousePositionOnImageControl.X / image.ActualWidth;
            double yScale = mousePositionOnImageControl.Y / image.ActualHeight;

            ImageSource pictureInImageControl = (ImageSource)image.Source;

            double xPosition = pictureInImageControl.Width * xScale;
            double yPosition = pictureInImageControl.Height * yScale;

            return new Point(Math.Floor(xPosition), Math.Floor(yPosition));
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) => ((ZoomPanBorder)sender).Reset();
    }
}
