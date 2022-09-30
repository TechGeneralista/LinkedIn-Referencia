using ImageProcess.ContourFinder.Detector;


namespace ImageProcess.Modules
{
    public interface ICanEvulateDetectorResult
    {
        void Evulate(IDetectorResult detectorResult);
    }
}
