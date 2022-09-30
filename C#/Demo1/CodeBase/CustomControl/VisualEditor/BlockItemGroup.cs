using Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace CustomControl.VisualEditor
{
    public class BlockItemGroup : Control, IMoveable
    {
        static BlockItemGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockItemGroup), new FrameworkPropertyMetadata(typeof(BlockItemGroup)));
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(BlockItemGroup), new FrameworkPropertyMetadata(new CornerRadius(10d)));

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(double), typeof(BlockItemGroup), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(XChangedCallback)));

        private static void XChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItemGroup blockItemGroup &&
               e.NewValue is double newValue)
            {
                blockItemGroup.Left = blockItemGroup.OffsetX + newValue;
            }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(double), typeof(BlockItemGroup), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(YChangedCallback)));

        private static void YChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItemGroup blockItemGroup &&
               e.NewValue is double newValue)
            {
                blockItemGroup.Top = blockItemGroup.OffsetY + newValue;
            }
        }

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }
        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(BlockItemGroup), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OffsetXChangedCallback), new CoerceValueCallback(OffsetXCoerceChangedCallback)));

        private static object OffsetXCoerceChangedCallback(DependencyObject d, object baseValue)
        {
            if(d is BlockItemGroup blockItemGroup &&
               blockItemGroup.OffsetX == 0 && blockItemGroup.X != blockItemGroup.Left)
            {
                blockItemGroup.X = blockItemGroup.Left - blockItemGroup.OffsetX;
            }

            return baseValue;
        }

        private static void OffsetXChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItemGroup blockItemGroup &&
               e.NewValue is double newValue)
            {
                blockItemGroup.Left = blockItemGroup.X + newValue;
            }
        }

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }
        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(BlockItemGroup), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OffsetYChangedCallback), new CoerceValueCallback(OffsetYCoerceChangedCallback)));

        private static object OffsetYCoerceChangedCallback(DependencyObject d, object baseValue)
        {
            if (d is BlockItemGroup blockItemGroup &&
               blockItemGroup.OffsetY == 0 && blockItemGroup.Y != blockItemGroup.Top)
            {
                blockItemGroup.Y = blockItemGroup.Top - blockItemGroup.OffsetY;
            }

            return baseValue;
        }

        private static void OffsetYChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItemGroup blockItemGroup &&
               e.NewValue is double newValue)
            {
                blockItemGroup.Top = blockItemGroup.Y + newValue;
            }
        }

        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(nameof(Left), typeof(double), typeof(BlockItemGroup), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, LeftChangedCallback));

        private static void LeftChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItemGroup blockItemGroup &&
                e.NewValue is double newValue)
            {
                blockItemGroup.X = newValue - blockItemGroup.OffsetX;
            }
        }

        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(nameof(Top), typeof(double), typeof(BlockItemGroup), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TopChangedCallback));

        private static void TopChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItemGroup blockItemGroup &&
                e.NewValue is double newValue)
            {
                blockItemGroup.Y = newValue - blockItemGroup.OffsetY;
            }
        }

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }
        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(BlockItemGroup), new FrameworkPropertyMetadata("N/A", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(BlockItemGroup), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) => ((BlockItemGroup)s).IsSelectedChanged()));


        Border mainBorder;
        Border selectionBorder;
        ColorAnimation selectionBorderFadeIn, selectionBorderFadeOut, resizeEllipseMouseEnter, resizeEllipseMouseLeave, moveEllipseMouseEnter, moveEllipseMouseLeave;
        SolidColorBrush selectionBorderBrush, resizeEllipseFillBrush, moveEllipseFillBrush;
        Ellipse resizeEllipse, moveEllipse;
        State state;
        Point lastMousePosition;


        public BlockItemGroup()
        {
            Loaded += BlockItemGroup_Loaded;
        }

        private void BlockItemGroup_Loaded(object sender, RoutedEventArgs e)
        {
            if(X == Left && Y == Top)
            {
                if (this.GetVisualParent<BlocksEditor>() is BlocksEditor blocksEditor)
                {
                    Point? lastContextMenuOpenedPosition = blocksEditor.LastContextMenuOpenPosition;

                    if (lastContextMenuOpenedPosition.HasValue)
                    {
                        Left = lastContextMenuOpenedPosition.Value.X;
                        Top = lastContextMenuOpenedPosition.Value.Y;
                    }
                }
            }
        }

        public override void OnApplyTemplate()
        {
            if (GetTemplateChild("PART_RemoveButton") is IconButton removeButton)
                removeButton.Click += RemoveButton_Click;

            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/CustomControl;component/Themes/VisualEditor/ColorStyle.xaml", UriKind.Absolute)
            };

            selectionBorder = GetTemplateChild("PART_SelectionBorder") as Border;

            if (resourceDictionary != null)
            {
                if (selectionBorder != null)
                {
                    selectionBorderBrush = new SolidColorBrush((Color)resourceDictionary["SelectionBorder_Static_Color"]);
                    selectionBorder.BorderBrush = selectionBorderBrush;
                    selectionBorderFadeIn = new ColorAnimation((Color)resourceDictionary["SelectionBorder_Selected_Color"], TimeSpan.FromMilliseconds(250));
                    selectionBorderFadeOut = new ColorAnimation((Color)resourceDictionary["SelectionBorder_Static_Color"], TimeSpan.FromMilliseconds(250));
                }
            }

            mainBorder = GetTemplateChild("PART_MainBorder") as Border;
            resizeEllipse = GetTemplateChild("PART_ResizeEllipse") as Ellipse;

            if(resizeEllipse != null)
            {
                resizeEllipseMouseEnter = new ColorAnimation((Color)resourceDictionary["MouseEnter_Color"], TimeSpan.FromMilliseconds(250));
                resizeEllipseMouseLeave = new ColorAnimation(((SolidColorBrush)BorderBrush).Color, TimeSpan.FromMilliseconds(250));

                resizeEllipse.MouseDown += ResizeEllipse_MouseDown;
                resizeEllipse.MouseUp += ResizeEllipse_MouseUp;
                resizeEllipseFillBrush = new SolidColorBrush(((SolidColorBrush)BorderBrush).Color);
                resizeEllipse.Fill = resizeEllipseFillBrush;

                if (resourceDictionary != null)
                {
                    resizeEllipse.MouseEnter += ResizeEllipse_MouseEnter;
                    resizeEllipse.MouseLeave += ResizeEllipse_MouseLeave;
                }
            }

            moveEllipse = GetTemplateChild("PART_MoveEllipse") as Ellipse;

            if (moveEllipse != null)
            {
                moveEllipseMouseEnter = new ColorAnimation((Color)resourceDictionary["MouseEnter_Color"], TimeSpan.FromMilliseconds(250));
                moveEllipseMouseLeave = new ColorAnimation(((SolidColorBrush)BorderBrush).Color, TimeSpan.FromMilliseconds(250));

                moveEllipse.MouseDown += MoveEllipse_MouseDown;
                moveEllipse.MouseUp += MoveEllipse_MouseUp;
                moveEllipseFillBrush = new SolidColorBrush(((SolidColorBrush)BorderBrush).Color);
                moveEllipse.Fill = moveEllipseFillBrush;

                if (resourceDictionary != null)
                {
                    moveEllipse.MouseEnter += MoveEllipse_MouseEnter;
                    moveEllipse.MouseLeave += MoveEllipse_MouseLeave;
                }
            }

            MouseMove += BlockItemGroup_MouseMove;
        }

        private void MoveEllipse_MouseEnter(object sender, MouseEventArgs e)
        {
            if (moveEllipseFillBrush != null && this.GetVisualParent<BlocksEditor>() is BlocksEditor blocksEditor && blocksEditor.State == State.Ready)
                moveEllipseFillBrush.BeginAnimation(SolidColorBrush.ColorProperty, moveEllipseMouseEnter);
        }

        private void MoveEllipse_MouseLeave(object sender, MouseEventArgs e)
        {
            if (moveEllipseFillBrush != null && this.GetVisualParent<BlocksEditor>() is BlocksEditor blocksEditor && blocksEditor.State == State.Ready)
                moveEllipseFillBrush.BeginAnimation(SolidColorBrush.ColorProperty, moveEllipseMouseLeave);
        }

        private void ResizeEllipse_MouseEnter(object sender, MouseEventArgs e)
        {
            if (resizeEllipseFillBrush != null && this.GetVisualParent<BlocksEditor>() is BlocksEditor blocksEditor && blocksEditor.State == State.Ready)
                resizeEllipseFillBrush.BeginAnimation(SolidColorBrush.ColorProperty, resizeEllipseMouseEnter);
        }

        private void ResizeEllipse_MouseLeave(object sender, MouseEventArgs e)
        {
            if (resizeEllipseFillBrush != null && this.GetVisualParent<BlocksEditor>() is BlocksEditor blocksEditor && blocksEditor.State == State.Ready)
                resizeEllipseFillBrush.BeginAnimation(SolidColorBrush.ColorProperty, resizeEllipseMouseLeave);
        }

        private void ResizeEllipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(this.GetVisualParent<BlocksEditor>() is BlocksEditor blocksEditor &&
               e.ChangedButton == blocksEditor.ResizeMouseButton && e.ButtonState == MouseButtonState.Pressed)
            {
                Cursor = Cursors.Hand;
                resizeEllipse.CaptureMouse();
                state = State.Resize;
                lastMousePosition = e.GetPosition(mainBorder);
                e.Handled = true;
            }
        }

        private void BlockItemGroup_MouseMove(object sender, MouseEventArgs e)
        {
            if(state == State.Resize)
            {
                Point newMousePosition = e.GetPosition(mainBorder);
                Vector vector = newMousePosition - lastMousePosition;

                double newWidth = Width + vector.X;
                double newHeight = Height + vector.Y;

                if (newWidth < MinWidth)
                    newWidth = MinWidth;

                if (newHeight < MinHeight)
                    newHeight = MinHeight;

                Width = newWidth;
                Height = newHeight;

                lastMousePosition = newMousePosition;
            }
        }

        private void ResizeEllipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GetVisualParent<BlocksEditor>() is BlocksEditor blocksEditor &&
                e.ChangedButton == blocksEditor.ResizeMouseButton && e.ButtonState == MouseButtonState.Released)
            {
                if (state == State.Resize)
                {
                    Cursor = Cursors.Arrow;
                    resizeEllipse.ReleaseMouseCapture();
                    state = State.Ready;
                }
                else
                    blocksEditor.Reset();
                
                e.Handled = true;
            }
        }

        private void MoveEllipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.GetVisualParent<BlocksEditor>() is BlocksEditor blocksEditor &&
               e.ChangedButton == blocksEditor.SelectAndMoveMouseButton &&
               e.ButtonState == MouseButtonState.Pressed)
            {
                Cursor = Cursors.Hand;
                blocksEditor.SelectOrStartMove(this, e);
                moveEllipse.CaptureMouse();
                e.Handled = true;
            }
        }

        private void MoveEllipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GetVisualParent<BlocksEditor>() is BlocksEditor blocksEditor &&
                e.ChangedButton == blocksEditor.SelectAndMoveMouseButton &&
                e.ButtonState == MouseButtonState.Released)
            {
                Cursor = Cursors.Arrow;
                blocksEditor.EndMove(this, e);
                moveEllipse.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.GetVisualParent<BlocksEditor>() is BlocksEditor blocksEditor)
                blocksEditor.RemoveChild(this);
        }

        private void IsSelectedChanged()
        {
            if (IsSelected)
                selectionBorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, selectionBorderFadeIn);
            else
                selectionBorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, selectionBorderFadeOut);

            if (this.GetVisualParent<BlocksEditor>() is BlocksEditor blocksEditor)
                blocksEditor.GroupSelectionChanged(this);
        }
    }
}
