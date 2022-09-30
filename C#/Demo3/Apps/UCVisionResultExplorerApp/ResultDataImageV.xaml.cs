using Common;
using System.Windows.Controls;
using System.Windows.Input;


namespace UCVisionResultExplorerApp
{
    /// <summary>
    /// Interaction logic for ResultDataImageV.xaml
    /// </summary>
    public partial class ResultDataImageV : UserControl
    {
        public ResultDataImageV()
        {
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
            => DataContext.CastTo<ResultDataImageDC>().OpenImage();
    }
}
