using Common.NotifyProperty;
using ImageProcess.ContourFinder;
using LogicalEvaluator;
using ImageProcess.Modules;
using Language;
using OpenCLWrapper;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ImageProcess.Source;
using Common.Settings;
using CustomControl.ImageViewControl;
using Common.Interface;
using System;
using ImageProcess.ReferenceImages;
using AppLog;
using CustomControl.Id;


namespace UniCamApp.Inspection
{
    public class InspectionDC : IHasId, IHasResult, ICanSaveLoadSettings, ICanShowResultImage, ICanProcessImageProcessSource, ICanHandleImageViewControlMouseEvents, ICanResetContent
    {
        public IdDC IdDC => LogicalEvaluatorDC.IdDC;
        public INonSettableObservableProperty<bool?> Result => LogicalEvaluatorDC.Result;
        public LanguageDC LanguageDC { get; }
        public ReferenceImagesDC ReferenceImagesDC { get; }
        public ISettableObservableProperty<int> SelectedTabItemIndex { get; } = new ObservableProperty<int>();
        public ContourFinderDC ContourFinderDC { get; }
        public ModulesDC ModulesDC { get; }
        public LogicalEvaluatorDC LogicalEvaluatorDC { get; }


        readonly ICanResetContent canResetContent;


        public InspectionDC(LanguageDC languageDC, OpenCLAccelerator openCLAccelerator, IImageProcessSource imageProcessSource, ISettableObservableProperty<ImageSource> mainDisplaySource, ILog log, ICanResetContent canResetContent)
        {
            LanguageDC = languageDC;
            this.canResetContent = canResetContent;

            ReferenceImagesDC = new ReferenceImagesDC(languageDC, mainDisplaySource, imageProcessSource);
            ContourFinderDC = new ContourFinderDC(languageDC, openCLAccelerator, imageProcessSource, ReferenceImagesDC, mainDisplaySource);
            ModulesDC = new ModulesDC(languageDC, openCLAccelerator, mainDisplaySource, log);
            LogicalEvaluatorDC = new LogicalEvaluatorDC(languageDC, log, ModulesDC.Modules);
        }

        internal void Back()
        {
            SelectedTabItemIndex.CurrentValue = 0;
            canResetContent.ResetContent();
        }

        public void ResetContent() => SelectedTabItemIndex.CurrentValue = 0;

        public void ImageViewControlMouseDown(Point mousePositionOnImage, MouseButtonEventArgs e)
        {
            switch(SelectedTabItemIndex.CurrentValue)
            {
                case 0:
                    ContourFinderDC.ImageViewControlMouseDown(mousePositionOnImage, e);
                    break;

                case 1:

                    break;

                case 2:

                    break;
            }
        }

        public void ImageViewControlMouseMove(Vector mouseVectorOnImage, Point mousePositionOnImage, MouseEventArgs e)
        {
            switch (SelectedTabItemIndex.CurrentValue)
            {
                case 0:
                    ContourFinderDC.ImageViewControlMouseMove(mouseVectorOnImage, mousePositionOnImage, e);
                    break;

                case 1:

                    break;

                case 2:

                    break;
            }
        }

        public void ImageViewControlMouseUp(Point mousePositionOnImage, MouseButtonEventArgs e)
        {
            switch (SelectedTabItemIndex.CurrentValue)
            {
                case 0:
                    ContourFinderDC.ImageViewControlMouseUp(mousePositionOnImage, e);
                    break;

                case 1:

                    break;

                case 2:

                    break;
            }
        }

        public void Process(IImageProcessSource input)
        {
            ContourFinderDC.Process(input);
            ModulesDC.Cycle(ContourFinderDC.DetectorDC.DetectorResult, input);
            LogicalEvaluatorDC.Evulate();
        }

        public void ShowResultImage()
        {
            switch (SelectedTabItemIndex.CurrentValue)
            {
                case 0:
                    ContourFinderDC.ShowResultImage();
                    break;

                case 1:
                    ModulesDC.ShowResultImage();
                    break;

                case 2:

                    break;
            }
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(Type));
            settingsCollection.SetValue(nameof(InspectionDC));

            settingsCollection.KeyCreator.ReplaceLast(nameof(InspectionDC));
            ReferenceImagesDC.SaveSettings(settingsCollection);
            ContourFinderDC.SaveSettings(settingsCollection);
            ModulesDC.SaveSettings(settingsCollection);
            LogicalEvaluatorDC.SaveSettings(settingsCollection);
            settingsCollection.KeyCreator.RemoveLast();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(InspectionDC));
            ReferenceImagesDC.LoadSettings(settingsCollection);
            ContourFinderDC.LoadSettings(settingsCollection);
            ModulesDC.LoadSettings(settingsCollection);
            LogicalEvaluatorDC.LoadSettings(settingsCollection);
            settingsCollection.KeyCreator.RemoveLast();
        }
    }
}
