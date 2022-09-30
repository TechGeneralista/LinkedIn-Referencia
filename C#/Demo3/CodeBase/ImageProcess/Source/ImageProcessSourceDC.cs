using Common.NotifyProperty;
using ImageCaptureDevice;
using ImageProcess.Templates;
using Common.Language;
using System.Windows.Media.Imaging;
using OpenCLWrapper;
using OpenCLWrapper.Buffer;
using Common.Settings;


namespace ImageProcess.Source
{
    public class ImageProcessSourceDC : ImageSourceDeviceDC, ICanSaveLoadSettings
    {
        public Image2DBuffer MonochromeImageBuffer => monochrome.Output;
        public IReadOnlyProperty<WriteableBitmap> MonochromeImage { get; } = new Property<WriteableBitmap>();


        readonly Monochrome monochrome;


        public ImageProcessSourceDC(LanguageDC languageDC, OpenCLAccelerator openCLAccelerator, IImageCaptureDevice imageCaptureDevice) 
            : base(languageDC, openCLAccelerator, imageCaptureDevice)
        {
            monochrome = new Monochrome(openCLAccelerator);
        }

        public new void Start() => base.Start();

        public new void Capture()
        {
            base.Capture();

            if (ImageCaptureDevice.IsRun.Value)
            {
                monochrome.Convert(ColorImageBuffer);
                MonochromeImage.ToSettable().Value = monochrome.Output.Upload();
            }
        }

        public new void Stop()
        {
            base.Stop();
            MonochromeImage.ToSettable().Value = null;
        }

        public new void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(nameof(ImageProcessSourceDC));
            base.SaveSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }

        public new void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(nameof(ImageProcessSourceDC));
            base.LoadSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }
    }
}
