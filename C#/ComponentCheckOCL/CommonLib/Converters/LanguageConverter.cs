using CommonLib.Interfaces;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;



namespace CommonLib.Converters
{
    public class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)GetConvertableParameter(value, parameter);
        }

        private object GetConvertableParameter(object value, object parameter)
        {
            if (parameter != null)
                return parameter;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
