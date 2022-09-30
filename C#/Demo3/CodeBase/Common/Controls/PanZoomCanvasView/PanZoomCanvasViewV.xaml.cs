using Common.Controls.PanZoomCanvasView.ModuleContainer;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Common.Controls.PanZoomCanvasView
{
    /// <summary>
    /// Interaction logic for PanZoomCanvasViewV.xaml
    /// </summary>
    public partial class PanZoomCanvasViewV : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(PanZoomCanvasViewV));
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(PanZoomCanvasViewV));
        public static readonly DependencyProperty CanvasWidthProperty = DependencyProperty.Register(nameof(CanvasWidth), typeof(double), typeof(PanZoomCanvasViewV), new PropertyMetadata(double.NaN));
        public static readonly DependencyProperty CanvasHeightProperty = DependencyProperty.Register(nameof(CanvasHeight), typeof(double), typeof(PanZoomCanvasViewV), new PropertyMetadata(double.NaN));
        public static new readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(PanZoomCanvasViewV), new PropertyMetadata(Brushes.DarkBlue));
        public static new readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(PanZoomCanvasViewV), new PropertyMetadata(Brushes.Gray));


        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public double CanvasWidth
        {
            get { return (double)GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }

        public double CanvasHeight
        {
            get { return (double)GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }

        public new Brush Foreground
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public new Brush Background
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }


        public PanZoomCanvasViewV()
            => InitializeComponent();

        private void WorkSheet_MouseDown(object sender, MouseButtonEventArgs e)
            => ModuleContainerDC.GetInstance(e.MouseDevice.DirectlyOver)?.MouseDown(e);

        private void WorkSheet_MouseMove(object sender, MouseEventArgs e)
            => ModuleContainerDC.GetInstance(e.MouseDevice.DirectlyOver)?.MouseMove(e);

        private void WorkSheet_MouseUp(object sender, MouseButtonEventArgs e)
            => ModuleContainerDC.GetInstance(e.MouseDevice.DirectlyOver)?.MouseUp(e);

        private void WorkSheet_Loaded(object sender, RoutedEventArgs e)
            => ((DependencyObject)sender).FindParent<ZoomPanBorder>()?.RegisterPanel((FrameworkElement)sender);
    }
}
