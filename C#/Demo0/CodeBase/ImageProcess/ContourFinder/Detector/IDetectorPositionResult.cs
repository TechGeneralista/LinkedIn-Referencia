using System.Windows;


namespace ImageProcess.ContourFinder.Detector
{
    public interface IDetectorPositionResult
    {
        uint Index { get; }
        Point AbsoluteCenter { get; }
        Point RelativeCenter { get; }
        double Angle { get; }
    }
}
