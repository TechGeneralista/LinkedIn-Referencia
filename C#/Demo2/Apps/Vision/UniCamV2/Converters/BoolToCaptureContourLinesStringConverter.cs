using System;
using System.Globalization;
using System.Windows.Data;


namespace UniCamV2.Converters
{
    public class BoolToCaptureContourLinesStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(bool)value)
                return "Felvétel";

            return "Kész";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
