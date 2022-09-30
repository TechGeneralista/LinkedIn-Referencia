using System;
using System.Globalization;
using System.Windows.Data;


namespace Common.Converter
{
    public class IsAssignableFromConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type parameterType = parameter as Type;

            if (parameterType == null)
                throw new Exception($"{nameof(IsAssignableFromConverter)} {nameof(parameter)} is not a Type");

            if (value == null)
                throw new Exception($"{nameof(IsAssignableFromConverter)} {nameof(value)} is null");

            Type valueType = value.GetType();

            return parameterType.IsAssignableFrom(valueType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
