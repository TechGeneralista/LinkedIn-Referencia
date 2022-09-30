using AppLog;
using Common.Settings;
using CustomControl.ImageViewControl;
using CustomControl.PopupWindow;
using Language;
using OpenCLWrapper;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using UniCamApp.Source;


namespace UniCamApp
{
    public class AppDC : ICanHandleImageViewControlMouseEvents
    {
        public string Title { get; }
        public SourceDC SourceDC { get; }
        public AppLogDC AppLogDC { get; }


        readonly string name = "UniCam";
        readonly string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();


        public AppDC(ISettingsCollection settingsCollection, LanguageDC languageDC, OpenCLAccelerator openCLAccelerator, PopupWindowDC popupWindowDC)
        {
            Title = string.Format("{0} V{1}", name, version.Substring(0,version.Length-2));

            AppLogDC = new AppLogDC(languageDC, 500);
            SourceDC = new SourceDC(languageDC, popupWindowDC, openCLAccelerator, AppLogDC, settingsCollection, Title);

            AppLogDC.NewMessage(LogTypes.Successful, languageDC.ApplicationStarted.CurrentValue);
        }

        public void ImageViewControlMouseDown(Point downPosition, MouseButtonEventArgs e) => (SourceDC as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseDown(downPosition, e);
        public void ImageViewControlMouseMove(Vector moveVector, Point movePosition, MouseEventArgs e) => (SourceDC as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseMove(moveVector, movePosition, e);
        public void ImageViewControlMouseUp(Point upPosition, MouseButtonEventArgs e) => (SourceDC as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseUp(upPosition, e);
    }
}
