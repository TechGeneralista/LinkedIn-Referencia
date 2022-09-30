using Common;
using ImageProcess.Templates;
using System.Collections.Generic;
using Language;
using System.Windows;
using Common.NotifyProperty;
using ImageProcess.Source;


namespace ImageProcess.ContourFinder.Detector
{
    public class DetectorResult : IDetectorResult
    {
        public LanguageDC LanguageDC { get; }
        public IImageProcessSource ImageProcessSource { get; private set; }
        public Point Center { get; private set; }
        public double Size { get; private set; }
        public INonSettableObservablePropertyArray<IDetectorPositionResult> PositionResults { get; } = new ObservablePropertyArray<IDetectorPositionResult>();


        public DetectorResult(LanguageDC languageDC)
        {
            LanguageDC = languageDC;
        }


        public void GroupRotatedPoints(PointPairsCenterFinder[] pointPairsCenterFinders, double groupTolerance, IImageProcessSource imageProcessSource, Point center, double size)
        {
            ImageProcessSource = imageProcessSource;
            Center = center;
            Size = size;

            List<AverageCenterAngleCalculator> averageCenterAngleCalculatorList = new List<AverageCenterAngleCalculator>();

            foreach (PointPairsCenterFinder ppcf in pointPairsCenterFinders)
            {
                foreach (AverageCenterCalculator cpg in ppcf.CenterPointGroups)
                {
                    bool isAdded = false;
                    foreach (AverageCenterAngleCalculator odrg in averageCenterAngleCalculatorList)
                    {
                        if (odrg.AbsoluteCenter.Distance(cpg.AbsoluteCenter) < groupTolerance)
                        {
                            odrg.Add(ppcf.Angle, cpg.AbsoluteCenter);
                            isAdded = true;
                            break;
                        }
                    }

                    if (!isAdded)
                    {
                        AverageCenterAngleCalculator objectDetectResultGroup = new AverageCenterAngleCalculator();
                        objectDetectResultGroup.Add(ppcf.Angle, cpg.AbsoluteCenter);
                        averageCenterAngleCalculatorList.Add(objectDetectResultGroup);
                    }
                }
            }

            foreach (AverageCenterAngleCalculator acac in averageCenterAngleCalculatorList)
                acac.CalculateRelativePosition(center);

            PositionResults.ForceClear();
            PositionResults.ForceAddRange(OrderByAscend(averageCenterAngleCalculatorList));
        }

        private IDetectorPositionResult[] OrderByAscend(List<AverageCenterAngleCalculator> averageCenterAngleCalculators)
        {
            foreach (AverageCenterAngleCalculator acac in averageCenterAngleCalculators)
                acac.CalculateOrderPosition(ImageProcessSource.ColorImage.CurrentValue.BackBufferStride);

            bool exchanged;
            do
            {
                exchanged = false;

                for (int i = 0; i < averageCenterAngleCalculators.Count - 1; i++)
                {
                    if (averageCenterAngleCalculators[i].OrderPosition > averageCenterAngleCalculators[i + 1].OrderPosition)
                    {
                        AverageCenterAngleCalculator temp = averageCenterAngleCalculators[i];
                        averageCenterAngleCalculators[i] = averageCenterAngleCalculators[i + 1];
                        averageCenterAngleCalculators[i + 1] = temp;
                        exchanged = true;
                    }
                }

            } while (exchanged);

            foreach (AverageCenterAngleCalculator acac in averageCenterAngleCalculators)
                acac.SetIndex(averageCenterAngleCalculators);

            return averageCenterAngleCalculators.ToArray();
        }
    }
}
