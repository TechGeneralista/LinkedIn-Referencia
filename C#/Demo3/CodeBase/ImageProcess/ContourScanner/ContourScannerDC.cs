using Common.Language;
using Common.NotifyProperty;
using Common.Settings;
using ImageProcess.ContourScanner.ContourDetector;
using ImageProcess.ContourScanner.UserContourPath;
using ImageProcess.ReferenceImages;
using ImageProcess.Source;
using OpenCLWrapper;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace ImageProcess.ContourScanner
{
    public class ContourScannerDC : ICanSaveLoadSettings
    {
        public ReferenceImagesDC ReferenceImagesDC { get; }
        public UserContourPathDC UserContourPathDC { get; }
        public ContourDetectorDC ContourDetectorDC { get; }


        readonly IReadOnlyProperty<ImageSource> resultImage;
        bool needToPrepare;
        readonly int timeout = 3000;


        public ContourScannerDC(LanguageDC languageDC, OpenCLAccelerator openCLAccelerator, ImageProcessSourceDC imageProcessSourceDC, IReadOnlyProperty<ImageSource> resultImage)
        {
            this.resultImage = resultImage;

            ReferenceImagesDC = new ReferenceImagesDC(languageDC, imageProcessSourceDC, resultImage);
            UserContourPathDC = new UserContourPathDC(languageDC, ReferenceImagesDC, imageProcessSourceDC, resultImage);
            ContourDetectorDC = new ContourDetectorDC(languageDC, openCLAccelerator, imageProcessSourceDC, UserContourPathDC.TransformedUserContourPathsDC, UserContourPathDC.UserContourPathEditorDC.UserContourPathDetector, resultImage);
        }

        public void Process()
        {
            double UserContourPathEditorDCElapsedMilliseconds = (DateTime.Now - UserContourPathDC.UserContourPathEditorDC.LastModified).TotalMilliseconds;
            double TransformedUserContourPathsDCElapsedMilliseconds = (DateTime.Now - UserContourPathDC.TransformedUserContourPathsDC.LastModified).TotalMilliseconds;

            if (UserContourPathEditorDCElapsedMilliseconds < timeout || TransformedUserContourPathsDCElapsedMilliseconds < timeout)
                needToPrepare = true;

            else if (UserContourPathEditorDCElapsedMilliseconds > timeout && TransformedUserContourPathsDCElapsedMilliseconds > timeout)
            {
                if (needToPrepare)
                {
                    UserContourPathDC.TransformedUserContourPathsDC.Create();
                    ContourDetectorDC.Prepare();
                    needToPrepare = false;
                }

                ContourDetectorDC.Detect();
            }
        }

        public void DrawResultImage()
        {
            double UserContourPathEditorDCElapsedMilliseconds = (DateTime.Now - UserContourPathDC.UserContourPathEditorDC.LastModified).TotalMilliseconds;
            double TransformedUserContourPathsDCElapsedMilliseconds = (DateTime.Now - UserContourPathDC.TransformedUserContourPathsDC.LastModified).TotalMilliseconds;

            if (UserContourPathEditorDCElapsedMilliseconds < timeout || TransformedUserContourPathsDCElapsedMilliseconds < timeout)
            {
                if (UserContourPathEditorDCElapsedMilliseconds < TransformedUserContourPathsDCElapsedMilliseconds)
                    resultImage.ToSettable().Value = UserContourPathDC.UserContourPathEditorDC.UserContourPathDrawerDC.ResultImage.Value;
                else
                    resultImage.ToSettable().Value = UserContourPathDC.TransformedUserContourPathsDC.TransformedUserContourPathsDrawerDC.ResultImage.Value;
            }
            else
                ContourDetectorDC.ContourDetectorResultDrawer.Draw();
        }

        public void PanZoomImageViewMouseDown(Point mousePositionOnImage, MouseButtonEventArgs e)
            => UserContourPathDC.PanZoomImageViewMouseDown(mousePositionOnImage, e);

        public void PanZoomImageViewMouseMove(Vector mouseVectorOnImage, Point mousePositionOnImage, MouseEventArgs e)
            => UserContourPathDC.PanZoomImageViewMouseMove(mouseVectorOnImage, mousePositionOnImage, e);

        public void PanZoomImageViewMouseUp(Point mousePositionOnImage, MouseButtonEventArgs e)
            => UserContourPathDC.PanZoomImageViewMouseUp(mousePositionOnImage, e);

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            ReferenceImagesDC.SaveSettings(settingsCollection);
            UserContourPathDC.SaveSettings(settingsCollection);
            ContourDetectorDC.SaveSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            ReferenceImagesDC.LoadSettings(settingsCollection);
            UserContourPathDC.LoadSettings(settingsCollection);
            ContourDetectorDC.LoadSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }
    }
}
