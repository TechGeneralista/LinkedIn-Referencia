using System.Windows;


namespace ImageProcess.ObjectDetection
{
    public class ContourFinderCenterPoint
    {
        public Point DetectedCenterPoint { get; private set; }


        public ContourFinderCenterPoint(Point firstCenter) => DetectedCenterPoint = firstCenter;

        public void Add(Point p) => DetectedCenterPoint = new Point((DetectedCenterPoint.X + p.X) / 2, (DetectedCenterPoint.Y + p.Y) / 2);
    }
}