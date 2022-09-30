using Common;
using Common.NotifyProperty;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace UCVisionResultExplorerApp
{
    public class ResultDataImageDC
    {
        public BitmapSource ResultImage { get; }
        public string ResultText { get; }
        public string TextForegroundColor { get; }
        public IReadOnlyProperty<int> ImageHeight { get; } = new Property<int>();


        readonly string imageFilePath;


        public ResultDataImageDC(string imageFilePath, int imageHeight)
        {
            this.imageFilePath = imageFilePath;
            ImageHeight.ToSettable().Value = imageHeight;

            ResultImage = Utils.LoadImageFromFile(imageFilePath, true);
            ResultText = Path.GetFileName(imageFilePath).Replace(".jpg", string.Empty).Replace('_','/');

            if (ResultText.Contains("Ok"))
                TextForegroundColor = Colors.Green.ToString();

            else if (ResultText.Contains("Nok"))
                TextForegroundColor = Colors.Red.ToString();

            else
                TextForegroundColor = Colors.Black.ToString();
        }

        internal void OpenImage()
        {
            if (File.Exists(imageFilePath))
                Process.Start(imageFilePath);
        }

        internal void SetHeight(int n)
            => ImageHeight.ToSettable().Value = n;

        internal bool CheckFilter(string text)
            => ResultText.ToLower().Contains(text.ToLower());
    }
}