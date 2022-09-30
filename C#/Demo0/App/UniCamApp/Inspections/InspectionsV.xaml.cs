using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace UniCamApp.Inspections
{
    /// <summary>
    /// Interaction logic for TaskListView.xaml
    /// </summary>
    public partial class InspectionsV : UserControl
    {
        public InspectionsDC dc => (InspectionsDC)DataContext;

        public InspectionsV() => InitializeComponent();

        private void AddNewButtonClick(object sender, RoutedEventArgs e) => dc.AddNewInspectionDC();
        private void RemoveSelectedButtonClick(object sender, RoutedEventArgs e) => dc.RemoveSelectedInspectionDC();
        private void RemoveAllButtonClick(object sender, RoutedEventArgs e) => dc.RemoveAllInspections();
        private void DoubleClick(object sender, MouseButtonEventArgs e) => dc.ShowComponentProperties();
    }
}
