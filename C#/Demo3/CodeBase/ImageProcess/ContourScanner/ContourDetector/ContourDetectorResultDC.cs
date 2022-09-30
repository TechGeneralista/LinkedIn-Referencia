using Common;
using ImageProcess.Templates;
using System.Collections.Generic;
using System.Windows;
using Common.NotifyProperty;
using Common.Language;
using ImageProcess.Source;
using ImageProcess.ContourScanner.UserContourPath.UserContourPathEditor;


namespace ImageProcess.ContourScanner.ContourDetector
{
    public class DetectorResultDC
    {
        public LanguageDC LanguageDC { get; }
        public ImageProcessSourceDC ImageProcessSourceDC { get; private set; }
        public Point Center => userContourPathDetector.DetectedCenter.Value;
        public double Size => userContourPathDetector.DetectedSize.Value;
        public IReadOnlyPropertyArray<AverageCenterAngleCalculator> PositionResults { get; } = new PropertyArray<AverageCenterAngleCalculator>();


        readonly UserContourPathDetector userContourPathDetector;


        public DetectorResultDC(LanguageDC languageDC, ImageProcessSourceDC imageProcessSourceDC, UserContourPathDetector userContourPathDetector)
        {
            LanguageDC = languageDC;
            ImageProcessSourceDC = imageProcessSourceDC;
            this.userContourPathDetector = userContourPathDetector;
        }


        public void GroupRotatedPoints(SampleCenterFinder[] sampleCenterFinders)
        {
            List<AverageCenterAngleCalculator> averageCenterAngleCalculatorList = new List<AverageCenterAngleCalculator>();
            double groupTolerance = userContourPathDetector.DetectedSize.Value / 2;

            foreach (SampleCenterFinder ppcf in sampleCenterFinders)
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
                        AverageCenterAngleCalculator averageCenterAngleCalculator = new AverageCenterAngleCalculator();
                        averageCenterAngleCalculator.Add(ppcf.Angle, cpg.AbsoluteCenter);
                        averageCenterAngleCalculatorList.Add(averageCenterAngleCalculator);
                    }
                }
            }

            foreach (AverageCenterAngleCalculator acac in averageCenterAngleCalculatorList)
                acac.CalculateRelativeCenter(userContourPathDetector.DetectedCenter.Value);

            PositionResults.ToSettable().ReAddRange(OrderByAscend(averageCenterAngleCalculatorList));
        }

        private AverageCenterAngleCalculator[] OrderByAscend(List<AverageCenterAngleCalculator> averageCenterAngleCalculators)
        {
            foreach (AverageCenterAngleCalculator acac in averageCenterAngleCalculators)
                acac.CalculateOrderPosition(ImageProcessSourceDC.ColorImage.Value.BackBufferStride);

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
