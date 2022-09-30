using ImageProcess.Templates;
using OpenCLWrapper;
using Common.Language;
using Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageProcess.Source;
using System.Windows.Media;
using Common.NotifyProperty;
using Common.Settings;
using ImageProcess.ContourScanner.UserContourPath.TransformedUserContourPaths;
using ImageProcess.ContourScanner.UserContourPath.UserContourPathEditor;


namespace ImageProcess.ContourScanner.ContourDetector
{
    public class ContourDetectorDC : ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public IProperty<byte> Sensitivity { get; } = new Property<byte>(96);
        public IProperty<byte> Match { get; } = new Property<byte>(100);
        public DetectorResultDC DetectorResultDC { get; }
        public ContourDetectorResultDrawer ContourDetectorResultDrawer { get; }


        readonly OpenCLAccelerator openCLAccelerator;
        SampleCenterFinder[] sampleCenterFinders;
        readonly ImageProcessSourceDC imageProcessSourceDC;
        readonly TransformedUserContourPathsDC transformedUserContourPathsDC;
        readonly UserContourPathDetector userContourPathDetector;
        

        public ContourDetectorDC(LanguageDC languageDC, OpenCLAccelerator openCLAccelerator, ImageProcessSourceDC imageProcessSourceDC, TransformedUserContourPathsDC transformedUserContourPathsDC, UserContourPathDetector userContourPathDetector, IReadOnlyProperty<ImageSource> resultImage)
        {
            LanguageDC = languageDC;
            this.openCLAccelerator = openCLAccelerator;
            this.imageProcessSourceDC = imageProcessSourceDC;
            this.transformedUserContourPathsDC = transformedUserContourPathsDC;
            this.userContourPathDetector = userContourPathDetector;

            DetectorResultDC = new DetectorResultDC(languageDC, imageProcessSourceDC, userContourPathDetector);
            ContourDetectorResultDrawer = new ContourDetectorResultDrawer(DetectorResultDC, resultImage);
        }

        public void Prepare()
        {
            if (this.sampleCenterFinders.IsNotNull() && this.sampleCenterFinders.Length != 0)
            {
                foreach (SampleCenterFinder ppcf in this.sampleCenterFinders)
                    ppcf.Dispose();
            }

            List<SampleCenterFinder> sampleCenterFinders = new List<SampleCenterFinder>();

            foreach (TransformedUserContourPath transformedUserContourPath in transformedUserContourPathsDC.Value)
                sampleCenterFinders.Add(new SampleCenterFinder(openCLAccelerator, transformedUserContourPath));

            this.sampleCenterFinders = sampleCenterFinders.ToArray();
        }

        public void Detect()
        {
            if (sampleCenterFinders.IsNull() || sampleCenterFinders.Length == 0)
                return;

            foreach (SampleCenterFinder ppcf in sampleCenterFinders)
                ppcf.FindCenters(imageProcessSourceDC.MonochromeImageBuffer, Sensitivity.Value, Match.Value);

            Parallel.ForEach(sampleCenterFinders, ppcf => ppcf.GroupCenterPoints(userContourPathDetector.DetectedSize.Value / 2));
            DetectorResultDC.GroupRotatedPoints(sampleCenterFinders);
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            settingsCollection.SetProperty(Sensitivity.Value, nameof(Sensitivity));
            settingsCollection.SetProperty(Match.Value, nameof(Match));
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            Sensitivity.Value = settingsCollection.GetProperty<byte>(nameof(Sensitivity));
            Match.Value = settingsCollection.GetProperty<byte>(nameof(Match));
            settingsCollection.ExitPoint();
        }
    }
}
