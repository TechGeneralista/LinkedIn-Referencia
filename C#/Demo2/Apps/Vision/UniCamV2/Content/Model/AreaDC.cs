using Common;
using Common.NotifyProperty;
using ImageProcess.ObjectDetection;
using OpenCLWrapper;
using System;
using System.Windows;
using System.Windows.Input;
using UniCamV2.Tools;


namespace UniCamV2.Content.Model
{
    public class AreaDC
    {
        public ISettableObservableProperty<string> Name { get; } = new ObservableProperty<string>(Utils.NewGuid(8));
        public INonSettableObservableProperty<bool> Result { get; } = new ObservableProperty<bool>();
        public ReferenceImages ReferenceImages { get; }
        public UserContourSetup UserContourSetup { get; }


        readonly ImagePreparator imagePreparator;
        readonly UserScreen userScreen;
        readonly ContourFinder contourFinder;


        public AreaDC(ImagePreparator imagePreparator, UserScreen userScreen, OpenCLAccelerator openCLAccelerator)
        {
            this.imagePreparator = imagePreparator;
            this.userScreen = userScreen;
            Name.ValueChanged += CheckName;

            ReferenceImages = new ReferenceImages(imagePreparator);
            UserContourSetup = new UserContourSetup(ReferenceImages, userScreen);

            UserContourSetup.ContourLinesAdded += ObjectContourSetup_ContourLinesAdded;
            contourFinder = new ContourFinder(openCLAccelerator);
        }

        private void CheckName(string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(newValue) || string.IsNullOrWhiteSpace(newValue))
                Name.Value = oldValue;
        }

        internal void MainDisplayMouseDown(object sender, Point position, MouseButtonEventArgs e)
        {
            UserContourSetup.MainDisplayMouseDown(sender, position, e);
        }

        internal void MainDisplayMouseMove(object sender, Point position, Vector moveVector, MouseEventArgs e)
        {
            UserContourSetup.MainDisplayMouseMove(sender, position, moveVector, e);
        }

        internal void MainDisplayMouseUp(object sender, Point position, MouseButtonEventArgs e)
        {
            UserContourSetup.MainDisplayMouseUp(sender, position, e);
        }

        private void ObjectContourSetup_ContourLinesAdded(object sender)
        {
            if (UserContourSetup.UserContour.TransformedPointPairs.IsNull() || UserContourSetup.UserContour.TransformedPointPairs.Length == 0)
                return;

            contourFinder.Prepare(imagePreparator.MonochromeImageBuffer,  UserContourSetup.UserContour);
            userScreen.DrawDetectedObjects(ReferenceImages.Monochrome.Value, contourFinder);
        }

        internal void ShowColorReferenceImage() => userScreen.Display.Value = ReferenceImages.Color.Value;
        internal void ShowMonochromeReferenceImage() => userScreen.Display.Value = ReferenceImages.Monochrome.Value;

        internal void Selected()
        {
            
        }

        internal void ShowAreaProperties()
        {
            
        }
    }
}
