using Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace CustomControl.VisualEditor
{
    public class BlockItem : ContentControl, IMoveable, IHasCollisionWhileMotion
    {
        static BlockItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockItem), new FrameworkPropertyMetadata(typeof(BlockItem)));
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(BlockItem), new FrameworkPropertyMetadata(new CornerRadius(10d)));

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(double), typeof(BlockItem), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(XChangedCallback)));

        private static void XChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is BlockItem blockItem &&
               e.NewValue is double newValue)
            {
                blockItem.Left = blockItem.OffsetX + newValue;
            }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(double), typeof(BlockItem), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(YChangedCallback)));

        private static void YChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItem blockItem &&
               e.NewValue is double newValue)
            {
                blockItem.Top = blockItem.OffsetY + newValue;
            }
        }

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }
        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(BlockItem), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OffsetXChangedCallback), new CoerceValueCallback(OffsetXCoerceChangedCallback)));

        private static object OffsetXCoerceChangedCallback(DependencyObject d, object baseValue)
        {
            if (d is BlockItem blockItem &&
               blockItem.OffsetX == 0 && blockItem.X != blockItem.Left)
            {
                blockItem.X = blockItem.Left - blockItem.OffsetX;
            }

            return baseValue;
        }

        private static void OffsetXChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItem blockItem &&
               e.NewValue is double newValue)
            {
                blockItem.Left = blockItem.X + newValue;
            }
        }

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }
        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(BlockItem), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OffsetYChangedCallback), new CoerceValueCallback(OffsetYCoerceChangedCallback)));

        private static object OffsetYCoerceChangedCallback(DependencyObject d, object baseValue)
        {
            if (d is BlockItem blockItem &&
               blockItem.OffsetY == 0 && blockItem.Y != blockItem.Top)
            {
                blockItem.Y = blockItem.Top - blockItem.OffsetY;
            }

            return baseValue;
        }

        private static void OffsetYChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItem blockItem &&
               e.NewValue is double newValue)
            {
                blockItem.Top = blockItem.Y + newValue;
            }
        }

        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(nameof(Left), typeof(double), typeof(BlockItem), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, LeftChangedCallback));

        private static void LeftChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItem blockItem &&
                e.NewValue is double newValue)
            {
                blockItem.X = newValue - blockItem.OffsetX;
                blockItem.UpdateConnectionPinSymbolsCenterAndConnectionsEnds();
            }
        }

        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(nameof(Top), typeof(double), typeof(BlockItem), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TopChangedCallback));

        private static void TopChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItem blockItem &&
                e.NewValue is double newValue)
            {
                blockItem.Y = newValue - blockItem.OffsetY;
                blockItem.UpdateConnectionPinSymbolsCenterAndConnectionsEnds();
            }
        }

        public string ModuleName
        {
            get { return (string)GetValue(ModuleNameProperty); }
            set { SetValue(ModuleNameProperty, value); }
        }
        public static readonly DependencyProperty ModuleNameProperty = DependencyProperty.Register(nameof(ModuleName), typeof(string), typeof(BlockItem));

        public bool CollisionWhileMotion
        {
            get { return (bool)GetValue(CollisionWhileMotionProperty); }
            set { SetValue(CollisionWhileMotionProperty, value); }
        }
        public static readonly DependencyProperty CollisionWhileMotionProperty = DependencyProperty.Register(nameof(CollisionWhileMotion), typeof(bool), typeof(BlockItem), new PropertyMetadata((s, e) => ((BlockItem)s).CollisionWhileMotionChanged()));

        public bool ErrorOccurred
        {
            get { return (bool)GetValue(ErrorOccurredProperty); }
            set { SetValue(ErrorOccurredProperty, value); }
        }
        public static readonly DependencyProperty ErrorOccurredProperty = DependencyProperty.Register(nameof(ErrorOccurred), typeof(bool), typeof(BlockItem), new PropertyMetadata(ErrorOccurredChangedCallback));

        private static void ErrorOccurredChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItem blockItem)
                blockItem.ErrorOccurredChanged();
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(BlockItem));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(BlockItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) => ((BlockItem)s).IsSelectedChanged()));

        public bool ShowStatus
        {
            get { return (bool)GetValue(ShowStatusProperty); }
            set { SetValue(ShowStatusProperty, value); }
        }
        public static readonly DependencyProperty ShowStatusProperty = DependencyProperty.Register(nameof(ShowStatus), typeof(bool), typeof(BlockItem), new PropertyMetadata(true, (s, e) => ((BlockItem)s).ShowStatusChanged()));


        public List<BlockItemDataInput> DataInputs { get; }
        public List<BlockItemDataOutput> DataOutputs { get; }
        public List<BlockItemTriggerInput> TriggerInputs { get; }
        public List<BlockItemTriggerOutput> TriggerOutputs { get; }


        Grid mainGrid, statusGrid;
        ItemsControl dataInputItemsControl;
        ItemsControl dataOutputItemsControl;
        ItemsControl triggerInputItemsControl;
        ItemsControl triggerOutputItemsControl;
        Grid errorIcon, okIcon;
        DoubleAnimation okIconFadeIn, okIconFadeOut, errorIconFadeIn, errorIconFadeOut;
        Border collisionBorder, selectionBorder;
        ColorAnimation collisionBorderFadeIn, collisionBorderFadeOut, selectionBorderFadeIn, selectionBorderFadeOut;
        SolidColorBrush collisionBorderBrush, selectionBorderBrush;
        BlocksEditor cachedBlocksEditor;


        public BlockItem()
        {
            DataInputs = new List<BlockItemDataInput>();
            TriggerInputs = new List<BlockItemTriggerInput>();
            DataOutputs = new List<BlockItemDataOutput>();
            TriggerOutputs = new List<BlockItemTriggerOutput>();

            Loaded += BlockItem_Loaded;
            SizeChanged += BlockItem_SizeChanged;
        }

        private void BlockItem_Loaded(object sender, RoutedEventArgs e)
        {
            cachedBlocksEditor = this.GetVisualParent<BlocksEditor>();

            if (X == Left && Y == Top)
            {
                    Point? lastContextMenuOpenedPosition = cachedBlocksEditor.LastContextMenuOpenPosition;

                    if (lastContextMenuOpenedPosition.HasValue)
                    {
                        Left = lastContextMenuOpenedPosition.Value.X;
                        Top = lastContextMenuOpenedPosition.Value.Y;
                    }
            }

            ShowStatusChanged();
        }

        private void BlockItem_SizeChanged(object sender, SizeChangedEventArgs e)
            => UpdateConnectionPinSymbolsCenterAndConnectionsEnds();

        public override void OnApplyTemplate()
        {
            mainGrid = GetTemplateChild("PART_MainGrid") as Grid;

            if (mainGrid != null)
            {
                mainGrid.MouseDown += MainGrid_MouseDown;
                mainGrid.MouseUp += MainGrid_MouseUp;
            }

            dataInputItemsControl = GetTemplateChild("PART_DataInputItemsControl") as ItemsControl;
            dataOutputItemsControl = GetTemplateChild("PART_DataOutputItemsControl") as ItemsControl;
            triggerInputItemsControl = GetTemplateChild("PART_TriggerInputItemsControl") as ItemsControl;
            triggerOutputItemsControl = GetTemplateChild("PART_TriggerOutputItemsControl") as ItemsControl;

            if (dataInputItemsControl != null)
                dataInputItemsControl.ItemsSource = DataInputs;

            if (dataOutputItemsControl != null)
                dataOutputItemsControl.ItemsSource = DataOutputs;

            if (triggerInputItemsControl != null)
                triggerInputItemsControl.ItemsSource = TriggerInputs;

            if (triggerOutputItemsControl != null)
                triggerOutputItemsControl.ItemsSource = TriggerOutputs;

            if (GetTemplateChild("PART_RemoveButton") is IconButton removeButton)
                removeButton.Click += RemoveButton_Click;

            errorIcon = GetTemplateChild("PART_ErrorIcon") as Grid;
            okIcon = GetTemplateChild("PART_OkIcon") as Grid;

            okIconFadeIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(250));
            okIconFadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(250));
            errorIconFadeIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(250));
            errorIconFadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(250));

            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/CustomControl;component/Themes/VisualEditor/ColorStyle.xaml", UriKind.Absolute)
            };

            collisionBorder = GetTemplateChild("PART_CollisionBorder") as Border;
            selectionBorder = GetTemplateChild("PART_SelectionBorder") as Border;

            if (resourceDictionary != null)
            {
                if (collisionBorder != null)
                {
                    collisionBorderBrush = new SolidColorBrush((Color)resourceDictionary["CollisionBorder_Static_Color"]);
                    collisionBorder.BorderBrush = collisionBorderBrush;
                    collisionBorderFadeIn = new ColorAnimation((Color)resourceDictionary["CollisionBorder_Limit_Color"], TimeSpan.FromMilliseconds(250));
                    collisionBorderFadeOut = new ColorAnimation((Color)resourceDictionary["CollisionBorder_Static_Color"], TimeSpan.FromMilliseconds(250));
                }

                if (selectionBorder != null)
                {
                    selectionBorderBrush = new SolidColorBrush((Color)resourceDictionary["SelectionBorder_Static_Color"]);
                    selectionBorder.BorderBrush = selectionBorderBrush;
                    selectionBorderFadeIn = new ColorAnimation((Color)resourceDictionary["SelectionBorder_Selected_Color"], TimeSpan.FromMilliseconds(250));
                    selectionBorderFadeOut = new ColorAnimation((Color)resourceDictionary["SelectionBorder_Static_Color"], TimeSpan.FromMilliseconds(250));
                }
            }

            statusGrid = GetTemplateChild("PART_StatusGrid") as Grid;
        }

        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == cachedBlocksEditor.SelectAndMoveMouseButton && 
               e.ButtonState == MouseButtonState.Pressed)
            {
                Cursor = Cursors.Hand;
                cachedBlocksEditor.SelectOrStartMove(this, e);
                mainGrid.CaptureMouse();
                e.Handled = true;
            }
        }

        private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == cachedBlocksEditor.SelectAndMoveMouseButton &&
                e.ButtonState == MouseButtonState.Released)
            {
                Cursor = Cursors.Arrow;
                cachedBlocksEditor.EndMove(this, e);
                mainGrid.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            cachedBlocksEditor.RemoveChild(this);
        }

        internal void UpdateConnectionPinSymbolsCenterAndConnectionsEnds()
        {
            foreach (BlockItemDataInput blockItemDataInput in DataInputs)
                blockItemDataInput.UpdatePinSymbolCenterAndConnectionEnd();

            foreach (BlockItemDataOutput blockItemDataOutput in DataOutputs)
                blockItemDataOutput.UpdatePinSymbolCenterAndConnectionEnd();

            foreach (BlockItemTriggerInput blockItemTriggerInput in TriggerInputs)
                blockItemTriggerInput.UpdatePinSymbolCenterAndConnectionEnd();

            foreach (BlockItemTriggerOutput blockItemTriggerOutput in TriggerOutputs)
                blockItemTriggerOutput.UpdatePinSymbolCenterAndConnectionEnd();
        }

        private void ErrorOccurredChanged()
        {
            if(ErrorOccurred)
            {
                errorIconFadeIn.Completed += errorIconFadeInCompleted;
                errorIconFadeOut.Completed += errorIconFadeOutCompleted;

                errorIcon.BeginAnimation(OpacityProperty, errorIconFadeIn);
                okIcon.BeginAnimation(OpacityProperty, okIconFadeOut);
            }
            else
            {
                errorIconFadeIn.Completed -= errorIconFadeInCompleted;
                errorIconFadeOut.Completed -= errorIconFadeOutCompleted;

                okIcon.BeginAnimation(OpacityProperty, okIconFadeIn);
                errorIcon.BeginAnimation(OpacityProperty, errorIconFadeOut);
            }
        }

        private void errorIconFadeInCompleted(object sender, EventArgs e)
            => errorIcon.BeginAnimation(OpacityProperty, errorIconFadeOut);

        private void errorIconFadeOutCompleted(object sender, EventArgs e)
            => errorIcon.BeginAnimation(OpacityProperty, errorIconFadeIn);

        private void CollisionWhileMotionChanged()
        {
            if (CollisionWhileMotion)
                collisionBorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, collisionBorderFadeIn);
            else
                collisionBorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, collisionBorderFadeOut);
        }

        private void IsSelectedChanged()
        {
            if (IsSelected)
                selectionBorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, selectionBorderFadeIn);
            else
                selectionBorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, selectionBorderFadeOut);
        }

        private void ShowStatusChanged()
        {
            if(statusGrid != null)
            {
                if (ShowStatus)
                    statusGrid.Visibility = Visibility.Visible;
                else
                    statusGrid.Visibility = Visibility.Hidden;
            }
        }
    }
}
