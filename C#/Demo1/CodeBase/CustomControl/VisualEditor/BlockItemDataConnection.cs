using Common;
using Common.Types;
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
    public class BlockItemDataConnection : Control, IHasQuadraticBezierCurvePoints, ISelectable
    {
        static BlockItemDataConnection()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockItemDataConnection), new FrameworkPropertyMetadata(typeof(BlockItemDataConnection)));
        }

        public Point Start
        {
            get { return (Point)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }
        public static readonly DependencyProperty StartProperty = DependencyProperty.Register(nameof(Start), typeof(Point), typeof(BlockItemDataConnection), new PropertyMetadata(new Point(0,0), StartPointChanged));

        private static void StartPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is BlockItemDataConnection blockItemDataConnection)
                blockItemDataConnection.UpdateStartEndDirectionPoints();
        }

        public Point StartDirection
        {
            get { return (Point)GetValue(StartDirectionProperty); }
            set { SetValue(StartDirectionProperty, value); }
        }
        public static readonly DependencyProperty StartDirectionProperty = DependencyProperty.Register(nameof(StartDirection), typeof(Point), typeof(BlockItemDataConnection), new PropertyMetadata(new Point(100, 0)));

        public Point EndDirection
        {
            get { return (Point)GetValue(EndDirectionProperty); }
            set { SetValue(EndDirectionProperty, value); }
        }
        public static readonly DependencyProperty EndDirectionProperty = DependencyProperty.Register(nameof(EndDirection), typeof(Point), typeof(BlockItemDataConnection), new PropertyMetadata(new Point(0, 100)));

        public Point End
        {
            get { return (Point)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }
        public static readonly DependencyProperty EndProperty = DependencyProperty.Register(nameof(End), typeof(Point), typeof(BlockItemDataConnection), new PropertyMetadata(new Point(100, 100), EndPointChanged));

        private static void EndPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItemDataConnection blockItemDataConnection)
                blockItemDataConnection.UpdateStartEndDirectionPoints();
        }

        public double BackgroundPathStrokeThickness
        {
            get { return (double)GetValue(BackgroundPathStrokeThicknessProperty); }
            set { SetValue(BackgroundPathStrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty BackgroundPathStrokeThicknessProperty = DependencyProperty.Register(nameof(BackgroundPathStrokeThickness), typeof(double), typeof(BlockItemDataConnection), new PropertyMetadata(6d));

        public double PathStrokeThickness
        {
            get { return (double)GetValue(PathStrokeThicknessProperty); }
            set { SetValue(PathStrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty PathStrokeThicknessProperty = DependencyProperty.Register(nameof(PathStrokeThickness), typeof(double), typeof(BlockItemDataConnection), new PropertyMetadata(3d));

        public object OutputDataContext
        {
            get { return GetValue(OutputDataContextProperty); }
            set { SetValue(OutputDataContextProperty, value); }
        }
        public static readonly DependencyProperty OutputDataContextProperty = DependencyProperty.Register(nameof(OutputDataContext), typeof(object), typeof(BlockItemDataConnection));

        public object InputDataContext
        {
            get { return GetValue(InputDataContextProperty); }
            set { SetValue(InputDataContextProperty, value); }
        }
        public static readonly DependencyProperty InputDataContextProperty = DependencyProperty.Register(nameof(InputDataContext), typeof(object), typeof(BlockItemDataConnection));

        public bool Active
        {
            get { return (bool)GetValue(ActiveProperty); }
            set { SetValue(ActiveProperty, value); }
        }
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.Register(nameof(Active), typeof(bool), typeof(BlockItemDataConnection), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ActiveChanged));

        private static void ActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool b && b &&
               d is BlockItemDataConnection blockItemDataConnection)
            {
                blockItemDataConnection.Active = false;
                blockItemDataConnection.Avtivated();
            }
        }

        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }
        public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(BlockItemDataConnection));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(BlockItemDataConnection), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) => ((BlockItemDataConnection)s).IsSelectedChanged()));


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


        public BlockItemDataConnection()
        {
            activeSignals = new List<SignalSymbol>();
            Loaded += BlockItemDataConnection_Loaded;
        }

        private void BlockItemDataConnection_Loaded(object sender, RoutedEventArgs e)
        {
            cachedBlocksEditor = this.GetVisualParent<BlocksEditor>();
            cachedBlocksEditor.GetBlockItemDataOutput(this).UpdatePinSymbolCenterAndConnectionEnd();
            cachedBlocksEditor.GetBlockItemDataInput(this).UpdatePinSymbolCenterAndConnectionEnd();
        }

        public override void OnApplyTemplate()
        {
            MenuItem menuItem = GetTemplateChild("PART_RemoveMenuItem") as MenuItem;
            menuItem.Click += (s, e) => cachedBlocksEditor.RemoveChild(this);

            grid = GetTemplateChild("PART_Grid") as Grid;

            if (grid != null)
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

            if (DataContext.GetType() is Type type &&
               type.IsGenericType && type.GetGenericArguments()[0] is Type type0)
            {
                if (type0 == typeof(string))
                    path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Data_String_Static_Color"]);
                else if (type0 == typeof(int?))
                    path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Data_Int_Static_Color"]);
                else if (type0 == typeof(float?))
                    path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Data_Float_Static_Color"]);
                else if (type0 == typeof(bool?))
                    path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Data_Bool_Static_Color"]);
                else if (type0.IsGenericType && type0.GetGenericTypeDefinition() == typeof(Container<>))
                {
                    StrokeDashArray = new DoubleCollection(new[] { 1d, 2d });

                    if (type0.IsGenericType && type0.GetGenericArguments()[0] is Type type00)
                    {
                        if (type00 == typeof(string))
                            path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Data_String_Static_Color"]);
                        else if (type00 == typeof(int?))
                            path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Data_Int_Static_Color"]);
                        else if (type00 == typeof(float?))
                            path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Data_Float_Static_Color"]);
                        else if (type00 == typeof(bool?))
                            path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Data_Bool_Static_Color"]);
                        else
                            path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Data_Object_Static_Color"]);
                    }
                }
                else
                    path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Data_Object_Static_Color"]);
            }
            else
                path.Stroke = pathStroke = new SolidColorBrush((Color)resourceDictionary["Data_Object_Static_Color"]);

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

            if (signalCanvas == null && cachedBlocksEditor.SignalCanvas is Canvas canvas)
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
