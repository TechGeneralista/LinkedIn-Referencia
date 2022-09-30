using Common;
using System.Collections.Generic;


namespace ImageProcess.ContourFinder.UserContourPath
{
    public class PointPairsGroupCreator
    {
        public PointPair[] PointPairs { get; private set; } = new PointPair[0];


        internal void Create(UserLineDC[] userLines)
        {
            if(userLines.IsNull() || userLines.Length == 0)
            {
                PointPairs = new PointPair[0];
                return;
            }

            List<PointPair> pointPairs = new List<PointPair>();
                foreach(UserLineDC userLineDC in userLines)
                    pointPairs.AddRange(userLineDC.PointPairs);
                
            PointPairs = pointPairs.ToArray();
        }
    }
}
