using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Common.SaveResult
{
    /// <summary>
    /// Interaction logic for SaveResultV.xaml
    /// </summary>
    public partial class SaveResultV : UserControl
    {
        public SaveResultV()
        {
            InitializeComponent();
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<SaveResultDC>().Browse();

        private void OpenFolderButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<SaveResultDC>().OpenFolder();
    }
}
