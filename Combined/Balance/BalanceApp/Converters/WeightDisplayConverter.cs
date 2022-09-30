using System;
using System.Globalization;
using System.Windows.Data;


namespace BalanceApp.Converters
{
    public class WeightDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int measuredWeightInt = (int)((double)value / (double)0.5);
            return (measuredWeightInt * 0.5).ToString("0.0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
