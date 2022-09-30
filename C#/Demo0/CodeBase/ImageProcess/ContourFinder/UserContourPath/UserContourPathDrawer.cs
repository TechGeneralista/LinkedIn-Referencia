using Common;
using Common.NotifyProperty;
using ImageProcess.ReferenceImages;
using ImageProcess.Source;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageProcess.ContourFinder.UserContourPath
{
    public class UserContourPathDrawer
    {
        readonly double positionCorrection = .5;
        readonly double ellipseSize = .25;
        readonly Vector positionCorrectionVector;
        readonly ISettableObservableProperty<ImageSource> mainDisplaySource;


        public UserContourPathDrawer(ISettableObservableProperty<ImageSource> mainDisplaySource)
        {
            this.mainDisplaySource = mainDisplaySource;
            positionCorrectionVector = new Vector(positionCorrection, positionCorrection);
        }

        internal void DrawLinesAndPointPairs(ReferenceImagesDC referenceImagesDC, UserLineDC[] userLines, UserLineDC selectedUserLine)
        {
            BitmapSource bitmap = referenceImagesDC.MonochromeImage.CurrentValue;
            DrawingGroup drawingGroup = new DrawingGroup();
            ImageDrawing imageDrawing = new ImageDrawing(bitmap, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

            GeometryDrawing geometryDrawingLines = new GeometryDrawing();
            geometryDrawingLines.Pen = new Pen(Brushes.Orange, 1);
            GeometryDrawing geometryDrawingSelectedLine = new GeometryDrawing();
            geometryDrawingSelectedLine.Pen = new Pen(Brushes.Yellow, .5);
            GeometryDrawing geometryDrawingLinePoints = new GeometryDrawing();
            geometryDrawingLinePoints.Pen = new Pen(Brushes.Yellow, .5);
            GeometryDrawing geometryDrawingSamplePointsBrighter = new GeometryDrawing();
            geometryDrawingSamplePointsBrighter.Pen = new Pen(Brushes.SkyBlue, .5);
            GeometryDrawing geometryDrawingSamplePointsDarker = new GeometryDrawing();
            geometryDrawingSamplePointsDarker.Pen = new Pen(Brushes.Blue, .5);

            GeometryGroup geometryGroupSelectedLine = new GeometryGroup();
            GeometryGroup geometryGroupLines = new GeometryGroup();
            GeometryGroup geometryGroupLinePoints = new GeometryGroup();
            GeometryGroup geometryGroupSamplePointsBrighter = new GeometryGroup();
            GeometryGroup geometryGroupSamplePointsDarker = new GeometryGroup();

            //Draw selected line
            if (selectedUserLine.IsNotNull())
            {
                geometryGroupSelectedLine.Children.Add(new LineGeometry(Point.Add(selectedUserLine.StartPoint.CurrentValue, positionCorrectionVector), Point.Add(selectedUserLine.EndPoint.CurrentValue, positionCorrectionVector)));
            }

            //Draw other lines
            foreach (UserLineDC userLineDC in userLines)
            {
                if (selectedUserLine.IsNotNull() && selectedUserLine == userLineDC)
                    continue;

                geometryGroupLines.Children.Add(new LineGeometry(Point.Add(userLineDC.StartPoint.CurrentValue, positionCorrectionVector), Point.Add(userLineDC.EndPoint.CurrentValue, positionCorrectionVector)));
            }

            //Draw lines start/end points
            foreach (UserLineDC userLineDC in userLines)
            {
                geometryGroupLinePoints.Children.Add(new EllipseGeometry(Point.Add(userLineDC.StartPoint.CurrentValue, positionCorrectionVector), ellipseSize, ellipseSize));
                geometryGroupLinePoints.Children.Add(new EllipseGeometry(Point.Add(userLineDC.EndPoint.CurrentValue, positionCorrectionVector), ellipseSize, ellipseSize));
            }

            //Draw sample points
            foreach (UserLineDC userLineDC in userLines)
            {
                foreach (PointPair pointPair in userLineDC.PointPairs)
                {
                    geometryGroupSamplePointsBrighter.Children.Add(new EllipseGeometry(Point.Add(pointPair.Brighter, positionCorrectionVector), ellipseSize, ellipseSize));
                    geometryGroupSamplePointsDarker.Children.Add(new EllipseGeometry(Point.Add(pointPair.Darker, positionCorrectionVector), ellipseSize, ellipseSize));
                }
            }

            geometryDrawingSelectedLine.Geometry = geometryGroupSelectedLine;
            geometryDrawingLines.Geometry = geometryGroupLines;
            geometryDrawingLinePoints.Geometry = geometryGroupLinePoints;
            geometryDrawingSamplePointsBrighter.Geometry = geometryGroupSamplePointsBrighter;
            geometryDrawingSamplePointsDarker.Geometry = geometryGroupSamplePointsDarker;

            drawingGroup.Children.Add(imageDrawing);
            drawingGroup.Children.Add(geometryDrawingLines);
            drawingGroup.Children.Add(geometryDrawingSelectedLine);
            drawingGroup.Children.Add(geometryDrawingLinePoints);
            drawingGroup.Children.Add(geometryDrawingSamplePointsBrighter);
            drawingGroup.Children.Add(geometryDrawingSamplePointsDarker);

            DrawingImage drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            mainDisplaySource.CurrentValue = drawingImage;
        }

        internal void DrawLinesAndAxes(ReferenceImagesDC referenceImagesDC, UserLineDC[] userLines, Point center, double size, int minimumAngle, int maximumAngle)
        {
            double coordAxesLength = size / 4;
            double coordAxesWidth = size / 80;
            double linesWidth = coordAxesWidth / 2;

            BitmapSource bitmap = referenceImagesDC.MonochromeImage.CurrentValue;
            DrawingGroup drawingGroup = new DrawingGroup();
            ImageDrawing imageDrawing = new ImageDrawing(bitmap, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

            GeometryDrawing geometryDrawingOrange = new GeometryDrawing();
            geometryDrawingOrange.Pen = new Pen(Brushes.Orange, linesWidth);
            GeometryDrawing geometryDrawingRed = new GeometryDrawing();
            geometryDrawingRed.Pen = new Pen(Brushes.Red, coordAxesWidth);
            GeometryDrawing geometryDrawingGreen = new GeometryDrawing();
            geometryDrawingGreen.Pen = new Pen(Brushes.Green, coordAxesWidth);
            GeometryDrawing geometryDrawingBlue = new GeometryDrawing();
            geometryDrawingBlue.Pen = new Pen(Brushes.DeepSkyBlue, coordAxesWidth);

            GeometryGroup geometryGroupLines = new GeometryGroup();
            GeometryGroup geometryGroupRedAxis = new GeometryGroup();
            GeometryGroup geometryGroupGreenAxis = new GeometryGroup();

            foreach (UserLineDC userLineDC in userLines)
            {
                geometryGroupLines.Children.Add(new LineGeometry(Point.Add(userLineDC.StartPoint.CurrentValue.RotatePoint(center, minimumAngle), positionCorrectionVector), Point.Add(userLineDC.EndPoint.CurrentValue.RotatePoint(center, minimumAngle), positionCorrectionVector)));
                geometryGroupLines.Children.Add(new LineGeometry(Point.Add(userLineDC.StartPoint.CurrentValue.RotatePoint(center, maximumAngle), positionCorrectionVector), Point.Add(userLineDC.EndPoint.CurrentValue.RotatePoint(center, maximumAngle), positionCorrectionVector)));
            }

            geometryGroupRedAxis.Children.Add(new LineGeometry(Point.Add(center.RotatePoint(center, minimumAngle), positionCorrectionVector), Point.Add(new Point(center.X + coordAxesLength, center.Y).RotatePoint(center, minimumAngle), positionCorrectionVector)));
            geometryGroupRedAxis.Children.Add(new LineGeometry(Point.Add(center.RotatePoint(center, maximumAngle), positionCorrectionVector), Point.Add(new Point(center.X + coordAxesLength, center.Y).RotatePoint(center, maximumAngle), positionCorrectionVector)));

            geometryGroupGreenAxis.Children.Add(new LineGeometry(Point.Add(center.RotatePoint(center, minimumAngle), positionCorrectionVector), Point.Add(new Point(center.X, center.Y + coordAxesLength).RotatePoint(center, minimumAngle), positionCorrectionVector)));
            geometryGroupGreenAxis.Children.Add(new LineGeometry(Point.Add(center.RotatePoint(center, maximumAngle), positionCorrectionVector), Point.Add(new Point(center.X, center.Y + coordAxesLength).RotatePoint(center, maximumAngle), positionCorrectionVector)));

            geometryDrawingOrange.Geometry = geometryGroupLines;
            geometryDrawingRed.Geometry = geometryGroupRedAxis;
            geometryDrawingGreen.Geometry = geometryGroupGreenAxis;
            geometryDrawingBlue.Geometry = new EllipseGeometry(Point.Add(center, positionCorrectionVector), linesWidth, linesWidth);

            drawingGroup.Children.Add(imageDrawing);
            drawingGroup.Children.Add(geometryDrawingOrange);
            drawingGroup.Children.Add(geometryDrawingRed);
            drawingGroup.Children.Add(geometryDrawingGreen);
            drawingGroup.Children.Add(geometryDrawingBlue);

            DrawingImage drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            mainDisplaySource.CurrentValue = drawingImage;
        }
    }
}
