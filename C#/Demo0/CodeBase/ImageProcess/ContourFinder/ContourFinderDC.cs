using Common.Interface;
using Common.NotifyProperty;
using Common.Settings;
using CustomControl.ImageViewControl;
using ImageProcess.ContourFinder.Detector;
using ImageProcess.ContourFinder.UserContourPath;
using ImageProcess.ReferenceImages;
using ImageProcess.Source;
using Language;
using OpenCLWrapper;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace ImageProcess.ContourFinder
{
    public class ContourFinderDC : ICanSaveLoadSettings, ICanProcessImageProcessSource, ICanShowResultImage, ICanHandleImageViewControlMouseEvents
    {
        public UserContourPathDC UserContourPathDC { get; }
        public DetectorDC DetectorDC { get; }


        IImageProcessSource lastImageProcessSource;
        ContourFinderResultDrawer contourFinderResultDrawer;


        public ContourFinderDC(LanguageDC languageDC, OpenCLAccelerator openCLAccelerator, IImageProcessSource lastImageProcessSource, ReferenceImagesDC referenceImagesDC, ISettableObservableProperty<ImageSource> mainDisplaySource)
        {
            this.lastImageProcessSource = lastImageProcessSource;

            UserContourPathDC = new UserContourPathDC(languageDC, referenceImagesDC, mainDisplaySource);
            DetectorDC = new DetectorDC(languageDC, openCLAccelerator);

            contourFinderResultDrawer = new ContourFinderResultDrawer(mainDisplaySource);
        }

        public void Process(IImageProcessSource imageProcessSource)
        {
            if (!UserContourPathDC.Editing)
            {
                lastImageProcessSource = imageProcessSource;

                if (UserContourPathDC.RotatedPointPairsGroup.RotatedPointPairsChanged.CurrentValue)
                    DetectorDC.Prepare(UserContourPathDC.RotatedPointPairsGroup.RotatedPointPairs);

                DetectorDC.Detect(imageProcessSource, UserContourPathDC.Detector.Center, UserContourPathDC.Detector.Size);
            } 
        }

        public void ShowResultImage()
        {
            if (!UserContourPathDC.Editing)
                contourFinderResultDrawer.DrawResultImage(lastImageProcessSource, DetectorDC.DetectorResult, UserContourPathDC.Detector.Size);
        }

        public void ImageViewControlMouseDown(Point mousePositionOnImage, MouseButtonEventArgs e)
            => UserContourPathDC.ImageViewControlMouseDown(mousePositionOnImage, e);

        public void ImageViewControlMouseMove(Vector mouseVectorOnImage, Point mousePositionOnImage, MouseEventArgs e)
            => UserContourPathDC.ImageViewControlMouseMove(mouseVectorOnImage, mousePositionOnImage, e);

        public void ImageViewControlMouseUp(Point mousePositionOnImage, MouseButtonEventArgs e)
            => UserContourPathDC.ImageViewControlMouseUp(mousePositionOnImage, e);

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(ContourFinderDC));
            UserContourPathDC.SaveSettings(settingsCollection);
            DetectorDC.SaveSettings(settingsCollection);
            settingsCollection.KeyCreator.RemoveLast();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(ContourFinderDC));
            UserContourPathDC.LoadSettings(settingsCollection);
            DetectorDC.LoadSettings(settingsCollection);
            settingsCollection.KeyCreator.RemoveLast();
        }
    }
}
