using Common;
using Common.NotifyProperty;
using ImageProcess.ObjectDetection;
using System;
using System.Windows;
using System.Windows.Input;


namespace UniCamV2.Content.Model
{
    public class UserContourSetup
    {
        public event Action<object> ContourLinesAdded;


        public ISettableObservableProperty<bool> IsCaptureContourLines { get; } = new ObservableProperty<bool>();
        public INonSettableObservableProperty<ContourLine[]> Lines { get; } = new ObservableProperty<ContourLine[]>();
        public ISettableObservableProperty<ContourLine> SelectedLine { get; } = new ObservableProperty<ContourLine>();
        public ISettableObservableProperty<uint> SamplePointsQuantity { get; } = new ObservableProperty<uint>(3);
        public ISettableObservableProperty<uint> SamplePointsDistance { get; } = new ObservableProperty<uint>(3);
        public UserContour UserContour { get; }
        public ISettableObservableProperty<uint> RotateTolerance { get; } = new ObservableProperty<uint>(30);


        readonly ReferenceImages referenceImages;
        readonly UserScreen userScreen;


        public UserContourSetup(ReferenceImages referenceImages, UserScreen userScreen)
        {
            this.referenceImages = referenceImages;
            this.userScreen = userScreen;

            ClearLines();

            SelectedLine.ValueChanged += (o, n) => userScreen.ObjectContourSetup(referenceImages.Monochrome.Value, Lines.Value, n);
            IsCaptureContourLines.ValueChanged += IsCaptureContourLines_ValueChanged;

            SamplePointsQuantity.ValueChanged += SampleQuantity_ValueChanged;
            SamplePointsDistance.ValueChanged += SamplePointsDistance_ValueChanged;

            UserContour = new UserContour();
        }

        private void SamplePointsDistance_ValueChanged(uint oldValue, uint newValue)
        {
            foreach (ContourLine cl in Lines.Value)
                cl.ParallelSamplingPointCalculator.Distance.Value = newValue;

            userScreen.ObjectContourSetup(referenceImages.Monochrome.Value, Lines.Value, SelectedLine.Value);
        }

        private void SampleQuantity_ValueChanged(uint oldValue, uint newValue)
        {
            foreach (ContourLine cl in Lines.Value)
                cl.ParallelSamplingPointCalculator.Quantity.Value = newValue;

            userScreen.ObjectContourSetup(referenceImages.Monochrome.Value, Lines.Value, SelectedLine.Value);
        }

        private void IsCaptureContourLines_ValueChanged(bool oldValue, bool newValue)
        {
            if(newValue)
                userScreen.ObjectContourSetup(referenceImages.Monochrome.Value, Lines.Value, SelectedLine.Value);
            else
            {
                SelectedLine.Value = null;

                UserContour.Refresh(Lines.Value, RotateTolerance.Value);
                ContourLinesAdded?.Invoke(this);
            }
        }

        public void ClearLines()
        {
            Lines.ForceSet(new ContourLine[0]);
            SelectedLine.Value = null;
        }

        public void CaptureContourLinesButton() => IsCaptureContourLines.Value = !IsCaptureContourLines.Value;

        public void RemoveSelectedLine()
        {
            if(SelectedLine.Value.IsNotNull())
            {
                Lines.ForceSet(Lines.Value.Remove(SelectedLine.Value));
                SelectedLine.Value = null;
            }
        }

        internal void MainDisplayMouseDown(object sender, Point position, MouseButtonEventArgs e)
        {
            if (!IsCaptureContourLines.Value)
                return;

            ContourLine newContourLine = new ContourLine(position, position, SamplePointsQuantity.Value, SamplePointsDistance.Value);
            Lines.ForceSet(Lines.Value.Add(newContourLine));
            SelectedLine.Value = newContourLine;
            newContourLine.PointsExchanged += (cl) => SelectedLine.Value = (ContourLine)cl;
        }

        internal void MainDisplayMouseMove(object sender, Point position, Vector moveVector, MouseEventArgs e)
        {
            if (!IsCaptureContourLines.Value || SelectedLine.Value.IsNull())
                return;

            SelectedLine.Value.EndPoint.Value = position;
            userScreen.ObjectContourSetup(referenceImages.Monochrome.Value, Lines.Value, SelectedLine.Value);
        }

        internal void MainDisplayMouseUp(object sender, Point position, MouseButtonEventArgs e)
        {
            if (!IsCaptureContourLines.Value)
                return;

            SelectedLine.Value.EndPoint.Value = position;

            if (SelectedLine.Value.Length.Value < 2)
                Lines.ForceSet(Lines.Value.Remove(SelectedLine.Value));

            SelectedLine.Value = null;
            userScreen.ObjectContourSetup(referenceImages.Monochrome.Value, Lines.Value, SelectedLine.Value);
        }

        internal void RefreshReferenceImages()
        {
            referenceImages.Refresh();
            userScreen.ObjectContourSetup(referenceImages.Monochrome.Value, Lines.Value, SelectedLine.Value);
        }
    }
}