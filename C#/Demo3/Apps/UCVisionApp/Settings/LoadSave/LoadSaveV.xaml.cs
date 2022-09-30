using Common;
using System.Windows;
using System.Windows.Controls;


namespace UCVisionApp.Settings.LoadSave
{
    /// <summary>
    /// Interaction logic for LoadSaveV.xaml
    /// </summary>
    public partial class LoadSaveV : UserControl
    {
        public LoadSaveV()
        {
            InitializeComponent();
        }

        private void OpenButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<LoadSaveDC>().OpenButtonClick();

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        => DataContext.CastTo<LoadSaveDC>().SaveButtonClick();
    }
}
