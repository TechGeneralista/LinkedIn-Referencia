using System.Windows;
using System.Windows.Controls;


namespace ImageProcess.Modules
{
    /// <summary>
    /// Interaction logic for ModulesV.xaml
    /// </summary>
    public partial class ModulesV : UserControl
    {
        ModulesDC dc => (ModulesDC)DataContext;

        public ModulesV()
        {
            InitializeComponent();
        }

        private void AddNewModuleButtonClick(object sender, RoutedEventArgs e) => dc.AddNewModule();
        private void RemoveSelectedModuleButtonClick(object sender, RoutedEventArgs e) => dc.RemoveSelected();
    }
}
