using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace Common.Converters
{
    public class NullableBoolToBrushOrangeRedGreenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? b = (bool?)value;

            if(b.IsNull())
                return Brushes.Orange;

            if (b == true)
                return Brushes.Green;

            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
