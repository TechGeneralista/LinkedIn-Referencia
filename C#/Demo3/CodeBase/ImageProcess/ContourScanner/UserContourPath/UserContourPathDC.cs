using Common.Language;
using Common.NotifyProperty;
using Common.Settings;
using ImageProcess.ContourScanner.UserContourPath.TransformedUserContourPaths;
using ImageProcess.ContourScanner.UserContourPath.UserContourPathEditor;
using ImageProcess.ReferenceImages;
using ImageProcess.Source;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace ImageProcess.ContourScanner.UserContourPath
{
    public class UserContourPathDC : ICanSaveLoadSettings
    {
        public UserContourPathEditorDC UserContourPathEditorDC { get; }
        public TransformedUserContourPathsDC TransformedUserContourPathsDC { get; }


        public UserContourPathDC(LanguageDC languageDC, ReferenceImagesDC referenceImagesDC, ImageProcessSourceDC imageProcessSourceDC, IReadOnlyProperty<ImageSource> resultImage)
        {
            UserContourPathEditorDC = new UserContourPathEditorDC(languageDC, referenceImagesDC, imageProcessSourceDC, resultImage);
            TransformedUserContourPathsDC = new TransformedUserContourPathsDC(languageDC, referenceImagesDC, UserContourPathEditorDC.UserContourPathDetector, UserContourPathEditorDC.UserLines, resultImage);
        }

        public void PanZoomImageViewMouseDown(Point mousePositionOnImage, MouseButtonEventArgs e)
            => UserContourPathEditorDC.PanZoomImageViewMouseDown(mousePositionOnImage, e);

        public void PanZoomImageViewMouseMove(Vector mouseVectorOnImage, Point mousePositionOnImage, MouseEventArgs e)
            => UserContourPathEditorDC.PanZoomImageViewMouseMove(mouseVectorOnImage, mousePositionOnImage, e);

        public void PanZoomImageViewMouseUp(Point mousePositionOnImage, MouseButtonEventArgs e)
            => UserContourPathEditorDC.PanZoomImageViewMouseUp(mousePositionOnImage, e);

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            UserContourPathEditorDC.SaveSettings(settingsCollection);
            TransformedUserContourPathsDC.SaveSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            UserContourPathEditorDC.LoadSettings(settingsCollection);
            TransformedUserContourPathsDC.LoadSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }
    }
}
