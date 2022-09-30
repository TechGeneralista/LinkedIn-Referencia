using Common;
using Common.NotifyProperty;
using Common.Settings;
using ImageProcess.Source;
using Language;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageProcess.ReferenceImages
{
    public class ReferenceImagesDC : ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public INonSettableObservableProperty<WriteableBitmap> ColorImage { get; } = new ObservableProperty<WriteableBitmap>();
        public INonSettableObservableProperty<WriteableBitmap> MonochromeImage { get; } = new ObservableProperty<WriteableBitmap>();


        readonly ISettableObservableProperty<ImageSource> mainDisplaySource;
        readonly IImageProcessSource source;


        public ReferenceImagesDC(LanguageDC languageDC, ISettableObservableProperty<ImageSource> mainDisplaySource, IImageProcessSource source)
        {
            LanguageDC = languageDC;
            this.mainDisplaySource = mainDisplaySource;
            this.source = source;

            Refresh();
        }

        public void Refresh()
        {
            source.Refresh();
            ColorImage.ForceSet(source.ColorImage.CurrentValue);
            MonochromeImage.ForceSet(source.MonochromeImage.CurrentValue);
            ShowColorImage();
        }

        internal void ShowColorImage() => mainDisplaySource.CurrentValue = ColorImage.CurrentValue;
        internal void ShowMonochromeImage() => mainDisplaySource.CurrentValue = MonochromeImage.CurrentValue;

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(ImageProcessSourceDC));
            settingsCollection.KeyCreator.AddNew(nameof(ColorImage));

            WriteableBitmap bitmap = ColorImage.CurrentValue;
            settingsCollection.KeyCreator.AddNew(nameof(bitmap.PixelWidth));
            settingsCollection.SetValue(bitmap.PixelWidth);
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.PixelHeight));
            settingsCollection.SetValue(bitmap.PixelHeight);
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.BackBufferStride));
            settingsCollection.SetValue(bitmap.BackBufferStride);
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.BackBuffer));
            settingsCollection.SetValue(bitmap.CopyPixelData());
            settingsCollection.KeyCreator.RemoveLast(2);

            settingsCollection.KeyCreator.AddNew(nameof(MonochromeImage));
            bitmap = MonochromeImage.CurrentValue;
            settingsCollection.KeyCreator.AddNew(nameof(bitmap.PixelWidth));
            settingsCollection.SetValue(bitmap.PixelWidth);
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.PixelHeight));
            settingsCollection.SetValue(bitmap.PixelHeight);
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.BackBufferStride));
            settingsCollection.SetValue(bitmap.BackBufferStride);
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.BackBuffer));
            settingsCollection.SetValue(bitmap.CopyPixelData());
            settingsCollection.KeyCreator.RemoveLast(3);
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(ImageProcessSourceDC));
            settingsCollection.KeyCreator.AddNew(nameof(ColorImage));

            WriteableBitmap bitmap = ColorImage.CurrentValue;
            settingsCollection.KeyCreator.AddNew(nameof(bitmap.PixelWidth));
            int pixelWidth = settingsCollection.GetValue<int>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.PixelHeight));
            int pixelHeight = settingsCollection.GetValue<int>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.BackBufferStride));
            int backBufferStride = settingsCollection.GetValue<int>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.BackBuffer));
            byte[] backBuffer = settingsCollection.GetValue<byte[]>();
            settingsCollection.KeyCreator.RemoveLast(2);

            bitmap = new WriteableBitmap(BitmapSource.Create(pixelWidth, pixelHeight, 96, 96, PixelFormats.Bgra32, null, backBuffer, backBufferStride));
            bitmap.Freeze();
            ColorImage.ForceSet(bitmap);

            settingsCollection.KeyCreator.AddNew(nameof(MonochromeImage));
            settingsCollection.KeyCreator.AddNew(nameof(bitmap.PixelWidth));
            pixelWidth = settingsCollection.GetValue<int>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.PixelHeight));
            pixelHeight = settingsCollection.GetValue<int>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.BackBufferStride));
            backBufferStride = settingsCollection.GetValue<int>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(bitmap.BackBuffer));
            backBuffer = settingsCollection.GetValue<byte[]>();
            settingsCollection.KeyCreator.RemoveLast(3);

            bitmap = new WriteableBitmap(BitmapSource.Create(pixelWidth, pixelHeight, 96, 96, PixelFormats.Bgra32, null, backBuffer, backBufferStride));
            bitmap.Freeze();
            MonochromeImage.ForceSet(bitmap);
        }
    }
}
