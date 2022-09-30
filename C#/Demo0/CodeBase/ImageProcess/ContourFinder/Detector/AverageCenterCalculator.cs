using System.Collections.Generic;
using System.Windows;


namespace ImageProcess.ContourFinder.Detector
{
    public class AverageCenterCalculator
    {
        public Point AbsoluteCenter { get; private set; }


        readonly List<Point> group = new List<Point>();


        public void Add(Point centerPoint)
        {
            group.Add(centerPoint);

            double xSum = 0;
            double ySum = 0;

            foreach(Point p in group)
            {
                xSum += p.X;
                ySum += p.Y;
            }

            AbsoluteCenter = new Point(xSum / group.Count, ySum / group.Count);
        }
    }
}