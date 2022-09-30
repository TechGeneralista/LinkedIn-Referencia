using Common.NotifyProperty;
using ImageProcess.Source;
using System.Windows;


namespace ImageProcess.ContourFinder.Detector
{
    public interface IDetectorResult
    {
        IImageProcessSource ImageProcessSource { get; }
        Point Center { get; }
        double Size { get; }
        INonSettableObservablePropertyArray<IDetectorPositionResult> PositionResults { get; }
    }
}