using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace IOBoard.View.Converter
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IOBoardClientStates status = (IOBoardClientStates)value;

            switch(status)
            {
                case IOBoardClientStates.Connecting:
                    return new SolidColorBrush(Colors.Red);
                case IOBoardClientStates.Connected:
                    return new SolidColorBrush(Colors.Green);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
