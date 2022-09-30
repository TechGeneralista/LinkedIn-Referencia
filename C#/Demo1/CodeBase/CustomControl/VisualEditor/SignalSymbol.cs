using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace CustomControl.VisualEditor
{
    internal class SignalSymbol : Shape
    {
        public EventHandler Finished;

        protected override Geometry DefiningGeometry => new EllipseGeometry(new Point(0, 0), symbolSize2, symbolSize2);
        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(double), typeof(SignalSymbol), new PropertyMetadata((s, e) => ((SignalSymbol)s).PositionChangedCallback((double)e.NewValue)));


        readonly int animationDuration;
        readonly double symbolSize2;
        readonly bool symbolConstantAnimationSpeed;
        readonly IHasQuadraticBezierCurvePoints hasQuadraticBezierCurvePoints;
        DoubleAnimation doubleAnimation;


        public SignalSymbol(int animationDuration, SolidColorBrush symbolFill, double symbolSize, bool symbolConstantAnimationSpeed, IHasQuadraticBezierCurvePoints hasQuadraticBezierCurvePoints)
        {
            this.animationDuration = animationDuration;
            symbolSize2 = symbolSize / 2;
            this.symbolConstantAnimationSpeed = symbolConstantAnimationSpeed;
            this.hasQuadraticBezierCurvePoints = hasQuadraticBezierCurvePoints;
            Fill = symbolFill;
            Visibility = Visibility.Hidden;
        }

        private void PositionChangedCallback(double newValue)
        {
            Point p0 = hasQuadraticBezierCurvePoints.Start;
            Point p1 = hasQuadraticBezierCurvePoints.StartDirection;
            Point p2 = hasQuadraticBezierCurvePoints.EndDirection;
            Point p3 = hasQuadraticBezierCurvePoints.End;

            double x = (1 - newValue) * (1 - newValue) * (1 - newValue) * p0.X + 3 * (1 - newValue) * (1 - newValue) * newValue * p1.X + 3 * (1 - newValue) * newValue * newValue * p2.X + newValue * newValue * newValue * p3.X;
            double y = (1 - newValue) * (1 - newValue) * (1 - newValue) * p0.Y + 3 * (1 - newValue) * (1 - newValue) * newValue * p1.Y + 3 * (1 - newValue) * newValue * newValue * p2.Y + newValue * newValue * newValue * p3.Y;

            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);

            if (Visibility == Visibility.Hidden)
                Visibility = Visibility.Visible;
        }

        internal void BeginAnimation()
        {
            if(symbolConstantAnimationSpeed)
            {
                doubleAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(animationDuration));
                doubleAnimation.Completed += (s, e) => Finished?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Point p0 = hasQuadraticBezierCurvePoints.Start;
                Point p3 = hasQuadraticBezierCurvePoints.End;
                double length = Math.Sqrt(Math.Pow(p3.X - p0.X, 2) + Math.Pow(p3.Y - p0.Y, 2));

                doubleAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(animationDuration * (length / 500d)));
                doubleAnimation.Completed += (s, e) => Finished?.Invoke(this, EventArgs.Empty);
            }

            BeginAnimation(PositionProperty, doubleAnimation);
        }
    }
}