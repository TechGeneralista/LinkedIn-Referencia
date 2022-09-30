using Common;
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

namespace UCVisionResultExplorerApp
{
    /// <summary>
    /// Interaction logic for ResultDataV.xaml
    /// </summary>
    public partial class ResultDataV : UserControl
    {
        public ResultDataV()
        {
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
            => sender.CastTo<Image>()?.DataContext.CastTo<ResultDataImageDC>()?.OpenImage();
    }
}
