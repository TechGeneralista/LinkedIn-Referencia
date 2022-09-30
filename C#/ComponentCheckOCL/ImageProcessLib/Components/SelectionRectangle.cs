using CommonLib.Components;
using CommonLib.Types;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ImageProcessLib.Components
{
    public class SelectionRectangle
    {
        public event Action PositionOrSizeChanged;
        public Int32Rect Int32Rect => new Int32Rect(x, y, width, height);


        int x;
        int y;
        int width;
        int height;
        Int32Size maximumSize;
        Int32Size minimumSize;
        Int32Size defaultSize;
        ImagePositionConverter imagePositionConverter;
        MouseVectorCalculator mouseVectorCalculator;
        bool capture;


        public SelectionRectangle()
        {
            imagePositionConverter = new ImagePositionConverter();
            mouseVectorCalculator = new MouseVectorCalculator();
        }

        public void CheckPosition(Int32Size maximumSize)
        {
            if (!(this.maximumSize.Width == maximumSize.Width && this.maximumSize.Height == maximumSize.Height))
            {
                this.maximumSize = maximumSize;

                minimumSize = new Int32Size
                    (
                        (int)(this.maximumSize.Width * 0.05f),
                        (int)(this.maximumSize.Height * 0.05f)
                    );

                defaultSize = new Int32Size
                    (
                        (int)(this.maximumSize.Width * 0.1f),
                        (int)(this.maximumSize.Height * 0.1f)
                    );

                x = ((this.maximumSize.Width / 2) - (minimumSize.Width / 2));
                y = ((this.maximumSize.Height / 2) - (minimumSize.Height / 2));
                width = defaultSize.Width;
                height = defaultSize.Height;
            }
        }

        public void MouseDown(Image control, MouseButtonEventArgs e)
        {
            imagePositionConverter.GetPosition(control, e);
            int mouseX = (int)imagePositionConverter.Position.X;
            int mouseY = (int)imagePositionConverter.Position.Y;

            if (mouseX > x && mouseX < x + width && mouseY > y && mouseY < y + height && (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed))
            {
                capture = true;
                mouseVectorCalculator.MouseDown(imagePositionConverter.Position);
            }
        }

        public void MouseMove(Image control, MouseEventArgs e)
        {
            imagePositionConverter.GetPosition(control, e);
            mouseVectorCalculator.Calculate(imagePositionConverter.Position);

            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released && capture)
            {
                x += (int)mouseVectorCalculator.Vector.X;
                y += (int)mouseVectorCalculator.Vector.Y;

                if (x < 0)
                    x = 0;

                if (x + width > maximumSize.Width)
                    x = maximumSize.Width - width;

                if (y < 0)
                    y = 0;

                if (y + height > maximumSize.Height)
                    y = maximumSize.Height - height;

                PositionOrSizeChanged?.Invoke();
            }

            if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed && capture)
            {
                width += (int)mouseVectorCalculator.Vector.X;
                height += (int)mouseVectorCalculator.Vector.Y;

                if (width < minimumSize.Width)
                    width = minimumSize.Width;

                if (width + x > maximumSize.Width)
                    width = maximumSize.Width - x;

                if (height < minimumSize.Width)
                    height = minimumSize.Width;

                if (height + y > maximumSize.Height)
                    height = maximumSize.Height - y;

                PositionOrSizeChanged?.Invoke();
            }
        }

        public void MouseUp(Image control, MouseButtonEventArgs e)
        {
            imagePositionConverter.GetPosition(control, e);

            if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released && capture)
            {
                capture = false;
            }
        }
    }
}
