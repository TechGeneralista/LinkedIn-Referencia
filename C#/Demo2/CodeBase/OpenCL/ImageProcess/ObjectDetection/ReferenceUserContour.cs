using Common;
using System;
using System.Collections.Generic;
using System.Windows;


namespace ImageProcess.ObjectDetection
{
    public class ReferenceUserContour
    {
        public ContourLine[] Lines { get; }
        public ContourSamplePointPair[] PointPairs { get; private set; }
        public Point Center { get; private set; }
        public ContourSamplePointPair[] OriginPointPairs { get; private set; }
        public Size Size { get; private set; }
        public Rect Rectangle { get; private set; }
        

        public ReferenceUserContour(ContourLine[] lines)
        {
            Lines = lines;
            Detect();
        }

        private void Detect()
        {
            CreateGroup();
            CalculateCenter();
            OffsetCenterToOrigin();
            CalculateSize();
            CalculateRectangle();
        }

        private void OffsetCenterToOrigin()
        {
            List<ContourSamplePointPair> originPointPairs = new List<ContourSamplePointPair>();
            foreach (ContourSamplePointPair pp in PointPairs)
                originPointPairs.Add(new ContourSamplePointPair(new Point(pp.Brighter.X - Center.X, pp.Brighter.Y - Center.Y), new Point(pp.Darker.X - Center.X, pp.Darker.Y - Center.Y)));

            OriginPointPairs = originPointPairs.ToArray();
        }

        private void CalculateRectangle()
        {
            if (Size.Width >= Size.Height)
                Rectangle = new Rect(Center.X - (Size.Width / 2), Center.Y - (Size.Width / 2), Size.Width, Size.Width);
            else
                Rectangle = new Rect(Center.X - (Size.Height / 2), Center.Y - (Size.Height / 2), Size.Height, Size.Height);
        }

        private void CalculateSize()
        {
            double maximumX = 0;
            double maximumY = 0;

            foreach (ContourSamplePointPair pp in OriginPointPairs)
            {
                double absBrighterX = Math.Abs(pp.Brighter.X);
                double absBrighterY = Math.Abs(pp.Brighter.Y);
                double absDarkerX = Math.Abs(pp.Darker.X);
                double absDarkerY = Math.Abs(pp.Darker.Y);

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
                Point rotatedPoint = Utils.RotatePoint(maximum, ui);

                if (rotatedPoint.X > maximumX)
                    maximumX = rotatedPoint.X;

                if (rotatedPoint.Y > maximumY)
                    maximumY = rotatedPoint.Y;
            }

            Size = new Size(Math.Ceiling(maximumX) * 2, Math.Ceiling(maximumY) * 2);
        }

        private void CalculateCenter()
        {
            double centerX = 0;
            double centerY = 0;
            double divider = 0;

            foreach (ContourSamplePointPair pp in PointPairs)
            {
                centerX += pp.Brighter.X + pp.Darker.X;
                centerY += pp.Brighter.Y + pp.Darker.Y;
                divider += 2;
            }

            Center = new Point(centerX /= divider, centerY /= divider);
        }

        private void CreateGroup()
        {
            List<ContourSamplePointPair> pointPairs = new List<ContourSamplePointPair>();

            foreach (ContourLine cl in Lines)
                foreach (ContourSamplePointPair spp in cl.ParallelSamplingPointCalculator.SamplePointPairs)
                    pointPairs.Add(new ContourSamplePointPair(spp.Brighter, spp.Darker));

            PointPairs = pointPairs.ToArray();
        }
    }
}