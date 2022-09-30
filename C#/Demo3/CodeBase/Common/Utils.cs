using Common.Language;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

        static double _180PI = (double)180 / Math.PI;

        public static string GetGuid()
        {
            throw new NotImplementedException();
        }

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

        public static double Atan2(Point point) => RadToDeg(Math.Atan2(point.Y, point.X));
        public static double Sin(double angle) => Math.Sin(DegToRad(angle));
        public static double Cos(double angle) => Math.Cos(DegToRad(angle));
        public static Rect CreateRect(Point center, Size size) => new Rect(center.X - (size.Width / 2), center.Y - (size.Height / 2), size.Width, size.Height);

        public static string GetAppPath() => AppDomain.CurrentDomain.BaseDirectory;

        public static string GetPath(params string[] strings)
        {
            List<string> list = new List<string>();
            list.Add(GetAppPath());
            list.AddRange(strings);
            return Path.Combine(list.ToArray());
        }

        public static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    return ipAddress.ToString();
            }

            return null;
        }

        public static string[] GetLocalIPAddresses()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            List<string> ipAddresses = new List<string>();

            foreach (IPAddress ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    ipAddresses.Add(ipAddress.ToString());
            }

            return ipAddresses.ToArray();
        }

        public static void InvokeIfNecessary(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                action.Invoke();
            else
                Application.Current.Dispatcher.Invoke(action);
        }

        public static MessageBoxResult ShowAreYouSureYouWantToDeleteThemAllQuestion(LanguageDC languageDC)
            => MessageBox.Show(languageDC.AreYouSureYouWantToDeleteThemAll.Value, languageDC.QuestionColon.Value, MessageBoxButton.YesNo, MessageBoxImage.Question);

        public static int ConvertMinutesToMilliseconds(int minutes)
            => ConvertSecondsToMillisecunds(minutes * 60);

        private static int ConvertSecondsToMillisecunds(int seconds)
            => seconds * 1000;

        public static int ConvertMegaBytesToBytes(int size)
            => ConvertKiloBytesToBytes(size * 1024);

        public static int ConvertKiloBytesToBytes(int size)
            => size * 1024;

        public static string WMIInfo(string wmiClass, string wmiProperty)
        {
            ManagementClass mc = new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();
            string value = null;

            foreach (ManagementObject mo in moc)
            {
                try
                {
                    value = mo[wmiProperty].ToString();
                }
                catch { }

                if (!string.IsNullOrEmpty(value))
                    break;
            }

            return value;
        }

        public static string ConcatWithSeparator(char separator, params string[] array)
            => ConcatWithSeparator(separator.ToString(), array);

        public static string ConcatWithSeparator(string separator, params string[] array)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string lastElement = array.Last();

            foreach(string s in array)
            {
                stringBuilder.Append(s);

                if (s != lastElement)
                    stringBuilder.Append(separator);
            }

            return stringBuilder.ToString();
        }

        public static void CreateShortcutOnDesktop(string exeFilePath, string shortcutName)
        {
            WshShell wsh = new WshShell();
            IWshShortcut shortcut = wsh.CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), string.Format("{0}.lnk", shortcutName))) as IWshShortcut;
            shortcut.TargetPath = exeFilePath;
            shortcut.Save();
        }

        public static int ConvertSecondsToMilliSeconds(int seconds)
            => seconds * 1000;

        public static bool ValueInRange(double value, double min, double max)
            => (value >= min) && (value <= max);

        public static void OpenContextMenu(object o, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = o as FrameworkElement;

            if (frameworkElement.IsNotNull())
            {
                ContextMenu contextMenu = frameworkElement.ContextMenu;
                contextMenu.PlacementTarget = frameworkElement;
                contextMenu.IsOpen = true;
                e.Handled = true;
            }
        }
    }
}
