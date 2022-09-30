using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;



namespace CommonLib.Components
{
    public class ImagePositionConverter
    {
        public Point Position { get; private set; }

        public Point GetPosition(Image control, MouseEventArgs e)
        {
            if (control == null)
                Position = new Point(0, 0);

            Point positionOnImageControl = Mouse.GetPosition(control);

            double xScale = (double)positionOnImageControl.X / control.ActualWidth;
            double yScale = (double)positionOnImageControl.Y / control.ActualHeight;

            BitmapSource pictureInImageControl = (BitmapSource)control.Source;

            double xPosition = pictureInImageControl.PixelWidth * xScale;
            double yPosition = pictureInImageControl.PixelHeight * yScale;

            Position = new Point((int)Math.Round(xPosition, 0), (int)Math.Round(yPosition, 0));
            return Position;
        }
    }
}
