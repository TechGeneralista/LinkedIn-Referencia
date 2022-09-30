using Common.NotifyProperty;
using ImageCaptureDevice;
using ImageProcess.OpticalDistortionCorrection;
using ImageProcess.Templates;
using Common.Language;
using OpenCLWrapper;
using System.Windows.Media.Imaging;
using OpenCLWrapper.Buffer;
using Common.Settings;
using Common;

namespace ImageProcess.Source
{
    public class ImageSourceDeviceDC : ICanSaveLoadSettings
    {
        public IImageCaptureDevice ImageCaptureDevice { get; }
        public OpticalDistortionCorrectionDC OpticalDistortionCorrectionDC { get; }
        public Image2DBuffer ColorImageBuffer => OpticalDistortionCorrectionDC.CorrectedImageBuffer;
        public IReadOnlyProperty<WriteableBitmap> ColorImage { get; } = new Property<WriteableBitmap>();


        readonly DataBuffer<byte> dataBuffer;
        readonly FormatConvert formatConvert;
        readonly FlipVertical flipVertical;


        public ImageSourceDeviceDC(LanguageDC languageDC, OpenCLAccelerator openCLAccelerator, IImageCaptureDevice imageCaptureDevice)
        {
            ImageCaptureDevice = imageCaptureDevice;
            dataBuffer = new DataBuffer<byte>(openCLAccelerator);
            formatConvert = new FormatConvert(openCLAccelerator);
            flipVertical = new FlipVertical(openCLAccelerator);
            OpticalDistortionCorrectionDC = new OpticalDistortionCorrectionDC(languageDC, openCLAccelerator);
        }

        public void Start() => ImageCaptureDevice.Start();

        public void Capture()
        {
            ImageCaptureDevice.Capture();

            if(ImageCaptureDevice.IsRun.Value)
            {
                dataBuffer.Download(ImageCaptureDevice.Output.Buffer, ImageCaptureDevice.Output.BufferLength);
                formatConvert.ConvertBGR24ToBGRA32(ImageCaptureDevice.Output.Width, ImageCaptureDevice.Output.Height, ImageCaptureDevice.Output.Stride, dataBuffer);
                flipVertical.Flip(formatConvert.Output);
                OpticalDistortionCorrectionDC.Correct(flipVertical.Output);
                ColorImage.ToSettable().Value = ColorImageBuffer.Upload();
            }
        }

        public void Stop()
        {
            ImageCaptureDevice.Stop();
            ColorImage.ToSettable().Value = null;
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(nameof(ImageSourceDeviceDC));
            ImageCaptureDevice.CastTo<ICanSaveLoadSettings>().SaveSettings(settingsCollection);
            OpticalDistortionCorrectionDC.SaveSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(nameof(ImageSourceDeviceDC));
            ImageCaptureDevice.CastTo<ICanSaveLoadSettings>().LoadSettings(settingsCollection);
            OpticalDistortionCorrectionDC.LoadSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }
    }
}
