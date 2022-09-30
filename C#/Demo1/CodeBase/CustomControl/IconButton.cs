using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace CustomControl
{
    public class IconButton : ContentControl
    {
        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));
        }

        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(IconButton));

        public Brush MouseOverBrush
        {
            get { return (Brush)GetValue(MouseOverBrushProperty); }
            set { SetValue(MouseOverBrushProperty, value); }
        }
        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register(nameof(MouseOverBrush), typeof(Brush), typeof(IconButton));

        public Brush MousePressBrush
        {
            get { return (Brush)GetValue(MousePressBrushProperty); }
            set { SetValue(MousePressBrushProperty, value); }
        }
        public static readonly DependencyProperty MousePressBrushProperty = DependencyProperty.Register(nameof(MousePressBrush), typeof(Brush), typeof(IconButton));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(IconButton));

        public double FadeInOutDuration
        {
            get { return (double)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(nameof(FadeInOutDuration), typeof(double), typeof(IconButton), new PropertyMetadata(250d, (d, e) => ((IconButton)d).DurationChanged()));

        

        DoubleAnimation mouseOverFadeIn, mouseOverFadeOut, mouseButtonPressFadeIn, mouseButtonReleaseFadeOut;
        Border mouseOverBorder, mousePressReleaseBorder;


        public IconButton()
        {
            mouseOverFadeIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(FadeInOutDuration));
            mouseOverFadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(FadeInOutDuration));
            mouseButtonPressFadeIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(FadeInOutDuration / 2d));
            mouseButtonReleaseFadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(FadeInOutDuration / 2d));
        }

        public override void OnApplyTemplate()
        {
            mouseOverBorder = GetTemplateChild("PART_MouseOverBorder") as Border;
            mousePressReleaseBorder = GetTemplateChild("PART_MousePressReleaseBorder") as Border;

            MouseEnter += (s, e) =>
            {
                if (mouseOverBorder != null)
                    mouseOverBorder.BeginAnimation(OpacityProperty, mouseOverFadeIn);

                e.Handled = true;
            };

            MouseLeave += (s, e) =>
            {
                if (mouseOverBorder != null)
                {
                    mouseOverBorder.BeginAnimation(OpacityProperty, mouseOverFadeOut);
                    mousePressReleaseBorder.BeginAnimation(OpacityProperty, mouseButtonReleaseFadeOut);
                }

                e.Handled = true;
            };

            MouseLeftButtonDown += (s, e) =>
            {
                if (mousePressReleaseBorder != null)
                    mousePressReleaseBorder.BeginAnimation(OpacityProperty, mouseButtonPressFadeIn);

                e.Handled = true;
            };

            MouseLeftButtonUp += (s, e) =>
            {
                if (mousePressReleaseBorder != null)
                    mousePressReleaseBorder.BeginAnimation(OpacityProperty, mouseButtonReleaseFadeOut);

                RaiseEvent(new RoutedEventArgs(ClickEvent, this));
                e.Handled = true;
            };
        }

        private void DurationChanged()
        {
            mouseOverFadeIn.Duration = TimeSpan.FromMilliseconds(FadeInOutDuration);
            mouseOverFadeOut.Duration = TimeSpan.FromMilliseconds(FadeInOutDuration);
            mouseButtonPressFadeIn.Duration = TimeSpan.FromMilliseconds(FadeInOutDuration / 2d);
            mouseButtonReleaseFadeOut.Duration = TimeSpan.FromMilliseconds(FadeInOutDuration / 2d);
        }
    }
}
