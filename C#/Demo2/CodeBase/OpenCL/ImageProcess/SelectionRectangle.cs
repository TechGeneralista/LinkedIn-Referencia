using System;
using System.Drawing;


namespace ImageProcess
{
    public class SelectionRectangle
    {
        public event Action Changed;
        public Rectangle Rectangle { get; private set; }
        public Size Limits { get; private set; }
        public Size MinimumSize { get; private set; }


        public void Initialize(Size size)
        {
            size = new Size(size.Width - 1, size.Height - 1);

            if (Limits != size)
            {
                Limits = size;
                MinimumSize = new Size(Limits.Width / 30, Limits.Height / 30);

                Rectangle = new Rectangle
                    (
                        (Limits.Width - MinimumSize.Width) / 2,
                        (Limits.Height - MinimumSize.Height) / 2,
                        MinimumSize.Width,
                        MinimumSize.Height
                    );
            }
        }

        public void Move(Point vector)
        {
            int newXpos = Rectangle.X + vector.X;
            int newYpos = Rectangle.Y + vector.Y;

            if (newXpos < 0)
                newXpos = 0;

            if (newYpos < 0)
                newYpos = 0;

            if (newXpos+Rectangle.Width > Limits.Width || newXpos + Rectangle.Width == Limits.Width)
                newXpos = Limits.Width - Rectangle.Width;

            if (newYpos + Rectangle.Height > Limits.Height || newYpos + Rectangle.Height == Limits.Height)
                newYpos = Limits.Height - Rectangle.Height;

            Rectangle = new Rectangle(newXpos, newYpos, Rectangle.Width, Rectangle.Height);
            Changed?.Invoke();
        }

        public void Resize(Point vector)
        {
            int newWidth = Rectangle.Width + vector.X;
            int newHeight = Rectangle.Height + vector.Y;

            if (newWidth < MinimumSize.Width)
                newWidth = MinimumSize.Width;

            if (newHeight < MinimumSize.Height)
                newHeight = MinimumSize.Height;

            if (newWidth + Rectangle.X > Limits.Width || newWidth + Rectangle.X == Limits.Width)
                newWidth = Limits.Width - Rectangle.X;

            if (newHeight + Rectangle.Y > Limits.Height || newHeight + Rectangle.Y == Limits.Height)
                newHeight = Limits.Height - Rectangle.Y;

            Rectangle = new Rectangle(Rectangle.X, Rectangle.Y, newWidth, newHeight);
            Changed?.Invoke();
        }

        public bool IsUnderMouse(Point currentPosition) =>    currentPosition.X > Rectangle.X && currentPosition.X < Rectangle.X + Rectangle.Width &&
                                                                currentPosition.Y > Rectangle.Y && currentPosition.Y < Rectangle.Y + Rectangle.Height;
    }
}