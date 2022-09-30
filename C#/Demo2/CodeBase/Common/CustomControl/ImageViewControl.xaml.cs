using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Common.CustomControl
{
    /// <summary>
    /// Interaction logic for ImageViewControl.xaml
    /// </summary>
    public partial class ImageViewControl : UserControl
    {
        new public event MouseButtonEventHandler MouseDown;
        new public event MouseEventHandler MouseMove;
        new public event MouseButtonEventHandler MouseUp;


        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(ImageViewControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d,e) => ((ImageViewControl)d).image.Source = e.NewValue as ImageSource));

        public Point MousePositionOnImage
        {
            get { return (Point)GetValue(MousePositionOnImageProperty); }
            set { SetValue(MousePositionOnImageProperty, value); }
        }

        public static readonly DependencyProperty MousePositionOnImageProperty =
            DependencyProperty.Register(nameof(MousePositionOnImage), typeof(Point), typeof(ImageViewControl), new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public Vector MouseMoveVectorOnImage
        {
            get { return (Vector)GetValue(MouseMoveVectorOnImageProperty); }
            set { SetValue(MouseMoveVectorOnImageProperty, value); }
        }

        public static readonly DependencyProperty MouseMoveVectorOnImageProperty =
            DependencyProperty.Register(nameof(MouseMoveVectorOnImage), typeof(Vector), typeof(ImageViewControl), new FrameworkPropertyMetadata(new Vector(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        Point previousMousePositionOnImage;


        public ImageViewControl()
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
                MousePositionOnImage = previousMousePositionOnImage = GetMousePositionOnImage();
                MouseDown?.Invoke(this, e);
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                Point newPosition = GetMousePositionOnImage();

                if(newPosition != MousePositionOnImage)
                {
                    MousePositionOnImage = newPosition;
                    MouseMoveVectorOnImage = GetMouseMoveVectorOnImage();
                    MouseMove?.Invoke(this, e);
                }
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released) ||
                    (e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Released))
            {
                MousePositionOnImage = previousMousePositionOnImage = GetMousePositionOnImage();
                MouseUp?.Invoke(this, e);
            }
        }

        private Vector GetMouseMoveVectorOnImage()
        {
            Vector newVector = MousePositionOnImage - previousMousePositionOnImage;
            previousMousePositionOnImage = MousePositionOnImage;
            return newVector;
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
    }
}
