using System.Drawing;


namespace Common.MouseTool
{
    public static class MouseVectorCalculator
    {
        public static Point Vector { get; private set; }


        static Point oldPosition;


        public static void Calculate(Point newPosition)
        {
            Vector = new Point(newPosition.X - oldPosition.X, newPosition.Y - oldPosition.Y);
            oldPosition = newPosition;
        }

        public static void MouseDown(Point downPosition)
        {
            oldPosition = downPosition;
            Calculate(downPosition);
        }
    }
}
