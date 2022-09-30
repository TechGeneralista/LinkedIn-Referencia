using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Common
{
    public static class Utils
    {
        public static string NewGuid(int charCount) => Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, charCount).ToUpper();

        public static WriteableBitmap LoadImageFromFile(string path, bool isFrozen)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (var stream = new FileStream(path, FileMode.Open))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapImage);

            if (isFrozen)
                writeableBitmap.Freeze();

            return writeableBitmap;
        }

        public static WriteableBitmap GetBlackImage()
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap(4, 3, 96, 96, PixelFormats.Bgra32, null);
            writeableBitmap.Freeze();
            return writeableBitmap;
        }

        public static T[] Add<T>(this T[] array, T item)
        {
            List<T> temp = new List<T>();
            temp.AddRange(array);
            temp.Add(item);
            return temp.ToArray();
        }

        public static T[] Remove<T>(this T[] array, T item)
        {
            List<T> temp = new List<T>();
            temp.AddRange(array);
            temp.Remove(item);
            return temp.ToArray();
        }

        public static T[] Clear<T>(this T[] array) => new T[0];

        static double _180PI = (double)180 / Math.PI;
        public static double RadToDeg(double rad)
        {
            double deg = rad * _180PI;

            if (deg < 0)
                deg += (double)360;

            return deg;
        }

        static double PI180 = Math.PI / (double)180;
        public static double DegToRad(double deg)
        {
            if (deg > 180)
                deg -= 360;

            return deg * PI180;
        }

        public static Point RotatePoint(Point point, double angle) => RotatePoint(new Point(0,0), point, angle);

        public static Point RotatePoint(Point origin, Point point, double angle)
        {
            //double rad = DegToRad(angle);
            //double s = Math.Sin(rad);
            //double c = Math.Cos(rad);
            //Point offsettedPoint = point.Subtract(origin);
            //Point rotatedPoint = new Point(offsettedPoint.X * c - offsettedPoint.Y * s, offsettedPoint.X * s + offsettedPoint.Y * c);
            //return rotatedPoint.Add(origin);

            double s = Sin(angle);
            double c = Cos(angle);
            Point offsettedPoint = point.Subtract(origin);
            Point rotatedPoint = new Point(offsettedPoint.X * c - offsettedPoint.Y * s, offsettedPoint.X * s + offsettedPoint.Y * c);
            return rotatedPoint.Add(origin);
        }

        public static double Atan2(Point point) => RadToDeg(Math.Atan2(point.Y, point.X));
        public static double Sin(double angle) => Math.Sin(DegToRad(angle));
        public static double Cos(double angle) => Math.Cos(DegToRad(angle));

        public static double GetPointsDistance(Point p0, Point p1) => Math.Sqrt(Math.Pow(p1.X - p0.X,2) + Math.Pow(p1.Y - p0.Y, 2));

        public static Rect CreateRect(Point center, Size size) => new Rect(center.X - (size.Width / 2), center.Y - (size.Height / 2), size.Width, size.Height);
    }
}
