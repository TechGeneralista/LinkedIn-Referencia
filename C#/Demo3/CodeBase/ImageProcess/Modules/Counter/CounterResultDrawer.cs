using Common;
using Common.NotifyProperty;
using ImageProcess.ContourScanner.ContourDetector;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageProcess.Modules.Counter
{
    public class CounterResultDrawer
    {
        readonly double positionCorrection = .5;
        readonly DetectorResultDC detectorResultDC;
        readonly IReadOnlyProperty<ImageSource> resultImage;
        readonly IReadOnlyProperty<bool?> result;


        public CounterResultDrawer(DetectorResultDC detectorResultDC, IReadOnlyProperty<ImageSource> resultImage, IReadOnlyProperty<bool?> result)
        {
            this.detectorResultDC = detectorResultDC;
            this.resultImage = resultImage;
            this.result = result;
        }

        public void Draw()
        {
            if (detectorResultDC.IsNull())
                return;

            double textSize = detectorResultDC.Size / 2.3;
            WriteableBitmap bitmap = detectorResultDC.ImageProcessSourceDC.MonochromeImage.Value;
            DrawingGroup drawingGroup = new DrawingGroup();
            ImageDrawing imageDrawing = new ImageDrawing(bitmap, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

            GeometryDrawing geometryDrawingBlue = new GeometryDrawing();
            geometryDrawingBlue.Pen = new Pen(Brushes.DeepSkyBlue, detectorResultDC.Size / 50);
            GeometryDrawing geometryDrawingRed = new GeometryDrawing();
            geometryDrawingRed.Pen = new Pen(Brushes.Red, detectorResultDC.Size / 50);
            GeometryDrawing geometryDrawingOrange = new GeometryDrawing();
            geometryDrawingOrange.Pen = new Pen(Brushes.Orange, detectorResultDC.Size / 50);
            GeometryDrawing geometryDrawingGreen = new GeometryDrawing();
            geometryDrawingGreen.Pen = new Pen(Brushes.Green, detectorResultDC.Size / 50);

            GeometryGroup geometryGroupRectangles = new GeometryGroup();
            GeometryGroup geometryGroupNumbers = new GeometryGroup();

            foreach (AverageCenterAngleCalculator dpr in detectorResultDC.PositionResults.Value)
            {
                geometryGroupRectangles.Children.Add(new RectangleGeometry(new Rect(dpr.AbsoluteCenter.X - (detectorResultDC.Size / 2) + positionCorrection, dpr.AbsoluteCenter.Y - (detectorResultDC.Size / 2) + positionCorrection, detectorResultDC.Size, detectorResultDC.Size)));

                FormattedText text = new FormattedText(string.Format("{0}.", dpr.Index), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), textSize, Brushes.Transparent);
                geometryGroupNumbers.Children.Add(text.BuildGeometry(new Point(dpr.AbsoluteCenter.X - (text.Width / 2) + positionCorrection, dpr.AbsoluteCenter.Y - (text.Height / 2) + positionCorrection)));
            }

            drawingGroup.Children.Add(imageDrawing);
            geometryDrawingBlue.Geometry = geometryGroupRectangles;
            drawingGroup.Children.Add(geometryDrawingBlue);

            if (result.Value.IsNull())
            {
                geometryDrawingOrange.Geometry = geometryGroupNumbers;
                drawingGroup.Children.Add(geometryDrawingOrange);
            }
            else
            {
                if ((bool)result.Value)
                {
                    geometryDrawingGreen.Geometry = geometryGroupNumbers;
                    drawingGroup.Children.Add(geometryDrawingGreen);
                }
                else
                {
                    geometryDrawingRed.Geometry = geometryGroupNumbers;
                    drawingGroup.Children.Add(geometryDrawingRed);
                }
            }

            DrawingImage drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            resultImage.ToSettable().Value = drawingImage;
        }
    }
}
