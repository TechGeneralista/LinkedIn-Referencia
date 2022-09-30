using System;
using System.Windows;
using System.Windows.Media;


namespace Common
{
    public static class Extensions
    {
        public static bool IsNull(this object o) => o == null;
        public static bool IsNotNull(this object o) => o != null;

        #region Color space converters (RGB <-> HSV)
        public static HSVColor GetHSV(this Color color)
        {
            double r = color.R;
            double g = color.G;
            double b = color.B;

            r /= 255.0;
            g /= 255.0;
            b /= 255.0;

            double cmax = Math.Max(r, Math.Max(g, b));
            double cmin = Math.Min(r, Math.Min(g, b));
            double diff = cmax - cmin;
            double h = -1;

            if (diff == 0)
                h = 0;

            else if (cmax == r)
                h = (60 * ((g - b) / diff) + 360) % 360;

            else if (cmax == g)
                h = (60 * ((b - r) / diff) + 120) % 360;

            else if (cmax == b)
                h = (60 * ((r - g) / diff) + 240) % 360;
            double s;
            if (cmax == 0)
                s = 0;
            else
                s = (diff / cmax);

            return new HSVColor(h, s, cmax);
        }

        public static Color GetColor(this HSVColor hsvColor)
        {
            double hue = hsvColor.Hue;
            double r, g, b;

            if (hsvColor.Saturation == 0)
            {
                r = hsvColor.Value;
                g = hsvColor.Value;
                b = hsvColor.Value;
            }
            else
            {
                if (hue == 360)
                    hue = 0;
                else
                    hue /= 60;

                int i = (int)hue;
                double f = hue - i;

                double p = hsvColor.Value * (1.0 - hsvColor.Saturation);
                double q = hsvColor.Value * (1.0 - (hsvColor.Saturation * f));
                double t = hsvColor.Value * (1.0 - (hsvColor.Saturation * (1.0 - f)));

                switch (i)
                {
                    case 0:
                        r = hsvColor.Value;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = hsvColor.Value;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = hsvColor.Value;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = hsvColor.Value;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = hsvColor.Value;
                        break;

                    default:
                        r = hsvColor.Value;
                        g = p;
                        b = q;
                        break;
                }

            }

            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        #endregion

        #region Point

        public static Point Add(this Point p0, double value) => new Point(p0.X + value, p0.Y + value);
        public static Point Add(this Point p0, Point p1) => new Point(p0.X + p1.X, p0.Y + p1.Y);
        public static Point Subtract(this Point p0, Point p1) => new Point(p0.X - p1.X, p0.Y - p1.Y);

        #endregion

        #region Size

        public static Size Subtract(this Size s0, Size s1) => new Size(s0.Width - s1.Width, s0.Height - s1.Height);

        #endregion
    }
}
