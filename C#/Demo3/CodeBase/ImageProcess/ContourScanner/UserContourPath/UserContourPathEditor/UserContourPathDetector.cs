using Common;
using Common.NotifyProperty;
using System;
using System.Windows;


namespace ImageProcess.ContourScanner.UserContourPath.UserContourPathEditor
{
    public class UserContourPathDetector
    {
        public IReadOnlyPropertyArray<SamplingPointPair> ReferenceSamplingPointPairsGroup { get; } = new SamplingPointPairsGroup();
        public IReadOnlyProperty<Point> DetectedCenter { get; } = new DetectedCenter();
        public IReadOnlyPropertyArray<SamplingPointPair> OriginSamplingPointPairsGroup { get; } = new SamplingPointPairsGroup();
        public IReadOnlyProperty<double> DetectedSize { get; } = new DetectedSize();


        readonly UserContourPathEditorDC userContourPathEditorDC;


        public UserContourPathDetector(UserContourPathEditorDC userContourPathEditorDC)
        {
            this.userContourPathEditorDC = userContourPathEditorDC;
        }

        public void Detect()
        {
            ReferenceSamplingPointPairsGroup.CastTo<SamplingPointPairsGroup>().Refresh(userContourPathEditorDC.UserLines.Value);
            DetectedCenter.CastTo<DetectedCenter>().Detect(ReferenceSamplingPointPairsGroup.Value);
            OriginSamplingPointPairsGroup.CastTo<SamplingPointPairsGroup>().Subtract(ReferenceSamplingPointPairsGroup.Value, DetectedCenter.Value);
            DetectedSize.CastTo<DetectedSize>().Detect(OriginSamplingPointPairsGroup.Value);
        }
    }
}
