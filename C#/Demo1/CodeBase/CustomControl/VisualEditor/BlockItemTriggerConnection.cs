using Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace CustomControl.VisualEditor
{
    public class BlockItemTriggerConnection : Control, IHasQuadraticBezierCurvePoints, ISelectable
    {
        static BlockItemTriggerConnection()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockItemTriggerConnection), new FrameworkPropertyMetadata(typeof(BlockItemTriggerConnection)));
        }

        public Point Start
        {
            get { return (Point)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }
        public static readonly DependencyProperty StartProperty = DependencyProperty.Register(nameof(Start), typeof(Point), typeof(BlockItemTriggerConnection), new PropertyMetadata(new Point(0, 0), StartPointChanged));

        private static void StartPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItemTriggerConnection BlockItemTriggerConnection)
                BlockItemTriggerConnection.UpdateStartEndDirectionPoints();
        }

        public Point StartDirection
        {
            get { return (Point)GetValue(StartDirectionProperty); }
            set { SetValue(StartDirectionProperty, value); }
        }
        public static readonly DependencyProperty StartDirectionProperty = DependencyProperty.Register(nameof(StartDirection), typeof(Point), typeof(BlockItemTriggerConnection), new PropertyMetadata(new Point(100, 0)));

        public Point EndDirection
        {
            get { return (Point)GetValue(EndDirectionProperty); }
            set { SetValue(EndDirectionProperty, value); }
        }
        public static readonly DependencyProperty EndDirectionProperty = DependencyProperty.Register(nameof(EndDirection), typeof(Point), typeof(BlockItemTriggerConnection), new PropertyMetadata(new Point(0, 100)));

        public Point End
        {
            get { return (Point)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }
        public static readonly DependencyProperty EndProperty = DependencyProperty.Register(nameof(End), typeof(Point), typeof(BlockItemTriggerConnection), new PropertyMetadata(new Point(100, 100), EndPointChanged));

        private static void EndPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItemTriggerConnection BlockItemTriggerConnection)
                BlockItemTriggerConnection.UpdateStartEndDirectionPoints();
        }

        public double BackgroundPathStrokeThickness
        {
            get { return (double)GetValue(BackgroundPathStrokeThicknessProperty); }
            set { SetValue(BackgroundPathStrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty BackgroundPathStrokeThicknessProperty = DependencyProperty.Register(nameof(BackgroundPathStrokeThickness), typeof(double), typeof(BlockItemTriggerConnection), new PropertyMetadata(6d));

        public double PathStrokeThickness
        {
            get { return (double)GetValue(PathStrokeThicknessProperty); }
            set { SetValue(PathStrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty PathStrokeThicknessProperty = DependencyProperty.Register(nameof(PathStrokeThickness), typeof(double), typeof(BlockItemTriggerConnection), new PropertyMetadata(3d));

        public object OutputDataContext
        {
            get { return GetValue(OutputDataContextProperty); }
            set { SetValue(OutputDataContextProperty, value); }
        }
        public static readonly DependencyProperty OutputDataContextProperty = DependencyProperty.Register(nameof(OutputDataContext), typeof(object), typeof(BlockItemTriggerConnection));

        public object InputDataContext
        {
            get { return GetValue(InputDataContextProperty); }
            set { SetValue(InputDataContextProperty, value); }
        }
        public static readonly DependencyProperty InputDataContextProperty = DependencyProperty.Register(nameof(InputDataContext), typeof(object), typeof(BlockItemTriggerConnection));

        public bool Active
        {
            get { return (bool)GetValue(ActiveProperty); }
            set { SetValue(ActiveProperty, value); }
        }
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.Register(nameof(Active), typeof(bool), typeof(BlockItemTriggerConnection), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ActiveChanged));

        private static void ActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool b && b &&
               d is BlockItemTriggerConnection blockItemTriggerConnection)
            {
                blockItemTriggerConnection.Active = false;
                blockItemTriggerConnection.Avtivated();
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(BlockItemTriggerConnection), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) => ((BlockItemTriggerConnection)s).IsSelectedChanged()));


        readonly int animationDuration = 200;
        readonly int signalAnimationDuration = 1000;
        Path backgroundPath;
        SolidColorBrush backgroundPathStroke;
        ColorAnimation mouseEnterColorAnimation, mouseLeaveColorAnimation;
        Canvas signalCanvas;
        List<SignalSymbol> activeSignals;
        SolidColorBrush pathStroke;
        DateTime lastSignalFlowStarted = DateTime.Now;
        Grid grid;
        BlocksEditor cachedBlocksEditor;


        public BlockItemTriggerConnection()
        {
            activeSignals = new List<SignalSymbol>();
            Loaded += BlockItemTriggerConnection_Loaded;
        }

        private void BlockItemTriggerConnection_Loaded(object sender, RoutedEventArgs e)
        {
            cachedBlocksEditor = this.GetVisualParent<BlocksEditor>();
            cachedBlocksEditor.GetBlockItemTriggerOutput(this).UpdatePinSymbolCenterAndConnectionEnd();
            cachedBlocksEditor.GetBlockItemTriggerInput(this).UpdatePinSymbolCenterAndConnectionEnd();
        }

        public override void OnApplyTemplate()
        {
            MenuItem removeMenuItem = GetTemplateChild("PART_RemoveMenuItem") as MenuItem;
            removeMenuItem.Click += (s, e) => cachedBlocksEditor.RemoveChild(this);

            grid = GetTemplateChild("PART_Grid") as Grid;

            if(grid != null)
            {
                grid.MouseDown += Grid_MouseDown;
                grid.MouseUp += Grid_MouseUp;
            }

            backgroundPath = GetTemplateChild("PART_BackgroundPath") as Path;
            Path path = GetTemplateChild("PART_Path") as Path;

            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/CustomControl;component/Themes/VisualEditor/ColorStyle.xaml", UriKind.Absolute)
            };

            backgroundPathStroke = new SolidColorBrush((Color)resourceDictionary["Static_Color"]);
            backgroundPath.Stroke = backgroundPathStroke;

            path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Trigger_Static_Color"]); ;

            mouseEnterColorAnimation = new ColorAnimation
                (
                    (Color)resourceDictionary["MouseEnter_Color"],
                    TimeSpan.FromMilliseconds(animationDuration)
                );

            mouseLeaveColorAnimation = new ColorAnimation
                (
                    (Color)resourceDictionary["Static_Color"],
                    TimeSpan.FromMilliseconds(animationDuration)
                );
        }

        private void UpdateStartEndDirectionPoints()
        {
            double xAbs = Math.Abs(Start.X - End.X) / 2;
            StartDirection = new Point(Start.X + xAbs, Start.Y);
            EndDirection = new Point(End.X - xAbs, End.Y);
        }

        private void Avtivated()
        {
            if ((DateTime.Now - lastSignalFlowStarted) < TimeSpan.FromMilliseconds(250))
                return;

            if (signalCanvas == null && this.GetVisualParent<BlocksEditor>().SignalCanvas is Canvas canvas)
                signalCanvas = canvas;

            SignalSymbol signalSymbol = new SignalSymbol(signalAnimationDuration, pathStroke, PathStrokeThickness * 3, true, this);
            signalSymbol.Finished += SignalSymbol_Finished;
            signalCanvas.Children.Add(signalSymbol);
            activeSignals.Add(signalSymbol);
            signalSymbol.BeginAnimation();
            lastSignalFlowStarted = DateTime.Now;
        }

        private void SignalSymbol_Finished(object sender, EventArgs e)
        {
            SignalSymbol signalSymbol = (SignalSymbol)sender;
            signalSymbol.Finished -= SignalSymbol_Finished;
            signalCanvas.Children.Remove(signalSymbol);
            activeSignals.Remove(signalSymbol);
        }

        private void IsSelectedChanged()
        {
            if (IsSelected)
                backgroundPathStroke.BeginAnimation(SolidColorBrush.ColorProperty, mouseEnterColorAnimation);
            else
                backgroundPathStroke.BeginAnimation(SolidColorBrush.ColorProperty, mouseLeaveColorAnimation);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == cachedBlocksEditor.SelectAndMoveMouseButton &&
                e.ButtonState == MouseButtonState.Pressed)
            {
                Cursor = Cursors.Hand;
                cachedBlocksEditor.SelectOrStartMove(this, e);
                grid.CaptureMouse();
                e.Handled = true;
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == cachedBlocksEditor.SelectAndMoveMouseButton &&
                e.ButtonState == MouseButtonState.Released)
            {
                Cursor = Cursors.Arrow;
                cachedBlocksEditor.EndMove(this, e);
                grid.ReleaseMouseCapture();
                e.Handled = true;
            }
        }
    }
}
