using Common;
using Common.NotifyProperty;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ImageProcess.ContourScanner.UserContourPath.UserContourPathEditor
{
    public class SamplingPointPairsGroup : PropertyArray<SamplingPointPair>
    {
        public void Refresh(UserLineDC[] userLineDCs)
        {
            if(userLineDCs.IsNull() || userLineDCs.Length == 0)
            {
                Clear();
                return;
            }

            List<SamplingPointPair> samplingPointPair = new List<SamplingPointPair>();

            foreach (UserLineDC userLineDC in userLineDCs)
                samplingPointPair.AddRange(userLineDC.SamplingPointPairs.Value);

            ReAddRange(samplingPointPair.ToArray());
        }

        public void Subtract(SamplingPointPair[] samplingPointPairs, Point value)
        {
            List<SamplingPointPair> referencePointPairs = new List<SamplingPointPair>();

            foreach (SamplingPointPair samplingPointPair in samplingPointPairs)
                referencePointPairs.Add(new SamplingPointPair(Point.Subtract(samplingPointPair.Brighter, (Vector)value), Point.Subtract(samplingPointPair.Darker, (Vector)value)));

            ReAddRange(referencePointPairs.ToArray());
        }
    }
}