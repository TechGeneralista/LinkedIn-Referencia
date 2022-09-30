using Common;
using Common.NotifyProperty;
using System.Windows;


namespace ImageProcess.ContourScanner.UserContourPath.UserContourPathEditor
{
    //public class SamplingPointPairsCenter : INonSettableObservableProperty<Point>
    //{
    //    public void Detect(SamplingPointPair[] samplingPointPairs)
    //    {
    //        if(samplingPointPairs.IsNull() || samplingPointPairs.Length == 0)
    //        {
    //            Center.ForceSet(new Point(double.NaN, double.NaN));
    //            return;
    //        }

    //        double centerX = 0;
    //        double centerY = 0;
    //        double divider = 0;

    //        foreach (SamplingPointPair samplingPointPair in samplingPointPairs)
    //        {
    //            centerX += samplingPointPair.Brighter.X + samplingPointPair.Darker.X;
    //            centerY += samplingPointPair.Brighter.Y + samplingPointPair.Darker.Y;
    //            divider += 2;
    //        }

    //        Center.ForceSet(new Point(centerX /= divider, centerY /= divider));
    //    }
    //}
}