using Common.NotifyProperty;
using Common.Settings;
using CustomControl.ImageViewControl;
using ImageProcess.ReferenceImages;
using Language;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace ImageProcess.ContourFinder.UserContourPath
{
    public class UserContourPathDC : ICanSaveLoadSettings, ICanHandleImageViewControlMouseEvents
    {
        public LanguageDC LanguageDC { get; }
        public INonSettableObservablePropertyArray<UserLineDC> UserLines { get; } = new ObservablePropertyArray<UserLineDC>();
        public ISettableObservableProperty<UserLineDC> SelectedUserLine { get; } = new ObservableProperty<UserLineDC>();
        public ISettableObservableProperty<uint> PointPairsQuantity { get; } = new ObservableProperty<uint>(3);
        public ISettableObservableProperty<uint> PointPairsDistance { get; } = new ObservableProperty<uint>(3);
        public ISettableObservableProperty<int> RotateToleranceMinus { get; } = new ObservableProperty<int>(0);
        public ISettableObservableProperty<int> RotateTolerancePlus { get; } = new ObservableProperty<int>(0);
        public Detector Detector { get; }
        public RotatedPointPairsCreator RotatedPointPairsGroup { get; }
        public bool Editing => (DateTime.Now - lastEditedDateTime).TotalSeconds <= editingTimeout || editing;


        readonly int editingTimeout = 3;
        bool drawRotation;
        bool editing;
        DateTime lastEditedDateTime;
        readonly UserContourPathDrawer userContourPathDrawer;
        readonly ReferenceImagesDC referenceImagesDC;
        readonly PointPairsGroupCreator originalPointPairsGroup;


        public UserContourPathDC(LanguageDC languageDC, ReferenceImagesDC referenceImagesDC, ISettableObservableProperty<ImageSource> mainDisplaySource)
        {
            LanguageDC = languageDC;
            this.referenceImagesDC = referenceImagesDC;
            lastEditedDateTime = DateTime.Now.AddSeconds(-editingTimeout);

            userContourPathDrawer = new UserContourPathDrawer(mainDisplaySource);

            originalPointPairsGroup = new PointPairsGroupCreator();
            Detector = new Detector();

            SelectedUserLine.CurrentValueChanged += (o, n) => SelectedUserLine_CurrentValueChanged();

            RotateToleranceMinus.CurrentValueChanged += (o, n) => RotateToleranceMinus_CurrentValueChanged();
            RotateTolerancePlus.CurrentValueChanged += (o, n) => RotateTolerancePlus_CurrentValueChanged();

            PointPairsQuantity.CurrentValueChanged += (o, n) => PointPairsQuantity_CurrentValueChanged();
            PointPairsDistance.CurrentValueChanged += (o, n) => PointPairsDistance_CurrentValueChanged();

            RotatedPointPairsGroup = new RotatedPointPairsCreator();
        }

        private void SelectedUserLine_CurrentValueChanged()
        {
            lastEditedDateTime = DateTime.Now;

            if (drawRotation)
            {
                userContourPathDrawer.DrawLinesAndAxes(referenceImagesDC, UserLines.CurrentValue, Detector.Center, Detector.Size, RotateToleranceMinus.CurrentValue, RotateTolerancePlus.CurrentValue);
                drawRotation = false;
            }
            else
                userContourPathDrawer.DrawLinesAndPointPairs(referenceImagesDC, UserLines.CurrentValue, SelectedUserLine.CurrentValue);
        }

        private void PointPairsQuantity_CurrentValueChanged()
        {
            foreach (UserLineDC userLineDC in UserLines.CurrentValue)
                userLineDC.PointPairsQuantity.CurrentValue = PointPairsQuantity.CurrentValue;

            SelectedUserLine.CurrentValue = null;
            CreateGroupAndDetect();
        }

        private void PointPairsDistance_CurrentValueChanged()
        {
            foreach (UserLineDC userLineDC in UserLines.CurrentValue)
                userLineDC.PointPairsDistance.CurrentValue = PointPairsDistance.CurrentValue;

            SelectedUserLine.CurrentValue = null;
            CreateGroupAndDetect();
        }

        public void ImageViewControlMouseDown(Point position, MouseButtonEventArgs e)
        {
            editing = true;

            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Left)
            {
                AddNewLine();
                SelectedUserLine.CurrentValue.SetStartAndEndPositions(position);
            }

            else if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Right)
            {
                foreach (UserLineDC userLineDC in UserLines.CurrentValue)
                    userLineDC.SelectPointToMove(position);

                SelectedUserLine.CurrentValue = null;
            }
        }

        private void AddNewLine()
        {
            UserLineDC userLineDC = new UserLineDC(LanguageDC, PointPairsQuantity.CurrentValue, PointPairsDistance.CurrentValue);
            userLineDC.Exchanged += UserLineExchanged;
            userLineDC.SelfRemoveRequest += UserLineNeedSelfRemove;
            UserLines.ForceAdd(userLineDC);
            SelectedUserLine.CurrentValue = userLineDC;
        }

        public void ImageViewControlMouseMove(Vector vector, Point position, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
            {
                SelectedUserLine.CurrentValue.SetEndPosition(vector);
                SelectedUserLine.CurrentValue = SelectedUserLine.CurrentValue;
            }

            else if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed)
            {
                foreach (UserLineDC userLineDC in UserLines.CurrentValue)
                    userLineDC.MovePoint(vector);

                SelectedUserLine.CurrentValue = null;
            }
        }

        public void ImageViewControlMouseUp(Point position, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Left)
            {
                SelectedUserLine.CurrentValue.SetEndPosition(position);
                SelectedUserLine.CurrentValue = SelectedUserLine.CurrentValue;
                CreateGroupAndDetect();
            }

            else if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Right)
            {
                foreach (UserLineDC userLineDC in UserLines.CurrentValue)
                    userLineDC.MovePoint(position);

                SelectedUserLine.CurrentValue = null;
            }

            editing = false;
        }

        private void UserLineNeedSelfRemove(UserLineDC userLineDC)
        {
            UserLines.ForceRemove(userLineDC);
            SelectedUserLine.CurrentValue = null;
            CreateGroupAndDetect();
        }

        private void UserLineExchanged(UserLineDC userLineDC)
        {
            SelectedUserLine.CurrentValue = userLineDC;
            CreateGroupAndDetect();
        }

        public void RemoveAll()
        {
            UserLines.ForceClear();
            SelectedUserLine.CurrentValue = null;
            CreateGroupAndDetect();
        }

        private void RotateTolerancePlus_CurrentValueChanged()
        {
            if (RotateToleranceMinus.CurrentValue > RotateTolerancePlus.CurrentValue)
                RotateToleranceMinus.CurrentValue = RotateTolerancePlus.CurrentValue;

            drawRotation = true;
            SelectedUserLine.CurrentValue = null;
            CreateGroupAndDetect();
        }

        private void RotateToleranceMinus_CurrentValueChanged()
        {
            if (RotateToleranceMinus.CurrentValue > RotateTolerancePlus.CurrentValue)
                RotateTolerancePlus.CurrentValue = RotateToleranceMinus.CurrentValue;

            drawRotation = true;
            SelectedUserLine.CurrentValue = null;
            CreateGroupAndDetect();
        }

        private void CreateGroupAndDetect()
        {
            originalPointPairsGroup.Create(UserLines.CurrentValue);
            Detector.Detect(originalPointPairsGroup.PointPairs);
            RotatedPointPairsGroup.Create(Detector.OriginPointPairs, RotateToleranceMinus.CurrentValue, RotateTolerancePlus.CurrentValue);
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(UserContourPathDC));
            settingsCollection.KeyCreator.AddNew(nameof(UserLines));
            settingsCollection.KeyCreator.AddNew(nameof(UserLines.CurrentValue.Length));
            settingsCollection.SetValue(UserLines.CurrentValue.Length);

            for(int i=0;i< UserLines.CurrentValue.Length;i++)
            {
                settingsCollection.KeyCreator.ReplaceLast(i.ToString());
                UserLines.CurrentValue[i].SaveSettings(settingsCollection);
            }

            settingsCollection.KeyCreator.RemoveLast();

            settingsCollection.KeyCreator.ReplaceLast(nameof(PointPairsQuantity));
            settingsCollection.SetValue(PointPairsQuantity.CurrentValue);
            settingsCollection.KeyCreator.ReplaceLast(nameof(PointPairsDistance));
            settingsCollection.SetValue(PointPairsDistance.CurrentValue);
            settingsCollection.KeyCreator.ReplaceLast(nameof(RotateToleranceMinus));
            settingsCollection.SetValue(RotateToleranceMinus.CurrentValue);
            settingsCollection.KeyCreator.ReplaceLast(nameof(RotateTolerancePlus));
            settingsCollection.SetValue(RotateTolerancePlus.CurrentValue);

            settingsCollection.KeyCreator.RemoveLast(2);
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(UserContourPathDC));
            settingsCollection.KeyCreator.AddNew(nameof(UserLines));
            settingsCollection.KeyCreator.AddNew(nameof(UserLines.CurrentValue.Length));
            int length = settingsCollection.GetValue<int>();

            SelectedUserLine.NextCurrentValueChangedDisabled = true;
            PointPairsQuantity.NextCurrentValueChangedDisabled = true;
            PointPairsDistance.NextCurrentValueChangedDisabled = true;
            RotateToleranceMinus.NextCurrentValueChangedDisabled = true;
            RotateTolerancePlus.NextCurrentValueChangedDisabled = true;

            for (int i = 0; i < length; i++)
            {
                AddNewLine();
                settingsCollection.KeyCreator.ReplaceLast(i.ToString());
                UserLines.CurrentValue[i].LoadSettings(settingsCollection);
            }

            settingsCollection.KeyCreator.RemoveLast();

            settingsCollection.KeyCreator.ReplaceLast(nameof(PointPairsQuantity));
            PointPairsQuantity.CurrentValue = settingsCollection.GetValue<uint>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(PointPairsDistance));
            PointPairsDistance.CurrentValue = settingsCollection.GetValue<uint>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(RotateToleranceMinus));
            RotateToleranceMinus.CurrentValue = settingsCollection.GetValue<int>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(RotateTolerancePlus));
            RotateTolerancePlus.CurrentValue = settingsCollection.GetValue<int>();
            
            settingsCollection.KeyCreator.RemoveLast(2);

            SelectedUserLine.NextCurrentValueChangedDisabled = false;
            PointPairsQuantity.NextCurrentValueChangedDisabled = false;
            PointPairsDistance.NextCurrentValueChangedDisabled = false;
            RotateToleranceMinus.NextCurrentValueChangedDisabled = false;
            RotateTolerancePlus.NextCurrentValueChangedDisabled = false;

            CreateGroupAndDetect();
        }
    }
}
