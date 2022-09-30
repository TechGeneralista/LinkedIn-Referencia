using Common;
using Common.NotifyProperty;
using ImageProcess.ContourFinder.Detector;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageProcess.Modules.Counter
{
    public class CounterResultDrawer
    {
        readonly double positionCorrection = .5;
        readonly ISettableObservableProperty<ImageSource> mainDisplaySource;

        public CounterResultDrawer(ISettableObservableProperty<ImageSource> mainDisplaySource)
        {
            this.mainDisplaySource = mainDisplaySource;
        }

        internal void Draw(IDetectorResult detectorResult, bool? counterResult)
        {
            if (detectorResult.IsNull())
                return;

            double textSize = detectorResult.Size / 2.3;
            WriteableBitmap bitmap = detectorResult.ImageProcessSource.MonochromeImage.CurrentValue;
            DrawingGroup drawingGroup = new DrawingGroup();
            ImageDrawing imageDrawing = new ImageDrawing(bitmap, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

            GeometryDrawing geometryDrawingBlue = new GeometryDrawing();
            geometryDrawingBlue.Pen = new Pen(Brushes.DeepSkyBlue, detectorResult.Size / 50);
            GeometryDrawing geometryDrawingRed = new GeometryDrawing();
            geometryDrawingRed.Pen = new Pen(Brushes.Red, detectorResult.Size / 50);
            GeometryDrawing geometryDrawingGreen = new GeometryDrawing();
            geometryDrawingGreen.Pen = new Pen(Brushes.Green, detectorResult.Size / 50);

            GeometryGroup geometryGroupRectangles = new GeometryGroup();
            GeometryGroup geometryGroupNumbers = new GeometryGroup();

            foreach(IDetectorPositionResult dpr in detectorResult.PositionResults.CurrentValue)
            {
                geometryGroupRectangles.Children.Add(new RectangleGeometry(new Rect(dpr.AbsoluteCenter.X - (detectorResult.Size / 2) + positionCorrection, dpr.AbsoluteCenter.Y - (detectorResult.Size / 2) + positionCorrection, detectorResult.Size, detectorResult.Size)));

                FormattedText text = new FormattedText(string.Format("{0}.", dpr.Index), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), textSize, Brushes.Transparent);
                geometryGroupNumbers.Children.Add(text.BuildGeometry(new Point(dpr.AbsoluteCenter.X - (text.Width/2) + positionCorrection, dpr.AbsoluteCenter.Y - (text.Height/2) + positionCorrection)));
            }

            geometryDrawingBlue.Geometry = geometryGroupRectangles;

            if (counterResult.IsNotNull())
            {
                if ((bool)counterResult)
                    geometryDrawingGreen.Geometry = geometryGroupNumbers;
                else
                    geometryDrawingRed.Geometry = geometryGroupNumbers;
            }

            drawingGroup.Children.Add(imageDrawing);
            drawingGroup.Children.Add(geometryDrawingBlue);
            drawingGroup.Children.Add(geometryDrawingGreen);
            drawingGroup.Children.Add(geometryDrawingRed);

            DrawingImage drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            mainDisplaySource.CurrentValue = drawingImage;
        }
    }
}
