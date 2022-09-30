using Common;
using System.Windows;


namespace ImageProcess.ObjectDetection
{
    public class ContourSamplePointPair
    {
        public Point Brighter { get; private set; }
        public Point Darker { get; private set; }


        public ContourSamplePointPair(Point brighter, Point darker)
        {
            Brighter = brighter;
            Darker = darker;
        }

        public void Rotate(double angle)
        {
            Brighter = Utils.RotatePoint(Brighter, angle);
            Darker = Utils.RotatePoint(Darker, angle);
        }

        public void Add(Point point)
        {
            Brighter = Brighter.Add(point);
            Darker = Darker.Add(point);
        }

        public void Subtract(Point point)
        {
            Brighter = Brighter.Subtract(point);
            Darker = Darker.Subtract(point);
        }
    }
}