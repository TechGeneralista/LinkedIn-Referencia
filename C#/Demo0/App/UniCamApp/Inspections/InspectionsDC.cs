using Common;
using Common.NotifyProperty;
using Language;
using OpenCLWrapper;
using System.Windows.Media;
using ImageProcess.Source;
using Common.Settings;
using Common.Interface;
using System.Windows;
using System.Windows.Input;
using CustomControl.ImageViewControl;
using AppLog;
using UniCamApp.Inspection;
using System;


namespace UniCamApp.Inspections
{
    public class InspectionsDC
    {
        public LanguageDC LanguageDC { get; }
        public INonSettableObservablePropertyArray<object> Inspections { get; } = new ObservablePropertyArray<object>();
        public ISettableObservableProperty<object> SelectedInspection { get; } = new ObservableProperty<object>();


        readonly OpenCLAccelerator openCLAccelerator;
        IImageProcessSource imageProcessSource;
        readonly ISettableObservableProperty<ImageSource> mainDisplaySource;
        readonly ISettableObservableProperty<object> currentContent;
        readonly ILog log;
        readonly ICanResetContent canResetContent;


        public InspectionsDC(LanguageDC languageDC, OpenCLAccelerator openCLAccelerator, IImageProcessSource imageProcessSource, ISettableObservableProperty<ImageSource> mainDisplaySource, ISettableObservableProperty<object> currentContent, ILog log, ICanResetContent canResetContent)
        {
            LanguageDC = languageDC;
            this.openCLAccelerator = openCLAccelerator;
            this.imageProcessSource = imageProcessSource;
            this.mainDisplaySource = mainDisplaySource;
            this.currentContent = currentContent;
            this.log = log;
            this.canResetContent = canResetContent;

            SelectedInspection.CurrentValueChanged += (o, n) => (n as ICanShowResultImage)?.ShowResultImage();
        }

        internal void AddNewInspectionDC()
        {
            InspectionDC inspectionDC = new InspectionDC(LanguageDC, openCLAccelerator, imageProcessSource, mainDisplaySource, log, canResetContent);
            Inspections.ForceAdd(inspectionDC);
            SelectedInspection.CurrentValue = inspectionDC;
        }

        internal void RemoveSelectedInspectionDC()
        {
            Inspections.ForceRemove(SelectedInspection.CurrentValue);
            SelectedInspection.CurrentValue = null;
        }

        internal void RemoveAllInspections()
        {
            Inspections.ForceClear();
            SelectedInspection.CurrentValue = null;
        }

        internal void ShowComponentProperties()
        {
            currentContent.CurrentValue = SelectedInspection.CurrentValue;
            (SelectedInspection.CurrentValue as ICanShowResultImage)?.ShowResultImage();
        }

        public void Process()
        {
            foreach (ICanProcessImageProcessSource canProcessImageProcessSource in Inspections.CurrentValue)
                canProcessImageProcessSource.Process(imageProcessSource);
        }

        internal void ResetContent()
        {
            foreach (ICanResetContent canResetContent in Inspections.CurrentValue)
                canResetContent.ResetContent();
        }

        public void ShowResultImage()
        {
            ICanShowResultImage canShowResultImage = SelectedInspection.CurrentValue as ICanShowResultImage;

            if (canShowResultImage.IsNotNull())
                canShowResultImage.ShowResultImage();
            else
                mainDisplaySource.CurrentValue = imageProcessSource.ColorImage.CurrentValue;
        }

        public void ImageViewControlMouseDown(Point mousePositionOnImage, MouseButtonEventArgs e)
        {
            if(currentContent.CurrentValue == SelectedInspection.CurrentValue)
                (SelectedInspection.CurrentValue as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseDown(mousePositionOnImage, e);
        }

        public void ImageViewControlMouseMove(Vector mouseVectorOnImage, Point mousePositionOnImage, MouseEventArgs e)
        {
            if (currentContent.CurrentValue == SelectedInspection.CurrentValue)
                (SelectedInspection.CurrentValue as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseMove(mouseVectorOnImage, mousePositionOnImage, e);
        }

        public void ImageViewControlMouseUp(Point mousePositionOnImage, MouseButtonEventArgs e)
        {
            if (currentContent.CurrentValue == SelectedInspection.CurrentValue)
                (SelectedInspection.CurrentValue as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseUp(mousePositionOnImage, e);
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(InspectionsDC));
            settingsCollection.KeyCreator.AddNew(nameof(Inspections));

            settingsCollection.KeyCreator.AddNew(nameof(Inspections.CurrentValue.Length));
            settingsCollection.SetValue(Inspections.CurrentValue.Length);

            for (int i = 0; i < Inspections.CurrentValue.Length; i++)
            {
                settingsCollection.KeyCreator.ReplaceLast(i.ToString());
                (Inspections.CurrentValue[i] as ICanSaveLoadSettings)?.SaveSettings(settingsCollection);
            }

            settingsCollection.KeyCreator.RemoveLast(3);
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(InspectionsDC));
            settingsCollection.KeyCreator.AddNew(nameof(Inspections));

            settingsCollection.KeyCreator.AddNew(nameof(Inspections.CurrentValue.Length));
            int length = settingsCollection.GetValue<int>();

            SelectedInspection.NextCurrentValueChangedDisabled = true;

            for (int i = 0; i < length; i++)
            {
                settingsCollection.KeyCreator.ReplaceLast(i.ToString());
                settingsCollection.KeyCreator.AddNew(nameof(Type));
                string typeName = settingsCollection.GetValue<string>();
                settingsCollection.KeyCreator.RemoveLast();

                if (typeName == nameof(InspectionDC))
                    AddNewInspectionDC();

                (Inspections.CurrentValue[i] as ICanSaveLoadSettings).LoadSettings(settingsCollection);
            }

            SelectedInspection.CurrentValue = null;
            SelectedInspection.NextCurrentValueChangedDisabled = false;

            settingsCollection.KeyCreator.RemoveLast(3);
        }
    }
}
