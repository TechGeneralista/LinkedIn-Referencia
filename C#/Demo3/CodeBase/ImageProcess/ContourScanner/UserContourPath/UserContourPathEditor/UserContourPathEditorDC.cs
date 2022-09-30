using Common;
using Common.Language;
using Common.NotifyProperty;
using Common.Settings;
using ImageProcess.ReferenceImages;
using ImageProcess.Source;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace ImageProcess.ContourScanner.UserContourPath.UserContourPathEditor
{
    public class UserContourPathEditorDC : ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public IReadOnlyPropertyArray<UserLineDC> UserLines { get; } = new PropertyArray<UserLineDC>();
        public IProperty<UserLineDC> SelectedUserLine { get; } = new Property<UserLineDC>();
        public IProperty<uint> PointPairsQuantity { get; } = new Property<uint>(3);
        public IProperty<uint> PointPairsDistance { get; } = new Property<uint>(3);
        public UserContourPathDrawerDC UserContourPathDrawerDC { get; }
        public DateTime LastModified { get; private set; } = DateTime.Now;
        public UserContourPathDetector UserContourPathDetector { get; }


        public UserContourPathEditorDC(LanguageDC languageDC, ReferenceImagesDC referenceImagesDC, ImageProcessSourceDC imageProcessSourceDC, IReadOnlyProperty<ImageSource> resultImage)
        {
            LanguageDC = languageDC;

            SelectedUserLine.OnValueChanged += (o, n) => SelectedUserLine_OnValueChanged();
            PointPairsQuantity.OnValueChanged += (o, n) => PointPairsQuantity_OnValueChanged();
            PointPairsDistance.OnValueChanged += (o, n) => PointPairsDistance_OnValueChanged();

            UserContourPathDrawerDC = new UserContourPathDrawerDC(referenceImagesDC, this);
            UserContourPathDrawerDC.ResultImage.OnValueChanged += (o,n) => resultImage.ToSettable().Value = n;

            UserContourPathDetector = new UserContourPathDetector(this);
        }

        private void SelectedUserLine_OnValueChanged()
        {
            LastModified = DateTime.Now;
            UserContourPathDrawerDC.Draw();
        }

        public void PanZoomImageViewMouseDown(Point position, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Left)
            {
                LastModified = DateTime.Now;
                AddNewLine();
                SelectedUserLine.Value = UserLines.Value.Last();
                SelectedUserLine.Value.SetStartAndEndPositions(position);
            }

            else if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Right)
            {
                LastModified = DateTime.Now;

                foreach (UserLineDC userLineDC in UserLines.Value)
                    userLineDC.SelectPointToMove(position);

                SelectedUserLine.Value = null;
            }
        }

        private void AddNewLine()
        {
            UserLineDC userLineDC = new UserLineDC(LanguageDC, PointPairsQuantity.Value, PointPairsDistance.Value);
            userLineDC.Exchanged += UserLineDC_Exchanged;
            userLineDC.SelfRemoveRequest += UserLineDC_SelfRemoveRequest;
            UserLines.ToSettable().Add(userLineDC);
        }

        public void PanZoomImageViewMouseMove(Vector vector, Point position, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
            {
                SelectedUserLine.Value.SetEndPosition(vector);
                UserContourPathDrawerDC.Draw();
            }

            else if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed)
            {
                foreach (UserLineDC userLineDC in UserLines.Value)
                    userLineDC.MovePoint(vector);

                UserContourPathDrawerDC.Draw();
            }
        }

        public void PanZoomImageViewMouseUp(Point position, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Left)
            {
                SelectedUserLine.Value.SetEndPosition(position);
                UserContourPathDrawerDC.Draw();
                LastModified = DateTime.Now;
                UserContourPathDetector.Detect();
            }

            else if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Right)
            {
                foreach (UserLineDC userLineDC in UserLines.Value)
                    userLineDC.MovePoint(position);

                UserContourPathDrawerDC.Draw();
                LastModified = DateTime.Now;
                UserContourPathDetector.Detect();
            }
        }

        private void UserLineDC_Exchanged(UserLineDC obj)
        {
            SelectedUserLine.Value = null;
            UserContourPathDetector.Detect();
        }

        private void UserLineDC_SelfRemoveRequest(UserLineDC userLineDC)
        {
            UserLines.ToSettable().Remove(userLineDC);
            SelectedUserLine.Value = null;
            UserContourPathDetector.Detect();
        }

        public void RemoveAll()
        {
            if (UserLines.Value.Length == 0)
                return;

            if (Utils.ShowAreYouSureYouWantToDeleteThemAllQuestion(LanguageDC) == MessageBoxResult.No)
                return;

            UserLines.ToSettable().Clear();
            SelectedUserLine.Value = null;
            UserContourPathDetector.Detect();
        }

        private void PointPairsQuantity_OnValueChanged()
        {
            foreach (UserLineDC userLineDC in UserLines.Value)
                userLineDC.PointPairsQuantity.Value = PointPairsQuantity.Value;

            SelectedUserLine.Value = null;
            UserContourPathDetector.Detect();
        }

        private void PointPairsDistance_OnValueChanged()
        {
            foreach (UserLineDC userLineDC in UserLines.Value)
                userLineDC.PointPairsDistance.Value = PointPairsDistance.Value;

            SelectedUserLine.Value = null;
            UserContourPathDetector.Detect();
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            settingsCollection.SetProperty(PointPairsQuantity.Value, nameof(PointPairsQuantity));
            settingsCollection.SetProperty(PointPairsDistance.Value, nameof(PointPairsDistance));

            settingsCollection.AddKey(nameof(UserLines));
            int length = UserLines.Value.Length;
            settingsCollection.SetProperty(length, nameof(UserLines.Value.Length));

            for(int index = 0; index < length; index++)
            {
                settingsCollection.AddKey(index.ToString());
                UserLines.Value[index].SaveSettings(settingsCollection);
                settingsCollection.RemoveLastKey();
            }

            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            PointPairsQuantity.Value = settingsCollection.GetProperty<uint>(nameof(PointPairsQuantity));
            PointPairsDistance.Value = settingsCollection.GetProperty<uint>(nameof(PointPairsDistance));

            settingsCollection.AddKey(nameof(UserLines));
            int length = settingsCollection.GetProperty<int>(nameof(UserLines.Value.Length));

            for (int index = 0; index < length; index++)
            {
                settingsCollection.AddKey(index.ToString());
                AddNewLine();
                UserLines.Value[index].LoadSettings(settingsCollection);
                settingsCollection.RemoveLastKey();
            }

            UserContourPathDetector.Detect();
            settingsCollection.ExitPoint();
        }
    }
}
