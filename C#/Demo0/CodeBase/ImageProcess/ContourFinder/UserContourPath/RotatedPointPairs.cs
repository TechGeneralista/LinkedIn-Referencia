using Common;
using System.Collections.Generic;


namespace ImageProcess.ContourFinder.UserContourPath
{
    public class RotatedPointPairs
    {
        public int Angle { get; }
        public PointPair[] PointPairs { get; private set; } = new PointPair[0];


        public RotatedPointPairs(int angle, PointPair[] originPointPairs)
        {
            Angle = angle;

            if(originPointPairs.IsNull() || originPointPairs.Length == 0)
            {
                PointPairs = new PointPair[0];
                return;
            }

            if (angle != 0)
            {
                List<PointPair> pointPairsList = new List<PointPair>();
                foreach (PointPair pp in originPointPairs)
                    pointPairsList.Add(new PointPair(pp.Brighter.RotatePoint(angle), pp.Darker.RotatePoint(angle)));

                PointPairs = pointPairsList.ToArray();
            }
            else
                PointPairs = originPointPairs;
        }
    }
}