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

        public static unsafe void SetPixel4(this WriteableBitmap wbm, int bytePerPixel, int stride, int x, int y, Color c)
        {
            if (x < 0 || x > (wbm.PixelWidth-1) || y < 0 || y > (wbm.PixelHeight-1))
                return;

            byte* scan0 = (byte*)wbm.BackBuffer.ToPointer();
            byte* pixelPointer = scan0 + (x * bytePerPixel) + (y * stride);
            *(pixelPointer) = c.B;
            *(pixelPointer + 1) = c.G;
            *(pixelPointer + 2) = c.R;
            *(pixelPointer + 3) = c.A;
        }

        public static unsafe void SetPixel4(this WriteableBitmap wbm, byte* scan0, int bytePerPixel, int stride, int width1, int height1, int x, int y, Color c)
        {
            if (x < 0 || x > width1 || y < 0 || y > height1)
                return;

            byte* pixelPointer = scan0 + (x * bytePerPixel) + (y * stride);
            *(pixelPointer) = c.B;
            *(pixelPointer + 1) = c.G;
            *(pixelPointer + 2) = c.R;
            *(pixelPointer + 3) = c.A;
        }

        public static unsafe Color GetPixel4(this WriteableBitmap wbm, int bytePerPixel, int stride, int x, int y)
        {
            if (x < 0 || x > (wbm.PixelWidth-1) || y < 0 || y > (wbm.PixelHeight-1))
                return Colors.Black;

            byte* scan0 = (byte*)wbm.BackBuffer.ToPointer();
            byte* pixelPointer = scan0 + (x * bytePerPixel) + (y * stride);
            return Color.FromArgb(*(pixelPointer + 3), *(pixelPointer + 2), *(pixelPointer + 1), *(pixelPointer));
        }

        public static unsafe Color GetPixel4(this WriteableBitmap wbm, byte* scan0, int bytePerPixel, int stride, int width1, int height1, int x, int y)
        {
            if (x < 0 || x > width1 || y < 0 || y > height1)
                return Colors.Black;

            byte* pixelPointer = scan0 + (x * bytePerPixel) + (y * stride);
            return Color.FromArgb(*(pixelPointer + 3), *(pixelPointer + 2), *(pixelPointer + 1), *(pixelPointer));
        }

        public static byte[] CopyPixelData(this BitmapSource bitmapSource)
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
