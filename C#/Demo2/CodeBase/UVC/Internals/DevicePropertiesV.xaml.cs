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

namespace UVC.Internals
{
    /// <summary>
    /// Interaction logic for DevicePropertiesView.xaml
    /// </summary>
    public partial class DevicePropertiesV : UserControl
    {
        public DevicePropertiesV()
        {
            InitializeComponent();
        }

        private void SetAllToDefaultButtonClick(object sender, RoutedEventArgs e) => ((DevicePropertiesDC)DataContext).ResetAllToDefault();
    }
}
