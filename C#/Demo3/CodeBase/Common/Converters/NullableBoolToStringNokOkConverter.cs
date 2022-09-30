using System;
using System.Globalization;
using System.Windows.Data;


namespace Common.Converters
{
    public class NullableBoolToStringNokOkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? b = (bool?)value;

            if (b.IsNull())
                return "N/A";

            if (b == false)
                return "NOK";

            return "OK";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
