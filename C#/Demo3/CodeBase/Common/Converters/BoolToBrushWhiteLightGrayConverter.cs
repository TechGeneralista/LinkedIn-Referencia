using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace Common.Converters
{
    public class BoolToBrushWhiteLightGrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Brushes.LightGray;

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
