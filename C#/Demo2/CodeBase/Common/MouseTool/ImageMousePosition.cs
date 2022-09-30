using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace Common.MouseTool
{
    public static class ImageMousePosition
    {
        public static System.Drawing.Point Position { get; private set; }

        public static System.Drawing.Point GetPosition(Image control, MouseEventArgs e)
        {
            if (control == null)
                Position = new System.Drawing.Point();

            Point positionOnImageControl = Mouse.GetPosition(control);

            double xScale = (double)positionOnImageControl.X / control.ActualWidth;
            double yScale = (double)positionOnImageControl.Y / control.ActualHeight;

            BitmapSource pictureInImageControl = (BitmapSource)control.Source;

            double xPosition = pictureInImageControl.PixelWidth * xScale;
            double yPosition = pictureInImageControl.PixelHeight * yScale;

            Position = new System.Drawing.Point((int)xPosition, (int)yPosition);
            return Position;
        }
    }
}
