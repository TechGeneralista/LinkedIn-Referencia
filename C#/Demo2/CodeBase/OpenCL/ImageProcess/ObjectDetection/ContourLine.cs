using Common.NotifyProperty;
using System;
using System.Windows;


namespace ImageProcess.ObjectDetection
{
    public class ContourLine
    {
        public event Action<object> PointsExchanged;

        public ISettableObservableProperty<Point> StartPoint { get; } = new ObservableProperty<Point>();
        public ISettableObservableProperty<Point> EndPoint { get; } = new ObservableProperty<Point>();
        public INonSettableObservableProperty<double> Length { get; } = new ObservableProperty<double>();
        public INonSettableObservableProperty<Point> CenterPoint { get; } = new ObservableProperty<Point>();
        public ParallelSamplingPointCalculator ParallelSamplingPointCalculator { get; }


        public ContourLine(Point startPoint, Point endPoint, uint sampleQuantity, uint sampleDistance)
        {
            StartPoint.Value = startPoint;
            EndPoint.Value = endPoint;
            ParallelSamplingPointCalculator = new ParallelSamplingPointCalculator(sampleQuantity, sampleDistance);

            StartPoint.ValueChanged += (o, n) => RefreshProperties();
            EndPoint.ValueChanged += (o, n) => RefreshProperties();

            RefreshProperties();
        }

        public void ExchangePoints()
        {
            Point temp = StartPoint.Value;
            StartPoint.Value = EndPoint.Value;
            EndPoint.Value = temp;
            PointsExchanged?.Invoke(this);
        }

        private void RefreshProperties()
        {
            Length.ForceSet(Math.Sqrt(Math.Pow(EndPoint.Value.X - StartPoint.Value.X, 2) + Math.Pow(EndPoint.Value.Y - StartPoint.Value.Y, 2)));
            CenterPoint.ForceSet(new Point((StartPoint.Value.X + EndPoint.Value.X) / 2, (StartPoint.Value.Y + EndPoint.Value.Y) / 2));
            ParallelSamplingPointCalculator.Calculate(this);
        }
    }
}