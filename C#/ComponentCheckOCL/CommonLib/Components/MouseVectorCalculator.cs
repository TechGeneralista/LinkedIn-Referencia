using System.Windows;



namespace CommonLib.Components
{
    public class MouseVectorCalculator
    {
        public Point Vector { get; private set; }


        Point oldPosition;


        public void Calculate(Point newPosition)
        {
            Vector = new Point(newPosition.X - oldPosition.X, newPosition.Y - oldPosition.Y);
            oldPosition = newPosition;
        }

        public void MouseDown(Point downPosition)
        {
            oldPosition = downPosition;
            Calculate(downPosition);
        }
    }
}
