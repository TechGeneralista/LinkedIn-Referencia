using Common;
using Common.Language;
using Common.NotifyProperty;
using Common.Settings;
using System;
using System.Collections.Generic;
using System.Windows;


namespace ImageProcess.ContourScanner.UserContourPath.UserContourPathEditor
{
    public class UserLineDC : ICanSaveLoadSettings
    {
        public event Action<UserLineDC> Exchanged, SelfRemoveRequest;

        public LanguageDC LanguageDC { get; }
        public IReadOnlyProperty<Point> StartPoint { get; } = new Property<Point>();
        public IReadOnlyProperty<Point> EndPoint { get; } = new Property<Point>();
        public IReadOnlyProperty<double> Length { get; } = new Property<double>();
        public IProperty<uint> PointPairsQuantity { get; } = new Property<uint>(3);
        public IProperty<uint> PointPairsDistance { get; } = new Property<uint>(3);
        public IReadOnlyPropertyArray<SamplingPointPair> SamplingPointPairs { get; } = new PropertyArray<SamplingPointPair>();


        IReadOnlyProperty<Point> pointToMove;


        public UserLineDC(LanguageDC languageDC, uint pointPairsQuantity, uint pointPairsDistance)
        {
            LanguageDC = languageDC;

            StartPoint.OnValueChanged += (o, n) => ReCalculate();
            EndPoint.OnValueChanged += (o, n) => ReCalculate();
            PointPairsQuantity.OnValueChanged += (o, n) => ReCalculate();
            PointPairsDistance.OnValueChanged += (o, n) => ReCalculate();
        }

        private void ReCalculate()
        {
            Length.ToSettable().Value = Math.Sqrt(Math.Pow(EndPoint.Value.X - StartPoint.Value.X, 2) + Math.Pow(EndPoint.Value.Y - StartPoint.Value.Y, 2));

            double quantity = Length.Value / PointPairsQuantity.Value;

            if (quantity == 0)
            {
                SamplingPointPairs.ToSettable().Clear();
                return;
            }

            if (quantity < 1)
                quantity = 1;

            Point offsettedToOrigin = Point.Subtract(EndPoint.Value, (Vector)StartPoint.Value);

            double rotationDeg = Utils.Atan2(offsettedToOrigin);

            double lengthHalf = (double)PointPairsDistance.Value / 2;
            SamplingPointPair pointPair = new SamplingPointPair(new Point(lengthHalf, 0), new Point(lengthHalf * (-1), 0));

            double dist = Length.Value / (quantity + (double)1);

            List<SamplingPointPair> samplePointPairs = new List<SamplingPointPair>();

            for (uint i = 1; i <= quantity; i += 1)
                samplePointPairs.Add(new SamplingPointPair(new Point(pointPair.Brighter.Y + (i * dist), pointPair.Brighter.X), new Point(pointPair.Darker.Y + (i * dist), pointPair.Darker.X)));

            foreach (SamplingPointPair samplingPointPair in samplePointPairs)
            {
                samplingPointPair.Rotate(rotationDeg);
                samplingPointPair.Add(StartPoint.Value);
            }

            SamplingPointPairs.ToSettable().ReAddRange(samplePointPairs.ToArray());
        }

        internal void SetStartAndEndPositions(Point position)
        {
            StartPoint.ToSettable().DisableNextOnValueChangeEvents();
            StartPoint.ToSettable().Value = position;
            EndPoint.ToSettable().Value = position;
        }

        internal void SetEndPosition(Vector vector) => EndPoint.ToSettable().Value = EndPoint.Value + vector;
        internal void SetEndPosition(Point position) => EndPoint.ToSettable().Value = position;

        internal void SelectPointToMove(Point position)
        {
            if (StartPoint.Value == position)
                pointToMove = StartPoint;

            else if (EndPoint.Value == position)
                pointToMove = EndPoint;

            else
                pointToMove = null;
        }

        internal void MovePoint(Vector vector)
        {
            if (pointToMove.IsNotNull())
                pointToMove.ToSettable().Value = pointToMove.Value + vector;
        }

        internal void MovePoint(Point position)
        {
            if (pointToMove.IsNotNull())
                pointToMove.ToSettable().Value = position;
        }

        public void Exchange()
        {
            StartPoint.ToSettable().DisableNextOnValueChangeEvents();

            Point temp = StartPoint.Value;
            StartPoint.ToSettable().Value = EndPoint.Value;
            EndPoint.ToSettable().Value = temp;

            Exchanged?.Invoke(this);
        }

        internal void SelfRemove() => SelfRemoveRequest?.Invoke(this);

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);

            settingsCollection.SetProperty(StartPoint.Value.X , nameof(StartPoint),nameof(StartPoint.Value.X));
            settingsCollection.SetProperty(StartPoint.Value.Y, nameof(StartPoint),nameof(StartPoint.Value.Y));

            settingsCollection.SetProperty(EndPoint.Value.X, nameof(EndPoint),nameof(EndPoint.Value.X));
            settingsCollection.SetProperty(EndPoint.Value.Y, nameof(EndPoint),nameof(EndPoint.Value.Y));

            settingsCollection.SetProperty(PointPairsQuantity.Value, nameof(PointPairsQuantity));
            settingsCollection.SetProperty(PointPairsDistance.Value, nameof(PointPairsDistance));

            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);

            double startX =  settingsCollection.GetProperty<double>(nameof(StartPoint),nameof(StartPoint.Value.X));
            double startY = settingsCollection.GetProperty<double>(nameof(StartPoint),nameof(StartPoint.Value.Y));
            double endX = settingsCollection.GetProperty<double>(nameof(EndPoint),nameof(EndPoint.Value.X));
            double endY = settingsCollection.GetProperty<double>(nameof(EndPoint),nameof(EndPoint.Value.Y));

            StartPoint.ToSettable().DisableNextOnValueChangeEvents();
            StartPoint.ToSettable().Value = new Point(startX, startY);

            EndPoint.ToSettable().DisableNextOnValueChangeEvents();
            EndPoint.ToSettable().Value = new Point(endX, endY);

            PointPairsQuantity.DisableNextOnValueChangeEvents();
            PointPairsQuantity.Value = settingsCollection.GetProperty<uint>(nameof(PointPairsQuantity));
            PointPairsDistance.DisableNextOnValueChangeEvents();
            PointPairsDistance.Value = settingsCollection.GetProperty<uint>(nameof(PointPairsDistance));

            ReCalculate();
            settingsCollection.ExitPoint();
        }
    }
}
