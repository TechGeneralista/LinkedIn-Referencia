using Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace UCVisionApp.Inspection
{
    /// <summary>
    /// Interaction logic for InspectionV.xaml
    /// </summary>
    public partial class InspectionV : UserControl
    {
        public InspectionV()
        {
            InitializeComponent();
        }

        private void PanZoomImageViewMouseDown(Point downPos, MouseButtonEventArgs e)
            => DataContext.CastTo<InspectionDC>().PanZoomImageViewMouseDown(downPos, e);

        private void PanZoomImageViewMouseMove(Vector moveVec, Point movePos, MouseEventArgs e)
            => DataContext.CastTo<InspectionDC>().PanZoomImageViewMouseMove(moveVec, movePos, e);

        private void PanZoomImageViewMouseUp(Point upPos, MouseButtonEventArgs e)
            => DataContext.CastTo<InspectionDC>().PanZoomImageViewMouseUp(upPos, e);

        private void AddNewModuleButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<InspectionDC>().AddNewModuleButtonClick();

        private void RemoveSelectedModuleButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<InspectionDC>().RemoveSelectedModuleButtonClick();

        private void RemoveAllModulesButtonClick(object sender, RoutedEventArgs e)
            => DataContext.CastTo<InspectionDC>().RemoveAllModulesButtonClick();
    }
}
