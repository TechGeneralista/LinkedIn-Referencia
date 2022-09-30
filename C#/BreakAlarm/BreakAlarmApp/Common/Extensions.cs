using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Common
{
    public static class Extensions
    {
        public static bool IsNull(this object o) => o == null;
        public static bool IsNotNull(this object o) => o != null;

        #region WriteableBitmap
        public static byte[] CopyData(this BitmapSource bitmapSource)
        {
            int stride = bitmapSource.PixelWidth * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            byte[] bitmapData = new byte[bitmapSource.PixelHeight * stride];
            bitmapSource.CopyPixels(bitmapData, stride, 0);
            return bitmapData;
        }

        #endregion

        #region Point

        public static Point RotatePoint(this Point point, double angle) => RotatePoint(point, new Point(0, 0), angle);

        public static Point RotatePoint(this Point point, Point origin, double angle)
        {
            double rad = Utils.DegToRad(angle);
            double s = Math.Sin(rad);
            double c = Math.Cos(rad);
            Point offsettedPoint = (Point)Point.Subtract(point, origin);
            Point rotatedPoint = new Point(offsettedPoint.X * c - offsettedPoint.Y * s, offsettedPoint.X * s + offsettedPoint.Y * c);
            return Point.Add(rotatedPoint, (Vector)origin);
        }

        public static double Distance(this Point p0, Point p1) => Math.Sqrt(Math.Pow(p1.X - p0.X, 2) + Math.Pow(p1.Y - p0.Y, 2));
        public static Point Average(this Point p0, Point p1) => new Point((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2);

        #endregion

        #region Array

        public static bool Compare(this string[] array, params string[] ps)
        {
            if(ps.Length > array.Length)
                return false;

            for (int i=0;i<ps.Length;i++)
            {
                if (array[i] != ps[i])
                    return false;
            }

            return true;
        }

        #endregion

        #region Serialization

        public static byte[] Serialize(this object obj)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        public static T Deserialize<T>(this byte[] data)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new MemoryStream(data))
                return (T)binaryFormatter.Deserialize(memoryStream);
        }

        #endregion
    }
}
