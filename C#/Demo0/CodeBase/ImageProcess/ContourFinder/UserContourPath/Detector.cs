using Common;
using System;
using System.Collections.Generic;
using System.Windows;


namespace ImageProcess.ContourFinder.UserContourPath
{
    public class Detector
    {
        public Point Center { get; private set; }
        public double Size { get; private set; }
        public PointPair[] ReferencePointPairs { get; private set; } = new PointPair[0];
        public PointPair[] OriginPointPairs { get; private set; } = new PointPair[0];


        public void Detect(PointPair[] pointPairs)
        {
            if(pointPairs.IsNull() || pointPairs.Length == 0)
            {
                OriginPointPairs = ReferencePointPairs = new PointPair[0];
                Center = new Point();
                Size = 0;
                return;
            }

            ReferencePointPairs = pointPairs;

            CalculateCenter();
            OffsetCenterToOrigin();
            CalculateSize();
        }

        private void OffsetCenterToOrigin()
        {
            List<PointPair> referencePointPairs = new List<PointPair>();
            foreach (PointPair pointPair in ReferencePointPairs)
                referencePointPairs.Add(new PointPair(Point.Subtract(pointPair.Brighter, (Vector)Center), Point.Subtract(pointPair.Darker, (Vector)Center)));

            OriginPointPairs = referencePointPairs.ToArray();
        }

        private void CalculateSize()
        {
            double maximumX = 0;
            double maximumY = 0;

            foreach (PointPair pointPair in OriginPointPairs)
            {
                double absBrighterX = Math.Abs(pointPair.Brighter.X);
                double absBrighterY = Math.Abs(pointPair.Brighter.Y);
                double absDarkerX = Math.Abs(pointPair.Darker.X);
                double absDarkerY = Math.Abs(pointPair.Darker.Y);

                if (maximumX < absBrighterX)
                    maximumX = absBrighterX;

                if (maximumY < absBrighterY)
                    maximumY = absBrighterY;

                if (maximumX < absDarkerX)
                    maximumX = absDarkerX;

                if (maximumY < absDarkerY)
                    maximumY = absDarkerY;
            }

            Point maximum = new Point(maximumX, maximumY);

            for (uint ui = 1; ui <= 90; ui++)
            {
                Point rotatedPoint = maximum.RotatePoint(ui);

                if (rotatedPoint.X > maximumX)
                    maximumX = rotatedPoint.X;

                if (rotatedPoint.Y > maximumY)
                    maximumY = rotatedPoint.Y;
            }

            double max = maximumX >= maximumY ? maximumX : maximumY;

            Size = Math.Ceiling(max) * 2;
        }

        private void CalculateCenter()
        {
            double centerX = 0;
            double centerY = 0;
            double divider = 0;

            foreach (PointPair pointPair in ReferencePointPairs)
            {
                centerX += pointPair.Brighter.X + pointPair.Darker.X;
                centerY += pointPair.Brighter.Y + pointPair.Darker.Y;
                divider += 2;
            }

            Center = new Point(centerX /= divider, centerY /= divider);
        }
    }
}