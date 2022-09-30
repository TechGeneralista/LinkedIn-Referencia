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

namespace uVisionV2.Modules.TriggerButton
{
    /// <summary>
    /// Interaction logic for TriggerButtonV.xaml
    /// </summary>
    public partial class TriggerButtonV : UserControl
    {
        public TriggerButtonV()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
            => ((TriggerButtonDC)DataContext).Button_Click();
    }
}
