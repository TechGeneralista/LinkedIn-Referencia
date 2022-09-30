using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace UserProgram.Converter
{
    public class InterpreterSatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            InterpreterStates status = (InterpreterStates)value;

            if (status == InterpreterStates.Running)
                return Brushes.Green;

            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
