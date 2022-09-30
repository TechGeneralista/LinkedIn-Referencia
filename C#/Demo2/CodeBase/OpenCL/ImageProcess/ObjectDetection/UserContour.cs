using Common;
using System;
using System.Collections.Generic;
using System.Windows;


namespace ImageProcess.ObjectDetection
{
    public class UserContour
    {
        public Point Center { get; private set; }
        public Size Size { get; private set; }
        public Rect Rectangle { get; private set; }
        public ContourSamplePointPair[] OriginalPointPairs { get; private set; }
        public TransformedPointPairs[] TransformedPointPairs { get; private set; }


        ContourSamplePointPair[] offsettedPointPairs;


        public void Refresh(ContourLine[] contourLines, uint rotateTolerance)
        {
            if (contourLines.IsNull() || contourLines.Length == 0)
            {
                offsettedPointPairs = OriginalPointPairs = new ContourSamplePointPair[0];
                Center = new Point();
                Size = new Size();
                Rectangle = new Rect();
                return;
            }

            CreateGroup(contourLines);
            CalculateCenter();
            OffsetCenterToOrigin();
            CalculateSize();
            CalculateRectangle();
            CalculateTransformedPointPairs(rotateTolerance);
        }

        private void CalculateTransformedPointPairs(uint rotateTolerance)
        {
            List<TransformedPointPairs> transformedPointPairsList = new List<TransformedPointPairs>();
            for (int i = 0 - (int)rotateTolerance; i <= (int)rotateTolerance; i++)
                transformedPointPairsList.Add(new TransformedPointPairs(offsettedPointPairs, i));

            TransformedPointPairs = transformedPointPairsList.ToArray();
        }

        private void OffsetCenterToOrigin()
        {
            List<ContourSamplePointPair> offsettedPointPairs = new List<ContourSamplePointPair>();
            foreach (ContourSamplePointPair pp in OriginalPointPairs)
                offsettedPointPairs.Add(new ContourSamplePointPair(new Point(pp.Brighter.X - Center.X, pp.Brighter.Y - Center.Y), new Point(pp.Darker.X - Center.X, pp.Darker.Y - Center.Y)));

            this.offsettedPointPairs = offsettedPointPairs.ToArray();
        }

        private void CalculateRectangle()
        {
            double size = 0;

            if (Size.Width >= Size.Height)
                size = Size.Width;
            else
                size = Size.Height;

            Rectangle = new Rect(Center.X - (size / 2), Center.Y - (size / 2), size, size);
        }

        private void CalculateSize()
        {
            double maximumX = 0;
            double maximumY = 0;

            foreach (ContourSamplePointPair pp in offsettedPointPairs)
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

            double max = maximumX >= maximumY ? maximumX : maximumY;

            Size = new Size(Math.Ceiling(max) * 2, Math.Ceiling(max) * 2);
        }

        private void CalculateCenter()
        {
            double centerX = 0;
            double centerY = 0;
            double divider = 0;

            foreach (ContourSamplePointPair pp in OriginalPointPairs)
            {
                centerX += pp.Brighter.X + pp.Darker.X;
                centerY += pp.Brighter.Y + pp.Darker.Y;
                divider += 2;
            }

            Center = new Point(centerX /= divider, centerY /= divider);
        }

        private void CreateGroup(ContourLine[] contourLines)
        {
            List<ContourSamplePointPair> pointPairs = new List<ContourSamplePointPair>();

            foreach (ContourLine cl in contourLines)
                foreach (ContourSamplePointPair spp in cl.ParallelSamplingPointCalculator.SamplePointPairs)
                    pointPairs.Add(new ContourSamplePointPair(spp.Brighter, spp.Darker));

            OriginalPointPairs = pointPairs.ToArray();
        }
    }
}
