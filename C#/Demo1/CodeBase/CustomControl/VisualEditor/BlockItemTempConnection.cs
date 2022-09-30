using System;
using System.Windows;
using System.Windows.Controls;


namespace CustomControl.VisualEditor
{
    public class BlockItemTempConnection : Control
    {
        static BlockItemTempConnection()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockItemTempConnection), new FrameworkPropertyMetadata(typeof(BlockItemTempConnection)));
        }

        public Point Start
        {
            get { return (Point)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }
        public static readonly DependencyProperty StartProperty = DependencyProperty.Register(nameof(Start), typeof(Point), typeof(BlockItemTempConnection), new PropertyMetadata(new Point(0,0), StartPointChanged));

        private static void StartPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is BlockItemTempConnection blockItemTempConnection)
                blockItemTempConnection.UpdateStartEndDirectionPoints();
        }

        public Point StartDirection
        {
            get { return (Point)GetValue(StartDirectionProperty); }
            set { SetValue(StartDirectionProperty, value); }
        }
        public static readonly DependencyProperty StartDirectionProperty = DependencyProperty.Register(nameof(StartDirection), typeof(Point), typeof(BlockItemTempConnection), new PropertyMetadata(new Point(100, 0)));

        public Point EndDirection
        {
            get { return (Point)GetValue(EndDirectionProperty); }
            set { SetValue(EndDirectionProperty, value); }
        }
        public static readonly DependencyProperty EndDirectionProperty = DependencyProperty.Register(nameof(EndDirection), typeof(Point), typeof(BlockItemTempConnection), new PropertyMetadata(new Point(0, 100)));

        public Point End
        {
            get { return (Point)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }
        public static readonly DependencyProperty EndProperty = DependencyProperty.Register(nameof(End), typeof(Point), typeof(BlockItemTempConnection), new PropertyMetadata(new Point(100, 100), EndPointChanged));

        private static void EndPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BlockItemTempConnection blockItemTempConnection)
                blockItemTempConnection.UpdateStartEndDirectionPoints();
        }

        private void UpdateStartEndDirectionPoints()
        {
            double xAbs = Math.Abs(Start.X - End.X) / 2;
            StartDirection = new Point(Start.X + xAbs, Start.Y);
            EndDirection = new Point(End.X - xAbs, End.Y);
        }
    }
}
