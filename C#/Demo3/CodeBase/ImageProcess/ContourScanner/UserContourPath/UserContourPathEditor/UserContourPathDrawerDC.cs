using Common;
using Common.NotifyProperty;
using ImageProcess.ReferenceImages;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageProcess.ContourScanner.UserContourPath.UserContourPathEditor
{
    public class UserContourPathDrawerDC
    {
        public IReadOnlyProperty<ImageSource> ResultImage { get; } = new Property<ImageSource>();


        readonly double positionCorrection = .5;
        readonly double ellipseSize = .25;
        readonly Vector positionCorrectionVector;
        readonly ReferenceImagesDC referenceImagesDC;
        readonly UserContourPathEditorDC userContourPathDC;


        public UserContourPathDrawerDC(ReferenceImagesDC referenceImagesDC, UserContourPathEditorDC userContourPathDC)
        {
            this.referenceImagesDC = referenceImagesDC;
            this.userContourPathDC = userContourPathDC;

            positionCorrectionVector = new Vector(positionCorrection, positionCorrection);
        }

        internal void Draw()
        {
            BitmapSource bitmap = referenceImagesDC.MonochromeImage.Value;
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
            if (userContourPathDC.SelectedUserLine.Value.IsNotNull())
                geometryGroupSelectedLine.Children.Add(new LineGeometry(Point.Add(userContourPathDC.SelectedUserLine.Value.StartPoint.Value, positionCorrectionVector), Point.Add(userContourPathDC.SelectedUserLine.Value.EndPoint.Value, positionCorrectionVector)));

            //Draw other lines
            foreach (UserLineDC userLineDC in userContourPathDC.UserLines.Value)
            {
                if (userContourPathDC.SelectedUserLine.Value.IsNotNull() && userContourPathDC.SelectedUserLine.Value == userLineDC)
                    continue;

                geometryGroupLines.Children.Add(new LineGeometry(Point.Add(userLineDC.StartPoint.Value, positionCorrectionVector), Point.Add(userLineDC.EndPoint.Value, positionCorrectionVector)));
            }

            //Draw lines start/end points
            foreach (UserLineDC userLineDC in userContourPathDC.UserLines.Value)
            {
                geometryGroupLinePoints.Children.Add(new EllipseGeometry(Point.Add(userLineDC.StartPoint.Value, positionCorrectionVector), ellipseSize, ellipseSize));
                geometryGroupLinePoints.Children.Add(new EllipseGeometry(Point.Add(userLineDC.EndPoint.Value, positionCorrectionVector), ellipseSize, ellipseSize));
            }

            //Draw sample points
            foreach (UserLineDC userLineDC in userContourPathDC.UserLines.Value)
            {
                foreach (SamplingPointPair pointPair in userLineDC.SamplingPointPairs.Value)
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

            ResultImage.ToSettable().Value = drawingImage;
        }
    }
}
