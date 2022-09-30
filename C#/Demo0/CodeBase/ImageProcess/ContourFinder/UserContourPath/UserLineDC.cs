using Common;
using Common.NotifyProperty;
using Common.Settings;
using Language;
using System;
using System.Collections.Generic;
using System.Windows;


namespace ImageProcess.ContourFinder.UserContourPath
{
    public class UserLineDC : ICanSaveLoadSettings
    {
        public event Action<UserLineDC> Exchanged, SelfRemoveRequest;

        public LanguageDC LanguageDC { get; }
        public INonSettableObservableProperty<Point> StartPoint { get; } = new ObservableProperty<Point>();
        public INonSettableObservableProperty<Point> EndPoint { get; } = new ObservableProperty<Point>();
        public INonSettableObservableProperty<double> Length { get; } = new ObservableProperty<double>();
        public ISettableObservableProperty<uint> PointPairsQuantity { get; } = new ObservableProperty<uint>(3);
        public ISettableObservableProperty<uint> PointPairsDistance { get; } = new ObservableProperty<uint>(3);
        public PointPair[] PointPairs { get; private set; } = new PointPair[0];


        INonSettableObservableProperty<Point> pointToMove;


        public UserLineDC(LanguageDC languageDC, uint pointPairsQuantity, uint pointPairsDistance)
        {
            LanguageDC = languageDC;

            PointPairsQuantity.CurrentValue = pointPairsQuantity;
            PointPairsDistance.CurrentValue = pointPairsDistance;

            StartPoint.CurrentValueChanged += (o, n) => Refresh();
            EndPoint.CurrentValueChanged += (o, n) => Refresh();
            PointPairsQuantity.CurrentValueChanged += (o, n) => Refresh();
            PointPairsDistance.CurrentValueChanged += (o, n) => Refresh();
        }

        internal void SetStartAndEndPositions(Point position)
        {
            StartPoint.NextCurrentValueChangedDisabled = true;
            EndPoint.NextCurrentValueChangedDisabled = true;

            StartPoint.ForceSet(position);
            EndPoint.ForceSet(position);

            StartPoint.NextCurrentValueChangedDisabled = false;
            EndPoint.NextCurrentValueChangedDisabled = false;

            Refresh();
        }

        internal void SetEndPosition(Vector vector) => EndPoint.ForceSet(EndPoint.CurrentValue + vector);
        internal void SetEndPosition(Point position) => EndPoint.ForceSet(position);

        internal void SelectPointToMove(Point position)
        {
            if (StartPoint.CurrentValue == position)
                pointToMove = StartPoint;

            else if (EndPoint.CurrentValue == position)
                pointToMove = EndPoint;

            else
                pointToMove = null;
        }

        internal void MovePoint(Vector vector)
        {
            if (pointToMove.IsNotNull())
                pointToMove.ForceSet(pointToMove.CurrentValue + vector);
        }

        internal void MovePoint(Point position)
        {
            if (pointToMove.IsNotNull())
                pointToMove.ForceSet(position);
        }

        private void Refresh()
        {
            Length.ForceSet(Math.Sqrt(Math.Pow(EndPoint.CurrentValue.X - StartPoint.CurrentValue.X, 2) + Math.Pow(EndPoint.CurrentValue.Y - StartPoint.CurrentValue.Y, 2)));
            CalculatePointPairs();
        }

        private void CalculatePointPairs()
        {
            double quantity = Length.CurrentValue / PointPairsQuantity.CurrentValue;

            if (quantity == 0)
            {
                PointPairs = new PointPair[0];
                return;
            }

            if (quantity < 1)
                quantity = 1;

            Point endPointOffsettedToOrigin = Point.Subtract(EndPoint.CurrentValue, (Vector)StartPoint.CurrentValue);

            double originalRotationDeg = Utils.Atan2(endPointOffsettedToOrigin);

            double lengthHalf = (double)PointPairsDistance.CurrentValue / 2;
            PointPair pointPair = new PointPair(new Point(lengthHalf, 0), new Point(lengthHalf * (-1), 0));

            double dist = Length.CurrentValue / (quantity + (double)1);

            List<PointPair> samplePointPairs = new List<PointPair>();
            for (uint i = 1; i <= quantity; i += 1)
                samplePointPairs.Add(new PointPair(new Point(pointPair.Brighter.Y + (i * dist), pointPair.Brighter.X), new Point(pointPair.Darker.Y + (i * dist), pointPair.Darker.X)));

            PointPairs = samplePointPairs.ToArray();

            foreach (PointPair pp in PointPairs)
            {
                pp.Rotate(originalRotationDeg);
                pp.Add(StartPoint.CurrentValue);
            }
        }

        public void Exchange()
        {
            StartPoint.NextCurrentValueChangedDisabled = true;
            EndPoint.NextCurrentValueChangedDisabled = true;

            Point temp = StartPoint.CurrentValue;
            StartPoint.ForceSet(EndPoint.CurrentValue);
            EndPoint.ForceSet(temp);

            StartPoint.NextCurrentValueChangedDisabled = false;
            EndPoint.NextCurrentValueChangedDisabled = false;

            Refresh();
            Exchanged?.Invoke(this);
        }

        internal void SelfRemove() => SelfRemoveRequest?.Invoke(this);

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(UserLineDC));

            settingsCollection.KeyCreator.AddNew(nameof(StartPoint));
            settingsCollection.KeyCreator.AddNew(nameof(StartPoint.CurrentValue.X));
            settingsCollection.SetValue(StartPoint.CurrentValue.X);
            settingsCollection.KeyCreator.ReplaceLast(nameof(StartPoint.CurrentValue.Y));
            settingsCollection.SetValue(StartPoint.CurrentValue.Y);
            settingsCollection.KeyCreator.RemoveLast();

            settingsCollection.KeyCreator.ReplaceLast(nameof(EndPoint));
            settingsCollection.KeyCreator.AddNew(nameof(EndPoint.CurrentValue.X));
            settingsCollection.SetValue(EndPoint.CurrentValue.X);
            settingsCollection.KeyCreator.ReplaceLast(nameof(EndPoint.CurrentValue.Y));
            settingsCollection.SetValue(EndPoint.CurrentValue.Y);
            settingsCollection.KeyCreator.RemoveLast();

            settingsCollection.KeyCreator.ReplaceLast(nameof(PointPairsQuantity));
            settingsCollection.SetValue(PointPairsQuantity.CurrentValue);

            settingsCollection.KeyCreator.ReplaceLast(nameof(PointPairsDistance));
            settingsCollection.SetValue(PointPairsDistance.CurrentValue);

            settingsCollection.KeyCreator.RemoveLast(2);
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(UserLineDC));

            settingsCollection.KeyCreator.AddNew(nameof(StartPoint));
            settingsCollection.KeyCreator.AddNew(nameof(StartPoint.CurrentValue.X));
            double startPointX = settingsCollection.GetValue<double>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(StartPoint.CurrentValue.Y));
            double startPointY = settingsCollection.GetValue<double>();
            settingsCollection.KeyCreator.RemoveLast();

            settingsCollection.KeyCreator.ReplaceLast(nameof(EndPoint));
            settingsCollection.KeyCreator.AddNew(nameof(EndPoint.CurrentValue.X));
            double endPointX = settingsCollection.GetValue<double>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(EndPoint.CurrentValue.Y));
            double endPointY = settingsCollection.GetValue<double>();
            settingsCollection.KeyCreator.RemoveLast();

            settingsCollection.KeyCreator.ReplaceLast(nameof(PointPairsQuantity));
            uint pointPairsQuantity = settingsCollection.GetValue<uint>();

            settingsCollection.KeyCreator.ReplaceLast(nameof(PointPairsDistance));
            uint pointPairsDistance = settingsCollection.GetValue<uint>();

            settingsCollection.KeyCreator.RemoveLast(2);

            StartPoint.NextCurrentValueChangedDisabled = true;
            EndPoint.NextCurrentValueChangedDisabled = true;
            PointPairsQuantity.NextCurrentValueChangedDisabled = true;
            PointPairsDistance.NextCurrentValueChangedDisabled = true;

            StartPoint.ForceSet(new Point(startPointX, startPointY));
            EndPoint.ForceSet(new Point(endPointX, endPointY));
            PointPairsQuantity.CurrentValue = pointPairsQuantity;
            PointPairsDistance.CurrentValue = pointPairsDistance;

            StartPoint.NextCurrentValueChangedDisabled = false;
            EndPoint.NextCurrentValueChangedDisabled = false;
            PointPairsQuantity.NextCurrentValueChangedDisabled = false;
            PointPairsDistance.NextCurrentValueChangedDisabled = false;

            Refresh();
        }
    }
}
