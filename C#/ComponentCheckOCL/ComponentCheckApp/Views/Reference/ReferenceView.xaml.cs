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

namespace ComponentCheckApp.Views.Reference
{
    /// <summary>
    /// Interaction logic for ShapeView.xaml
    /// </summary>
    public partial class ReferenceView : UserControl
    {
        ReferenceViewModel vm => (ReferenceViewModel)DataContext;

        public ReferenceView()
        {
            InitializeComponent();
        }

        private void MouseDownImage(object sender, MouseButtonEventArgs e)
        {
            vm.MouseDown(e);
        }

        private void SliderDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            vm.SliderDragCompleted();
        }
    }
}
