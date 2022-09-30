using CommonLib.Components;
using ImageProcessLib.OpenCL;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ComponentCheckApp.Views.Shape
{
    public class ShapeViewModel : ObservableProperty
    {
        public WriteableBitmap ShapeContour { get; private set; }
        public int Angle { get; private set; }

        public string BorderColor
        {
            get => borderColor;
            private set => SetField(value, ref borderColor);
        }

        public double Match
        {
            get => match;
            private set => SetField(value, ref match);
        }

        public Point Position { get; private set; }


        string borderColor;
        double match;
        OCLShapePixelCounter oclShapePixelCounter;
        OCLShapeFinder oclShapeFinder;


        public ShapeViewModel(WriteableBitmap shapeContour, int angle)
        {
            OpenCLDevice openCLDevice = (OpenCLDevice)Application.Current.Resources[ResourceKeys.OpenCLDeviceKey];
            oclShapePixelCounter = new OCLShapePixelCounter(openCLDevice);
            oclShapeFinder = new OCLShapeFinder(openCLDevice);

            ShapeContour = shapeContour;
            Angle = angle;
            borderColor = Colors.Orange.ToString();
            match = 0;

            oclShapePixelCounter.Start(shapeContour);
        }

        public void Find(WriteableBitmap input, int minimumMatch)
        {
            oclShapeFinder.Start(ShapeContour, input);
            Match = Math.Round(((double)oclShapeFinder.MatchedPixelsCount / (double)oclShapePixelCounter.PixelsCount) * 100d,1);
            Position = oclShapeFinder.Position;

            if (match >= minimumMatch)
                BorderColor = Colors.Green.ToString();
            else
                BorderColor = Colors.Red.ToString();
        }

        internal void Reset()
        {
            Match = 0;
            BorderColor = Colors.Orange.ToString();
        }
    }
}
