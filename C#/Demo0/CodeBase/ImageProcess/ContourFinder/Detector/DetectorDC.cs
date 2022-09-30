using ImageProcess.Templates;
using Language;
using OpenCLWrapper;
using Common.NotifyProperty;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageProcess.ContourFinder.UserContourPath;
using Common;
using System.Windows;
using ImageProcess.Source;
using Common.Settings;


namespace ImageProcess.ContourFinder.Detector
{
    public class DetectorDC : ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public ISettableObservableProperty<byte> Sensitivity { get; } = new ObservableProperty<byte>(96);
        public ISettableObservableProperty<byte> Match { get; } = new ObservableProperty<byte>(100);
        public DetectorResult DetectorResult { get; }
        

        readonly OpenCLAccelerator openCLAccelerator;
        PointPairsCenterFinder[] pointPairsCenterFinders;


        public DetectorDC(LanguageDC languageDC, OpenCLAccelerator openCLAccelerator)
        {
            LanguageDC = languageDC;
            this.openCLAccelerator = openCLAccelerator;

            DetectorResult = new DetectorResult(languageDC);
        }

        public void Prepare(RotatedPointPairs[] rotatedPointPairs)
        {
            if(pointPairsCenterFinders.IsNotNull() && pointPairsCenterFinders.Length != 0)
            {
                foreach (PointPairsCenterFinder ppcf in pointPairsCenterFinders)
                    ppcf.Dispose();
            }

            List<PointPairsCenterFinder> pointPairsCenterFinderList = new List<PointPairsCenterFinder>();
            foreach (RotatedPointPairs rpp in rotatedPointPairs)
                pointPairsCenterFinderList.Add(new PointPairsCenterFinder(openCLAccelerator, rpp.Angle, rpp.PointPairs));

            pointPairsCenterFinders = pointPairsCenterFinderList.ToArray();
        }

        public void Detect(IImageProcessSource imageProcessSource, Point center, double size)
        {
            double groupTolerance = size / 2;

            foreach (PointPairsCenterFinder ppcf in pointPairsCenterFinders)
                ppcf.FindCenters(imageProcessSource.MonochromeImageBuffer, Sensitivity.CurrentValue, Match.CurrentValue);

            Parallel.ForEach(pointPairsCenterFinders, ppcf => ppcf.GroupCenterPoints(groupTolerance));
            DetectorResult.GroupRotatedPoints(pointPairsCenterFinders, groupTolerance, imageProcessSource, center, size);
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(DetectorDC));
            settingsCollection.KeyCreator.AddNew(nameof(Sensitivity));
            settingsCollection.SetValue(Sensitivity.CurrentValue);
            settingsCollection.KeyCreator.ReplaceLast(nameof(Match));
            settingsCollection.SetValue(Match.CurrentValue);
            settingsCollection.KeyCreator.RemoveLast(2);
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(DetectorDC));
            settingsCollection.KeyCreator.AddNew(nameof(Sensitivity));
            Sensitivity.CurrentValue = settingsCollection.GetValue<byte>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(Match));
            Match.CurrentValue = settingsCollection.GetValue<byte>();
            settingsCollection.KeyCreator.RemoveLast(2);
        }
    }
}
