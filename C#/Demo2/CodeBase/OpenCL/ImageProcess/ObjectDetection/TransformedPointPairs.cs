using Common;
using System.Collections.Generic;


namespace ImageProcess.ObjectDetection
{
    public class TransformedPointPairs
    {
        public ContourSamplePointPair[] PointPairs { get; }
        public int Angle { get; }


        public TransformedPointPairs(ContourSamplePointPair[] pointPairs, int angle)
        {
            Angle = angle;

            List<ContourSamplePointPair> pointPairsList = new List<ContourSamplePointPair>();
            foreach (ContourSamplePointPair pp in pointPairs)
                pointPairsList.Add(new ContourSamplePointPair(Utils.RotatePoint(pp.Brighter, angle), Utils.RotatePoint(pp.Darker, angle)));

            PointPairs = pointPairsList.ToArray();
        }
    }
}
