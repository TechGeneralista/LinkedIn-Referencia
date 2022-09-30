using Common;
using Common.NotifyProperty;
using System.Collections.Generic;
using System.Windows;


namespace ImageProcess.ObjectDetection
{
    public class ParallelSamplingPointCalculator
    {
        public ISettableObservableProperty<uint> Quantity { get; } = new ObservableProperty<uint>();
        public ISettableObservableProperty<uint> Distance { get; } = new ObservableProperty<uint>();
        public ContourSamplePointPair[] SamplePointPairs { get; private set; } = new ContourSamplePointPair[0];


        ContourLine lastContourLine;


        public ParallelSamplingPointCalculator(uint quantity, uint distance)
        {
            Quantity.Value = quantity;
            Distance.Value = distance;

            Quantity.ValueChanged += (o, n) => RefreshSamplePointPairs();
            Distance.ValueChanged += (o, n) => RefreshSamplePointPairs();
        }


        internal void Calculate(ContourLine contourLine)
        {
            lastContourLine = contourLine;
            RefreshSamplePointPairs();
        }

        private void RefreshSamplePointPairs()
        {
            double sampleQuantity = lastContourLine.Length.Value / (double)Quantity.Value;

            if (lastContourLine.IsNull() || sampleQuantity == 0)
            {
                SamplePointPairs = new ContourSamplePointPair[0];
                return;
            }

            if (sampleQuantity < 1)
                sampleQuantity = 1;

            Point offsetToOrigin = lastContourLine.StartPoint.Value;
            Point endPointOffsettedToOrigin = lastContourLine.EndPoint.Value.Subtract(offsetToOrigin);

            double originalRotationDeg = Utils.Atan2(endPointOffsettedToOrigin);

            double lengthHalf = (double)Distance.Value / (double)2;
            ContourSamplePointPair pointPair = new ContourSamplePointPair(new Point(lengthHalf, 0), new Point(lengthHalf * (-1), 0));

            double distance = lastContourLine.Length.Value / (sampleQuantity + (double)1);

            List<ContourSamplePointPair> samplePointPairs = new List<ContourSamplePointPair>();
            for (uint i = 1; i <= sampleQuantity; i += 1)
                samplePointPairs.Add(new ContourSamplePointPair(new Point(pointPair.Brighter.Y + (i * distance), pointPair.Brighter.X), new Point(pointPair.Darker.Y + (i * distance), pointPair.Darker.X)));

            SamplePointPairs = samplePointPairs.ToArray();

            foreach (ContourSamplePointPair pp in SamplePointPairs)
            {
                pp.Rotate(originalRotationDeg);
                pp.Add(offsetToOrigin);
            }
        }
    }
}
