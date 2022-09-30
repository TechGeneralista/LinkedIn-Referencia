using System;
using System.Globalization;
using System.Windows.Data;


namespace Common.Converters
{
    public class DateFormat_YYYY_MM_DD_HH_MM_SS_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                DateTime dt = (DateTime)value;
                return dt.ToString("yyyy.MM.dd HH:mm:ss");
            }
            catch { }

            return DateTime.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
