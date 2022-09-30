using Common;
using Common.NotifyProperty;
using System;
using System.Windows;


namespace ImageProcess.ContourScanner.UserContourPath.UserContourPathEditor
{
    public class DetectedSize : Property<double>
    {
        public void Detect(SamplingPointPair[] samplingPointPairs)
        {
            if(samplingPointPairs.IsNull() || samplingPointPairs.Length == 0)
            {
                Value = double.NaN;
                return;
            }

            double maximumX = 0;
            double maximumY = 0;

            foreach (SamplingPointPair samplingPointPair in samplingPointPairs)
            {
                double absBrighterX = Math.Abs(samplingPointPair.Brighter.X);
                double absBrighterY = Math.Abs(samplingPointPair.Brighter.Y);
                double absDarkerX = Math.Abs(samplingPointPair.Darker.X);
                double absDarkerY = Math.Abs(samplingPointPair.Darker.Y);

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

            Value = Math.Ceiling(max) * 2;
        }
    }
}