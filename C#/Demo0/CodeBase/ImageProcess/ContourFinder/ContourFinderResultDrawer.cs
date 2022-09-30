using Common;
using Common.NotifyProperty;
using ImageProcess.ContourFinder.Detector;
using ImageProcess.Source;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageProcess.ContourFinder
{
    public class ContourFinderResultDrawer
    {
        readonly double positionCorrection = .5;
        readonly Vector positionCorrectionVector;
        readonly ISettableObservableProperty<ImageSource> mainDisplaySource;


        public ContourFinderResultDrawer(ISettableObservableProperty<ImageSource> mainDisplaySource)
        {
            this.mainDisplaySource = mainDisplaySource;
            positionCorrectionVector = new Vector(positionCorrection, positionCorrection);
        }

        public void DrawResultImage(IImageProcessSource source, IDetectorResult result, double size)
        {
            WriteableBitmap bitmap = source.MonochromeImage.CurrentValue;
            double coordAxesLength = size / 4;
            double coordAxesWidth = (size / 40) / 2;
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

            foreach (IDetectorPositionResult positionResult in result.PositionResults.CurrentValue)
            {
                geometryGroupRedAxis.Children.Add(new LineGeometry(Point.Add(positionResult.AbsoluteCenter, positionCorrectionVector), Point.Add(new Point(positionResult.AbsoluteCenter.X + coordAxesLength, positionResult.AbsoluteCenter.Y).RotatePoint(positionResult.AbsoluteCenter, positionResult.Angle), positionCorrectionVector)));
                geometryGroupGreenAxis.Children.Add(new LineGeometry(Point.Add(positionResult.AbsoluteCenter, positionCorrectionVector), Point.Add(new Point(positionResult.AbsoluteCenter.X, positionResult.AbsoluteCenter.Y + coordAxesLength).RotatePoint(positionResult.AbsoluteCenter, positionResult.Angle), positionCorrectionVector)));
                geometryGroupBlueCircle.Children.Add(new EllipseGeometry(Point.Add(positionResult.AbsoluteCenter, positionCorrectionVector), circleRadius, circleRadius));
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

            mainDisplaySource.CurrentValue = drawingImage;
        }
    }
}