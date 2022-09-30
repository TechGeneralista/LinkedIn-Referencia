using System;
using System.Globalization;
using System.Windows.Data;


namespace Common.Converters
{
    public class BoolToStringNokOkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "OK";

            return "NOK";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
