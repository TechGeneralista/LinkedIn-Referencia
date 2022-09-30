using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ViProEditorApp.Domain.Inspector;

namespace ViProEditorApp.UI.Project.Converter
{
    internal class ProjectNameInspectorStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (ProjectNameInspectorState)value == ProjectNameInspectorState.Ok ? Visibility.Hidden : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
