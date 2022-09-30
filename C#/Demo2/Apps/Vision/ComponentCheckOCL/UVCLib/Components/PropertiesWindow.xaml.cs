using System.Windows;



namespace UVCLib.Components
{
    /// <summary>
    /// Interaction logic for VideoSourcePropertyWindow.xaml
    /// </summary>
    public partial class PropertiesWindow : Window
    {
        public PropertiesWindow()
        {
            InitializeComponent();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SetDefaultValuesButtonClick(object sender, RoutedEventArgs e)
        {
            PropertiesModel vsp = (PropertiesModel)DataContext;
            vsp.SetDefaultValues();
        }
    }
}
