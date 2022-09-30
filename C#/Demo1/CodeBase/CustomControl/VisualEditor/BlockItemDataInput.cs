using Common;
using Common.Types;
using ImageProcess.Buffer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace CustomControl.VisualEditor
{
    public class BlockItemDataInput : Control
    {
        static BlockItemDataInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockItemDataInput), new FrameworkPropertyMetadata(typeof(BlockItemDataInput)));
        }

        public string InputName
        {
            get { return (string)GetValue(InputNameProperty); }
            set { SetValue(InputNameProperty, value); }
        }
        public static readonly DependencyProperty InputNameProperty = DependencyProperty.Register(nameof(InputName), typeof(string), typeof(BlockItemDataInput));

        public Point PinSymbolCenter
        {
            get { return (Point)GetValue(PinSymbolCenterProperty); }
            set { SetValue(PinSymbolCenterProperty, value); }
        }
        public static readonly DependencyProperty PinSymbolCenterProperty = DependencyProperty.Register(nameof(PinSymbolCenter), typeof(Point), typeof(BlockItemDataInput));

        public Visibility PinSymbolStringVisibility
        {
            get { return (Visibility)GetValue(PinSymbolStringVisibilityProperty); }
            set { SetValue(PinSymbolStringVisibilityProperty, value); }
        }
        public static readonly DependencyProperty PinSymbolStringVisibilityProperty = DependencyProperty.Register(nameof(PinSymbolStringVisibility), typeof(Visibility), typeof(BlockItemDataInput));

        public bool Active
        {
            get { return (bool)GetValue(ActiveProperty); }
            set { SetValue(ActiveProperty, value); }
        }
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.Register(nameof(Active), typeof(bool), typeof(BlockItemDataInput), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ActiveChanged));

        private static void ActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool b && b &&
               d is BlockItemDataInput blockItemDataInput)
            {
                blockItemDataInput.Active = false;
                blockItemDataInput.Avtivated();
            }
        }


        readonly int animationDuration = 250;
        readonly DispatcherTimer activeTimer;
        Viewbox pinSymbolViewBox;
        IconContainer mouseOverIconContainer, activeIconContainer;
        BlocksEditor cachedBlocksEditor;


        public BlockItemDataInput()
        {
            activeTimer = new DispatcherTimer();
            activeTimer.Interval = TimeSpan.FromMilliseconds(animationDuration);
            activeTimer.Tick += ActiveTimer_Elapsed;

            DataContextChanged += (s, e) => SetColorAndAnimations();
            Loaded += (s, e) => cachedBlocksEditor = this.GetVisualParent<BlocksEditor>();
        }

        public override void OnApplyTemplate()
        {
            mouseOverIconContainer = GetTemplateChild("PART_PinSymbolContainerMouseOver") as IconContainer;
            activeIconContainer = GetTemplateChild("PART_PinSymbolContainerActive") as IconContainer;
            pinSymbolViewBox = GetTemplateChild("PART_PinSymbolViewBox") as Viewbox;

            mouseOverIconContainer.FadeInOutDuration = TimeSpan.FromMilliseconds(animationDuration);
            activeIconContainer.FadeInOutDuration = TimeSpan.FromMilliseconds(animationDuration);
        }

        private void SetColorAndAnimations()
        {
            ResourceDictionary colorResourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/CustomControl;component/Themes/VisualEditor/ColorStyle.xaml", UriKind.Absolute)
            };

            ResourceDictionary iconResourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Common;component/Resource/VectorIcons.xaml", UriKind.Absolute)
            };

            Image symbolDisplay = new Image();
            pinSymbolViewBox.Child = symbolDisplay;

            if (DataContext.GetType() is Type type &&
               type.IsGenericType && type.GetGenericArguments()[0] is Type type0)
            {
                if (type0 == typeof(string))
                {
                    DrawingImage circle = (DrawingImage)iconResourceDictionary["CircleDrawingImage"];
                    ((GeometryDrawing)circle.Drawing).Brush = new SolidColorBrush((Color)colorResourceDictionary["Data_String_Static_Color"]);
                    symbolDisplay.Source = circle;
                }

                else if (type0 == typeof(int?))
                {
                    DrawingImage circle = (DrawingImage)iconResourceDictionary["CircleDrawingImage"];
                    ((GeometryDrawing)circle.Drawing).Brush = new SolidColorBrush((Color)colorResourceDictionary["Data_Int_Static_Color"]);
                    symbolDisplay.Source = circle;
                }

                else if (type0 == typeof(float?))
                {
                    DrawingImage circle = (DrawingImage)iconResourceDictionary["CircleDrawingImage"];
                    ((GeometryDrawing)circle.Drawing).Brush = new SolidColorBrush((Color)colorResourceDictionary["Data_Float_Static_Color"]);
                    symbolDisplay.Source = circle;
                }

                else if (type0 == typeof(bool?))
                {
                    DrawingImage circle = (DrawingImage)iconResourceDictionary["CircleDrawingImage"];
                    ((GeometryDrawing)circle.Drawing).Brush = new SolidColorBrush((Color)colorResourceDictionary["Data_Bool_Static_Color"]);
                    symbolDisplay.Source = circle;
                }

                else if (type0 == typeof(ImageBufferBGRA32))
                    symbolDisplay.Source = (DrawingImage)iconResourceDictionary["RGBImageDrawingImage"];

                else if (type0.GetGenericTypeDefinition() == typeof(Container<>) &&
                         type0.IsGenericType && type0.GetGenericArguments()[0] is Type type00)
                {
                    if (type00 == typeof(string))
                    {
                        DrawingImage box = (DrawingImage)iconResourceDictionary["BoxDrawingImage"];
                        ((GeometryDrawing)box.Drawing).Brush = new SolidColorBrush((Color)colorResourceDictionary["Data_String_Static_Color"]);
                        symbolDisplay.Source = box;
                    }

                    else if (type00 == typeof(int?))
                    {
                        DrawingImage box = (DrawingImage)iconResourceDictionary["BoxDrawingImage"];
                        ((GeometryDrawing)box.Drawing).Brush = new SolidColorBrush((Color)colorResourceDictionary["Data_Int_Static_Color"]);
                        symbolDisplay.Source = box;
                    }

                    else if (type00 == typeof(float?))
                    {
                        DrawingImage box = (DrawingImage)iconResourceDictionary["BoxDrawingImage"];
                        ((GeometryDrawing)box.Drawing).Brush = new SolidColorBrush((Color)colorResourceDictionary["Data_Float_Static_Color"]);
                        symbolDisplay.Source = box;
                    }

                    else if (type00 == typeof(bool?))
                    {
                        DrawingImage box = (DrawingImage)iconResourceDictionary["BoxDrawingImage"];
                        ((GeometryDrawing)box.Drawing).Brush = new SolidColorBrush((Color)colorResourceDictionary["Data_Bool_Static_Color"]);
                        symbolDisplay.Source = box;
                    }

                    else
                    {
                        DrawingImage box = (DrawingImage)iconResourceDictionary["BoxDrawingImage"];
                        ((GeometryDrawing)box.Drawing).Brush = new SolidColorBrush((Color)colorResourceDictionary["Data_Object_Static_Color"]);
                        symbolDisplay.Source = box;
                    }
                }

                else
                {
                    DrawingImage circle = (DrawingImage)iconResourceDictionary["CircleDrawingImage"];
                    ((GeometryDrawing)circle.Drawing).Brush = new SolidColorBrush((Color)colorResourceDictionary["Data_Object_Static_Color"]);
                    symbolDisplay.Source = circle;
                }
            }
            else
            {
                DrawingImage circle = (DrawingImage)iconResourceDictionary["CircleDrawingImage"];
                ((GeometryDrawing)circle.Drawing).Brush = new SolidColorBrush((Color)colorResourceDictionary["Data_Object_Static_Color"]);
                symbolDisplay.Source = circle;
            }

            if (pinSymbolViewBox != null)
            {
                pinSymbolViewBox.MouseEnter += (s, e) => mouseOverIconContainer.Visible = true;
                pinSymbolViewBox.MouseLeave += (s, e) => mouseOverIconContainer.Visible = false;
                pinSymbolViewBox.MouseDown += UsedPinSymbol_MouseDown;
                pinSymbolViewBox.MouseUp += UsedPinSymbol_MouseUp;
            }
        }

        private void UsedPinSymbol_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == cachedBlocksEditor.WiringMouseButton && e.ButtonState == MouseButtonState.Pressed)
            {
                cachedBlocksEditor.StartWiring(this, e);
                e.Handled = true;
            }
        }

        private void UsedPinSymbol_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == cachedBlocksEditor.WiringMouseButton && e.ButtonState == MouseButtonState.Released)
            {
                cachedBlocksEditor.EndWiring(this, e);
                e.Handled = true;
            }
        }

        internal void UpdatePinSymbolCenterAndConnectionEnd()
        {
            if (pinSymbolViewBox != null && cachedBlocksEditor != null)
            {
                PinSymbolCenter = pinSymbolViewBox.TranslatePoint(new Point(pinSymbolViewBox.RenderSize.Width / 2, pinSymbolViewBox.RenderSize.Height / 2), cachedBlocksEditor.MainCanvas);

                if (cachedBlocksEditor.GetBlockItemDataConnection(this) is BlockItemDataConnection blockItemDataConnection)
                    blockItemDataConnection.End = PinSymbolCenter;
            }
        }

        private void Avtivated()
        {
            activeTimer.Stop();
            activeTimer.Start();
            activeIconContainer.Visible = true;
        }

        private void ActiveTimer_Elapsed(object sender, EventArgs e)
        {
            activeTimer.Stop();
            activeIconContainer.Visible = false;
        }
    }
}
