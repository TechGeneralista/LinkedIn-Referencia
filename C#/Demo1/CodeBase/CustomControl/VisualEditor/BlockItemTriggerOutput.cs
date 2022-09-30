using Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace CustomControl.VisualEditor
{
    public class BlockItemTriggerOutput : Control
    {
        static BlockItemTriggerOutput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockItemTriggerOutput), new FrameworkPropertyMetadata(typeof(BlockItemTriggerOutput)));
        }

        public string OutputName
        {
            get { return (string)GetValue(OutputNameProperty); }
            set { SetValue(OutputNameProperty, value); }
        }
        public static readonly DependencyProperty OutputNameProperty = DependencyProperty.Register(nameof(OutputName), typeof(string), typeof(BlockItemTriggerOutput));

        public Point PinSymbolCenter
        {
            get { return (Point)GetValue(PinSymbolCenterProperty); }
            set { SetValue(PinSymbolCenterProperty, value); }
        }
        public static readonly DependencyProperty PinSymbolCenterProperty = DependencyProperty.Register(nameof(PinSymbolCenter), typeof(Point), typeof(BlockItemTriggerOutput));

        public bool Active
        {
            get { return (bool)GetValue(ActiveProperty); }
            set { SetValue(ActiveProperty, value); }
        }
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.Register(nameof(Active), typeof(bool), typeof(BlockItemTriggerOutput), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ActiveChanged));

        private static void ActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool b && b &&
               d is BlockItemTriggerOutput blockItemTriggerOutput)
            {
                blockItemTriggerOutput.Active = false;
                blockItemTriggerOutput.Avtivated();
            }
        }


        readonly int animationDuration = 250;
        readonly DispatcherTimer activeTimer;
        Path pinSymbol;
        IconContainer mouseOverIconContainer, activeIconContainer;
        BlocksEditor cachedBlocksEditor;


        public BlockItemTriggerOutput()
        {
            activeTimer = new DispatcherTimer();
            activeTimer.Interval = TimeSpan.FromMilliseconds(animationDuration);
            activeTimer.Tick += ActiveTimer_Elapsed;
            Loaded += (s, e) => cachedBlocksEditor = this.GetVisualParent<BlocksEditor>();
        }

        public override void OnApplyTemplate()
        {
            pinSymbol = GetTemplateChild("PART_PinSymbol") as Path;
            mouseOverIconContainer = GetTemplateChild("PART_PinSymbolContainerMouseOver") as IconContainer;
            activeIconContainer = GetTemplateChild("PART_PinSymbolContainerActive") as IconContainer;

            mouseOverIconContainer.FadeInOutDuration = TimeSpan.FromMilliseconds(animationDuration);
            activeIconContainer.FadeInOutDuration = TimeSpan.FromMilliseconds(animationDuration);

            if(pinSymbol != null)
            {
                pinSymbol.MouseEnter += (s, e) => mouseOverIconContainer.Visible = true;
                pinSymbol.MouseLeave += (s, e) => mouseOverIconContainer.Visible = false;
                pinSymbol.MouseDown += PinSymbol_MouseDown;
                pinSymbol.MouseUp += PinSymbol_MouseUp;
            }
        }

        private void PinSymbol_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == cachedBlocksEditor.WiringMouseButton && e.ButtonState == MouseButtonState.Released)
            {
                cachedBlocksEditor.EndWiring(this, e);
                e.Handled = true;
            }
        }

        private void PinSymbol_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == cachedBlocksEditor.WiringMouseButton && e.ButtonState == MouseButtonState.Pressed)
            {
                cachedBlocksEditor.StartWiring(this, e);
                e.Handled = true;
            }
        }

        internal void UpdatePinSymbolCenterAndConnectionEnd()
        {
            if (pinSymbol != null && cachedBlocksEditor != null)
            {
                PinSymbolCenter = pinSymbol.TranslatePoint(new Point(pinSymbol.RenderSize.Width / 2, pinSymbol.RenderSize.Height / 2), cachedBlocksEditor.MainCanvas);

                if (cachedBlocksEditor.GetBlockItemTriggerConnection(this) is BlockItemTriggerConnection blockItemTriggerConnection)
                    blockItemTriggerConnection.Start = PinSymbolCenter;
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
