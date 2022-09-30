using Common;
using Common.Language;
using Common.NotifyProperty;
using Common.Settings;
using ImageProcess.Source;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageProcess.ReferenceImages
{
    public class ReferenceImagesDC : ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public IReadOnlyProperty<WriteableBitmap> ColorImage { get; } = new Property<WriteableBitmap>();
        public IReadOnlyProperty<WriteableBitmap> MonochromeImage { get; } = new Property<WriteableBitmap>();


        readonly IReadOnlyProperty<ImageSource> resultImage;
        readonly ImageProcessSourceDC imageProcessSourceDC;


        public ReferenceImagesDC(LanguageDC languageDC, ImageProcessSourceDC imageProcessSourceDC, IReadOnlyProperty<ImageSource> resultImage)
        {
            LanguageDC = languageDC;
            this.imageProcessSourceDC = imageProcessSourceDC;
            this.resultImage = resultImage;

            Refresh();
        }

        public void Refresh()
        {
            imageProcessSourceDC.Capture();
            ColorImage.ToSettable().Value = imageProcessSourceDC.ColorImage.Value;
            MonochromeImage.ToSettable().Value = imageProcessSourceDC.MonochromeImage.Value;
            ShowColorImage();
        }

        internal void ShowColorImage() => resultImage.ToSettable().Value = ColorImage.Value;
        internal void ShowMonochromeImage() => resultImage.ToSettable().Value = MonochromeImage.Value;

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);

            WriteableBitmap bitmap = ColorImage.Value;
            settingsCollection.AddKey(nameof(ColorImage));
            settingsCollection.SetProperty(bitmap.PixelWidth, nameof(bitmap.PixelWidth));
            settingsCollection.SetProperty(bitmap.PixelHeight, nameof(bitmap.PixelHeight));
            settingsCollection.SetProperty(bitmap.BackBufferStride, nameof(bitmap.BackBufferStride));
            settingsCollection.SetProperty(bitmap.CopyPixelData(), nameof(bitmap.BackBuffer));

            bitmap = MonochromeImage.Value;
            settingsCollection.ReplaceLastKey(nameof(MonochromeImage));
            settingsCollection.SetProperty(bitmap.PixelWidth, nameof(bitmap.PixelWidth));
            settingsCollection.SetProperty(bitmap.PixelHeight, nameof(bitmap.PixelHeight));
            settingsCollection.SetProperty(bitmap.BackBufferStride, nameof(bitmap.BackBufferStride));
            settingsCollection.SetProperty(bitmap.CopyPixelData(), nameof(bitmap.BackBuffer));

            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);

            WriteableBitmap bitmap;
            settingsCollection.AddKey(nameof(ColorImage));
            int pixelWidth = settingsCollection.GetProperty<int>(nameof(bitmap.PixelWidth));
            int pixelHeight = settingsCollection.GetProperty<int>(nameof(bitmap.PixelHeight));
            int backBufferStride = settingsCollection.GetProperty<int>(nameof(bitmap.BackBufferStride));
            byte[] backBuffer = settingsCollection.GetProperty<byte[]>(nameof(bitmap.BackBuffer));

            bitmap = new WriteableBitmap(BitmapSource.Create(pixelWidth, pixelHeight, 96, 96, PixelFormats.Bgra32, null, backBuffer, backBufferStride));
            bitmap.Freeze();
            ColorImage.ToSettable().Value = bitmap;

            settingsCollection.ReplaceLastKey(nameof(MonochromeImage));
            pixelWidth = settingsCollection.GetProperty<int>(nameof(bitmap.PixelWidth));
            pixelHeight = settingsCollection.GetProperty<int>(nameof(bitmap.PixelHeight));
            backBufferStride = settingsCollection.GetProperty<int>(nameof(bitmap.BackBufferStride));
            backBuffer = settingsCollection.GetProperty<byte[]>(nameof(bitmap.BackBuffer));

            bitmap = new WriteableBitmap(BitmapSource.Create(pixelWidth, pixelHeight, 96, 96, PixelFormats.Bgra32, null, backBuffer, backBufferStride));
            bitmap.Freeze();
            MonochromeImage.ToSettable().Value = bitmap;

            settingsCollection.ExitPoint();
        }
    }
}
