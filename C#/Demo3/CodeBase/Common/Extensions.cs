using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Common
{
    public static class Extensions
    {
        public static bool IsNull(this object o) => o == null;
        public static bool IsNotNull(this object o) => o != null;

        #region Explicit casting

        public static T CastTo<T>(this object o) where T : class
            => o as T;

        #endregion

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

        public static T[] Add<T>(this T[] array, T item)
        {
            List<T> temp = new List<T>();
            temp.AddRange(array);
            temp.Add(item);
            return temp.ToArray();
        }

        public static T[] AddRange<T>(this T[] array, T[] items)
        {
            List<T> temp = new List<T>();
            temp.AddRange(array);
            temp.AddRange(items);
            return temp.ToArray();
        }

        public static T[] Remove<T>(this T[] array, T item)
        {
            List<T> temp = new List<T>();
            temp.AddRange(array);
            temp.Remove(item);
            return temp.ToArray();
        }

        public static T[] RemoveFirst<T>(this T[] array)
        {
            List<T> temp = new List<T>();
            temp.AddRange(array);
            temp.Remove(temp.First());
            return temp.ToArray();
        }

        public static T[] Clear<T>(this T[] array) => new T[0];

        public static bool Compare(this string[] array, params string[] reference)
        {
            if (reference.Length > array.Length)
                return false;

            for (int i = 0; i < reference.Length; i++)
            {
                if (array[i] != reference[i])
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

        public static byte[] Serialize2(this object obj)
        {
            int size = Marshal.SizeOf(obj);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            byte[] bytes = new byte[size];
            Marshal.StructureToPtr(obj, ptr, false);
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        public static T Deserialize2<T>(this byte[] data)
        {
            IntPtr ptr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);
            T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return obj;
        }

        #endregion

        #region byte Array

        public static bool StartWith(this byte[] array, byte[] pattern)
        {
            if (array.Length < pattern.Length)
                return false;

            return array.Take(pattern.Length).SequenceEqual(pattern);
        }

        public static bool EndWith(this byte[] array, int length, byte[] pattern)
        {
            if (length < pattern.Length)
                return false;

            return array.Skip(length - pattern.Length).Take(pattern.Length).SequenceEqual(pattern);
        }

        public static byte[] ToUTF8Bytes(this string str)
            => Encoding.UTF8.GetBytes(str);

        public static string ToUTF8String(this byte[] array)
            => Encoding.UTF8.GetString(array);

        #endregion

        #region string

        public static bool CheckLicenseKeyFormat(this string key)
        {
            if (key.Length != 32)
                return false;

            foreach(char c in key)
            {
                if (!char.IsNumber(c) && c != 'A' && c != 'B' && c != 'C' && c != 'D' && c != 'E' && c != 'F')
                    return false;
            }

            return true;
        }

        #endregion

        public static BitmapSource ToBitmapSource(this ImageSource source)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(source, new Rect(new Point(0, 0), new Size(source.Width, source.Height)));
            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)source.Width, (int)source.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            bmp.Freeze();

            return bmp;
        }

        public static DrawingImage ToDrawingImage(this BitmapSource source)
            => new DrawingImage(new ImageDrawing(source, new Rect(0, 0, source.PixelWidth, source.PixelHeight)));

        public static Screen GetScreen(this Window window)
            => Screen.FromHandle(new WindowInteropHelper(window).Handle);

        public static T FindParent<T>(this DependencyObject child, string parentName = null) where T : DependencyObject
        {
            if (child == null) return null;

            T foundParent = null;
            DependencyObject currentParent = VisualTreeHelper.GetParent(child);

            do
            {
                FrameworkElement frameworkElement = currentParent as FrameworkElement;

                if ((parentName.IsNull() || (parentName.IsNotNull() && frameworkElement.Name == parentName)) && frameworkElement is T)
                {
                    foundParent = (T)currentParent;
                    break;
                }

                currentParent = VisualTreeHelper.GetParent(currentParent);

            } while (currentParent.IsNotNull());

            return foundParent;
        }

        public static bool RectOverlap(this Rect thisRect, Rect anotherRect)
        {
            Point trc0 = new Point(thisRect.X, thisRect.Y);
            Point trc1 = new Point(thisRect.X + thisRect.Width, thisRect.Y);
            Point trc2 = new Point(thisRect.X + thisRect.Width, thisRect.Y + thisRect.Height);
            Point trc3 = new Point(thisRect.X, thisRect.Y + thisRect.Height);

            bool corner0Contains = Utils.ValueInRange(trc0.X, anotherRect.X, anotherRect.X + anotherRect.Width) && Utils.ValueInRange(trc0.Y, anotherRect.Y, anotherRect.Y + anotherRect.Height);
            bool corner1Contains = Utils.ValueInRange(trc1.X, anotherRect.X, anotherRect.X + anotherRect.Width) && Utils.ValueInRange(trc1.Y, anotherRect.Y, anotherRect.Y + anotherRect.Height);
            bool corner2Contains = Utils.ValueInRange(trc2.X, anotherRect.X, anotherRect.X + anotherRect.Width) && Utils.ValueInRange(trc2.Y, anotherRect.Y, anotherRect.Y + anotherRect.Height);
            bool corner3Contains = Utils.ValueInRange(trc3.X, anotherRect.X, anotherRect.X + anotherRect.Width) && Utils.ValueInRange(trc3.Y, anotherRect.Y, anotherRect.Y + anotherRect.Height);

            return corner0Contains || corner1Contains || corner2Contains || corner3Contains;
        }
    }
}
