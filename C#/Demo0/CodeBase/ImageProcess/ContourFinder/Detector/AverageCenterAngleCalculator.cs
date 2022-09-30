using System;
using System.Collections.Generic;
using System.Windows;


namespace ImageProcess.ContourFinder.Detector
{
    public class AverageCenterAngleCalculator : AverageCenterCalculator, IDetectorPositionResult
    {
        public uint Index { get; private set; }
        public double Angle { get; private set; }
        public Point RelativeCenter { get; private set; }
        public double OrderPosition { get; private set; }


        readonly List<int> angleGroup = new List<int>();


        internal void Add(int newAngle, Point newCenter)
        {
            Add(newCenter);
            angleGroup.Add(newAngle);

            int angleSum = 0;

            foreach (int i in angleGroup)
                angleSum += i;

            Angle = (double)angleSum / angleGroup.Count;
        }

        internal void CalculateRelativePosition(Point center) => RelativeCenter = (Point)Point.Subtract(AbsoluteCenter, center);
        public void SetIndex(List<AverageCenterAngleCalculator> list) => Index = (uint)list.IndexOf(this) + 1;
        internal void CalculateOrderPosition(int backBufferStride) => OrderPosition = AbsoluteCenter.X + (AbsoluteCenter.Y * (double)backBufferStride);
    }
}