using Common;
using Common.NotifyProperty;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageProcess.ContourScanner.ContourDetector
{
    public class ContourDetectorResultDrawer
    {
        public IReadOnlyProperty<ImageSource> ResultImage { get; }


        readonly double positionCorrection = .5;
        readonly Vector positionCorrectionVector;
        readonly DetectorResultDC detectorResultDC;


        public ContourDetectorResultDrawer(DetectorResultDC detectorResultDC, IReadOnlyProperty<ImageSource> resultImage)
        {
            this.detectorResultDC = detectorResultDC;
            ResultImage = resultImage;

            positionCorrectionVector = new Vector(positionCorrection, positionCorrection);
        }

        public void Draw()
        {
            WriteableBitmap bitmap = detectorResultDC.ImageProcessSourceDC.MonochromeImage.Value;
            double coordAxesLength = detectorResultDC.Size / 4;
            double coordAxesWidth = (detectorResultDC.Size / 40) / 2;
            double circleWidth = coordAxesWidth / 2;
            double circleRadius = coordAxesLength / 10;

            DrawingGroup drawingGroup = new DrawingGroup();
            ImageDrawing imageDrawing = new ImageDrawing(bitmap, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

            GeometryDrawing geometryDrawingRed = new GeometryDrawing();
            geometryDrawingRed.Pen = new Pen(Brushes.Red, coordAxesWidth);
            GeometryDrawing geometryDrawingGreen = new GeometryDrawing();
            geometryDrawingGreen.Pen = new Pen(Brushes.Green, coordAxesWidth);
            GeometryDrawing geometryDrawingBlue = new GeometryDrawing();
            geometryDrawingBlue.Pen = new Pen(Brushes.DeepSkyBlue, circleWidth);

            GeometryGroup geometryGroupRedAxis = new GeometryGroup();
            GeometryGroup geometryGroupGreenAxis = new GeometryGroup();
            GeometryGroup geometryGroupBlueCircle = new GeometryGroup();

            foreach (AverageCenterAngleCalculator acac in detectorResultDC.PositionResults.Value)
            {
                geometryGroupRedAxis.Children.Add(new LineGeometry(Point.Add(acac.AbsoluteCenter, positionCorrectionVector), Point.Add(new Point(acac.AbsoluteCenter.X + coordAxesLength, acac.AbsoluteCenter.Y).RotatePoint(acac.AbsoluteCenter, acac.Angle), positionCorrectionVector)));
                geometryGroupGreenAxis.Children.Add(new LineGeometry(Point.Add(acac.AbsoluteCenter, positionCorrectionVector), Point.Add(new Point(acac.AbsoluteCenter.X, acac.AbsoluteCenter.Y + coordAxesLength).RotatePoint(acac.AbsoluteCenter, acac.Angle), positionCorrectionVector)));
                geometryGroupBlueCircle.Children.Add(new EllipseGeometry(Point.Add(acac.AbsoluteCenter, positionCorrectionVector), circleRadius, circleRadius));
            }

            geometryDrawingRed.Geometry = geometryGroupRedAxis;
            geometryDrawingGreen.Geometry = geometryGroupGreenAxis;
            geometryDrawingBlue.Geometry = geometryGroupBlueCircle;

            drawingGroup.Children.Add(imageDrawing);
            drawingGroup.Children.Add(geometryDrawingBlue);
            drawingGroup.Children.Add(geometryDrawingRed);
            drawingGroup.Children.Add(geometryDrawingGreen);

            DrawingImage drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            ResultImage.ToSettable().Value = drawingImage;
        }
    }
}