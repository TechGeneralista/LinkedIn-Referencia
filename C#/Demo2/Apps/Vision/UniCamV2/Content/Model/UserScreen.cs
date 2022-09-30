using Common;
using Common.NotifyProperty;
using ImageProcess.ObjectDetection;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace UniCamV2.Content.Model
{
    public class UserScreen
    {
        public ISettableObservableProperty<ImageSource> Display { get; } = new ObservableProperty<ImageSource>();


        readonly double positionCorrection = .5;


        public void ObjectContourSetup(WriteableBitmap bitmap, ContourLine[] lines, ContourLine selectedLine)
        {
            double ellipseSize = .25;

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

            //Draw selected line
            if (selectedLine.IsNotNull())
                geometryDrawingSelectedLine.Geometry = new LineGeometry(selectedLine.StartPoint.Value.Add(positionCorrection), selectedLine.EndPoint.Value.Add(positionCorrection));

            GeometryGroup geometryGroupLines = new GeometryGroup();
            GeometryGroup geometryGroupLinePoints = new GeometryGroup();
            GeometryGroup geometryGroupSamplePointsBrighter = new GeometryGroup();
            GeometryGroup geometryGroupSamplePointsDarker = new GeometryGroup();

            //Draw other lines
            foreach (ContourLine cl in lines)
            {
                if (selectedLine.IsNotNull() && selectedLine == cl)
                    continue;

                geometryGroupLines.Children.Add(new LineGeometry(cl.StartPoint.Value.Add(positionCorrection), cl.EndPoint.Value.Add(positionCorrection)));
            }

            //Draw lines start/end points
            foreach (ContourLine cl in lines)
            {
                geometryGroupLinePoints.Children.Add(new EllipseGeometry(cl.StartPoint.Value.Add(positionCorrection), ellipseSize, ellipseSize));
                geometryGroupLinePoints.Children.Add(new EllipseGeometry(cl.EndPoint.Value.Add(positionCorrection), ellipseSize, ellipseSize));
            }

            //Draw sample points
            foreach (ContourLine cl in lines)
            {
                foreach (ContourSamplePointPair spp in cl.ParallelSamplingPointCalculator.SamplePointPairs)
                {
                    geometryGroupSamplePointsBrighter.Children.Add(new EllipseGeometry(spp.Brighter.Add(positionCorrection), ellipseSize, ellipseSize));
                    geometryGroupSamplePointsDarker.Children.Add(new EllipseGeometry(spp.Darker.Add(positionCorrection), ellipseSize, ellipseSize));
                }
            }

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

            Display.Value = drawingImage;
        }

        internal void DrawDetectedObjects(WriteableBitmap bitmap, ContourFinder contourFinder)
        {
            double rectangleWidth = contourFinder.RectangleSize.Width / 40;
            double coordAxesLength = contourFinder.RectangleSize.Width / 4;
            double coordAxesWidth = rectangleWidth / 2;
            double linesWidth = coordAxesWidth / 2;

            ImageDrawing imageDrawing = new ImageDrawing(bitmap, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(imageDrawing);

            GeometryDrawing geometryDrawingOrange = new GeometryDrawing();
            geometryDrawingOrange.Pen = new Pen(Brushes.Orange, linesWidth);
            GeometryDrawing geometryDrawingRed = new GeometryDrawing();
            geometryDrawingRed.Pen = new Pen(Brushes.Red, coordAxesWidth);
            GeometryDrawing geometryDrawingGreen = new GeometryDrawing();
            geometryDrawingGreen.Pen = new Pen(Brushes.Green, coordAxesWidth);
            GeometryDrawing geometryDrawingBlue = new GeometryDrawing();
            geometryDrawingBlue.Pen = new Pen(Brushes.DeepSkyBlue, rectangleWidth);

            GeometryGroup geometryGroupRed = new GeometryGroup();
            GeometryGroup geometryGroupGreen = new GeometryGroup();
            GeometryGroup geometryGroupBlue = new GeometryGroup();

            foreach(ContourFinderCenterPoint p in contourFinder.Results[0].Centers)
            {
                geometryGroupRed.Children.Add(new LineGeometry(p.DetectedCenterPoint.Add(positionCorrection), new Point(p.DetectedCenterPoint.X + coordAxesLength, p.DetectedCenterPoint.Y).Add(positionCorrection)));
                geometryGroupGreen.Children.Add(new LineGeometry(p.DetectedCenterPoint.Add(positionCorrection), new Point(p.DetectedCenterPoint.X, p.DetectedCenterPoint.Y + coordAxesLength).Add(positionCorrection)));

                Rect rect = Utils.CreateRect(p.DetectedCenterPoint, contourFinder.RectangleSize);
                geometryGroupBlue.Children.Add(new RectangleGeometry(new Rect(rect.X + positionCorrection, rect.Y + positionCorrection, rect.Width, rect.Height)));
            }

            geometryDrawingRed.Geometry = geometryGroupRed;
            geometryDrawingGreen.Geometry = geometryGroupGreen;
            geometryDrawingBlue.Geometry = geometryGroupBlue;

            drawingGroup.Children.Add(geometryDrawingOrange);
            drawingGroup.Children.Add(geometryDrawingRed);
            drawingGroup.Children.Add(geometryDrawingGreen);
            drawingGroup.Children.Add(geometryDrawingBlue);

            DrawingImage drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            Display.Value = drawingImage;
        }

        internal void DrawReferenceObject(WriteableBitmap bitmap, ContourLine[] lines, UserContour pointPairGroup)
        {
            double rectangleWidth = pointPairGroup.Rectangle.Width / 40;
            double coordAxesLength = pointPairGroup.Rectangle.Width / 4;
            double coordAxesWidth = rectangleWidth / 2;
            double linesWidth = coordAxesWidth / 2;

            ImageDrawing imageDrawing = new ImageDrawing(bitmap, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(imageDrawing);

            GeometryDrawing geometryDrawingOrange = new GeometryDrawing();
            geometryDrawingOrange.Pen = new Pen(Brushes.Orange, linesWidth);
            GeometryDrawing geometryDrawingRed = new GeometryDrawing();
            geometryDrawingRed.Pen = new Pen(Brushes.Red, coordAxesWidth);
            GeometryDrawing geometryDrawingGreen = new GeometryDrawing();
            geometryDrawingGreen.Pen = new Pen(Brushes.Green, coordAxesWidth);
            GeometryDrawing geometryDrawingBlue = new GeometryDrawing();
            geometryDrawingBlue.Pen = new Pen(Brushes.DeepSkyBlue, rectangleWidth);

            GeometryGroup geometryGroupLines = new GeometryGroup();
            foreach (ContourLine cl in lines)
                geometryGroupLines.Children.Add(new LineGeometry(cl.StartPoint.Value.Add(positionCorrection), cl.EndPoint.Value.Add(positionCorrection)));

            geometryDrawingOrange.Geometry = geometryGroupLines;
            geometryDrawingRed.Geometry = new LineGeometry(pointPairGroup.Center.Add(positionCorrection), new Point(pointPairGroup.Center.X + coordAxesLength, pointPairGroup.Center.Y).Add(positionCorrection));
            geometryDrawingGreen.Geometry = new LineGeometry(pointPairGroup.Center.Add(positionCorrection), new Point(pointPairGroup.Center.X, pointPairGroup.Center.Y + coordAxesLength).Add(positionCorrection));
            geometryDrawingBlue.Geometry = new RectangleGeometry(new Rect(new Point(pointPairGroup.Rectangle.X + positionCorrection, pointPairGroup.Rectangle.Y + positionCorrection), new Size(pointPairGroup.Rectangle.Width, pointPairGroup.Rectangle.Height)));

            drawingGroup.Children.Add(geometryDrawingOrange);
            drawingGroup.Children.Add(geometryDrawingRed);
            drawingGroup.Children.Add(geometryDrawingGreen);
            drawingGroup.Children.Add(geometryDrawingBlue);

            DrawingImage drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();

            Display.Value = drawingImage;
        }
    }
}