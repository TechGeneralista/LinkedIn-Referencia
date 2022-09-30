using Common;
using Common.Interfaces;
using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using Common.SaveResult;
using Common.Settings;
using ImageProcess.ContourScanner;
using ImageProcess.Source;
using OpenCLWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UCVisionApp.LevelBase;
using UCVisionApp.Modules.Counter;


namespace UCVisionApp.Inspection
{
    public class InspectionDC : LevelBaseDC, ICanResetSelectedTabItemIndex, ICanBeforeProcessCycle, ICanProcessCycle, ICanAfterProcessCycle, ICanRemote, ICanSaveLoadSettings, ICanSaveResult
    {
        public IReadOnlyProperty<ImageSource> ResultImage { get; } = new Property<ImageSource>();
        public IProperty<int> SelectedTabItemIndex { get; } = new Property<int>();
        public IProperty<int> InternalSelectedTabItemIndex { get; } = new Property<int>();
        public ContourScannerDC ContourScannerDC { get; }
        public IReadOnlyPropertyArray<ModuleInformation> AvailableModules { get; } = new PropertyArray<ModuleInformation>();
        public IProperty<ModuleInformation> SelectedNewModule { get; } = new Property<ModuleInformation>();


        readonly object lockObject = new object();


        public InspectionDC(LanguageDC languageDC, LogDC logDC, OpenCLAccelerator openCLAccelerator, ImageProcessSourceDC imageProcessSourceDC) : base(languageDC, logDC)
        {
            Name = languageDC.Inspection;
            ResultImage.ToSettable().Value = imageProcessSourceDC.ColorImage.Value;

            ContourScannerDC = new ContourScannerDC(languageDC, openCLAccelerator, imageProcessSourceDC, ResultImage);

            CreateAvailableModules();
            SelectedNewModule.Value = AvailableModules.Value[0];
        }

        private void CreateAvailableModules()
        {
            List<ModuleInformation> moduleInformations = new List<ModuleInformation>();
            moduleInformations.Add(new ModuleInformation(nameof(CounterDC), LanguageDC.Counter, true));
            AvailableModules.ToSettable().AddRange(moduleInformations.ToArray());
        }

        public void ResetSelectedTabItemIndex()
        {
            SelectedTabItemIndex.Value = 0;
            InternalSelectedTabItemIndex.Value = 0;
        }

        public void BeforeProcessCycle()
        {
            lock(lockObject)
            {
                Children.ForEach(x => x.CastTo<ICanBeforeProcessCycle>()?.BeforeProcessCycle());
            }
        }

        public void ProcessCycle()
        {
            lock (lockObject)
            {
                ContourScannerDC.Process();
                Children.ForEach(x => x.CastTo<ICanProcessCycle>()?.ProcessCycle());
            }
        }

        public void AfterProcessCycle()
        {
            lock (lockObject)
            {
                Children.ForEach(x => x.CastTo<ICanAfterProcessCycle>()?.AfterProcessCycle());
                LogicalEvaluatorDC.Evulate();
                ContourScannerDC.DrawResultImage();
            }
        }

        internal void PanZoomImageViewMouseDown(Point downPos, MouseButtonEventArgs e)
        {
            if (InternalSelectedTabItemIndex.Value != 0)
                return;

            ContourScannerDC.PanZoomImageViewMouseDown(downPos, e);
        }

        internal void PanZoomImageViewMouseMove(Vector moveVec, Point movePos, MouseEventArgs e)
        {
            if (InternalSelectedTabItemIndex.Value != 0)
                return;

            ContourScannerDC.PanZoomImageViewMouseMove(moveVec, movePos, e);
        }

        internal void PanZoomImageViewMouseUp(Point upPos, MouseButtonEventArgs e)
        {
            if (InternalSelectedTabItemIndex.Value != 0)
                return;

            ContourScannerDC.PanZoomImageViewMouseUp(upPos, e);
        }

        internal void AddNewModuleButtonClick()
        {
            lock (lockObject)
            {
                if (SelectedNewModule.Value.IsNull() || !SelectedNewModule.Value.IsEnable)
                    return;

                if (SelectedNewModule.Value.TypeName == nameof(CounterDC))
                    Children.ToSettable().Add(new CounterDC(LanguageDC, logDC, ContourScannerDC.ContourDetectorDC.DetectorResultDC));
            }
        }

        internal void RemoveSelectedModuleButtonClick()
        {
            lock (lockObject)
            {
                Children.ToSettable().Remove(SelectedChild.Value);
                SelectedChild.Value = null;
            }
        }

        internal void RemoveAllModulesButtonClick()
        {
            if (Children.Value.Length == 0)
                return;

            if (Utils.ShowAreYouSureYouWantToDeleteThemAllQuestion(LanguageDC) == MessageBoxResult.No)
                return;

            lock (lockObject)
            {
                Children.ToSettable().Clear();
            }
        }

        public string Remote(string command, string[] ids)
        {
            string response = LogicalEvaluatorDC.Remote(command, ids);

            if (response.IsNull())
            {
                if (ids.Length > 1 && Id.Value == ids.First())
                {
                    string[] idsFirstRemoved = ids.RemoveFirst();

                    foreach (object o in Children.Value)
                    {
                        o.CastTo<ICanRemote>().Remote(command, idsFirstRemoved);

                        if (response.IsNotNull())
                            break;
                    }
                }
            }

            return response;
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            ContourScannerDC.SaveSettings(settingsCollection);

            settingsCollection.AddKey(nameof(Children));
            int length = Children.Value.Length;
            settingsCollection.SetProperty(length, nameof(Children.Value.Length));

            for (int index = 0; index < length; index++)
            {
                settingsCollection.AddKey(index.ToString());
                settingsCollection.SetProperty(Children.Value[index].GetType().Name, nameof(Type));
                Children.Value[index].CastTo<ICanSaveLoadSettings>().SaveSettings(settingsCollection);
                settingsCollection.RemoveLastKey(); // index.ToString()
            }

            settingsCollection.RemoveLastKey(); // nameof(Children)
            LogicalEvaluatorDC.SaveSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            ContourScannerDC.LoadSettings(settingsCollection);

            settingsCollection.AddKey(nameof(Children));
            int length = settingsCollection.GetProperty<int>(nameof(Children.Value.Length));

            for (int index = 0; index < length; index++)
            {
                settingsCollection.AddKey(index.ToString());
                string typeName = settingsCollection.GetProperty<string>(nameof(Type));

                if (typeName == nameof(CounterDC))
                {
                    Children.ToSettable().Add(new CounterDC(LanguageDC, logDC, ContourScannerDC.ContourDetectorDC.DetectorResultDC));
                    Children.Value[index].CastTo<ICanSaveLoadSettings>().LoadSettings(settingsCollection);
                }

                settingsCollection.RemoveLastKey(); // index.ToString()
            }

            settingsCollection.RemoveLastKey(); // nameof(Children)
            LogicalEvaluatorDC.LoadSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }

        public void SaveResult(KeyCreator keyCreator, List<SaveResultDTO> list)
        {
            keyCreator.AddNew(Id.Value);

            list.Add(new SaveResultDTO(keyCreator, Result.Value, ContourScannerDC.ContourDetectorDC.ContourDetectorResultDrawer.ResultImage.Value));

            keyCreator.AddNew("Piece");
            list.Add(new SaveResultDTO(keyCreator, ContourScannerDC.ContourDetectorDC.DetectorResultDC.PositionResults.Value.Length, null));
            keyCreator.RemoveLast();

            Children.ForEach(x => x.CastTo<ICanSaveResult>()?.SaveResult(keyCreator, list));

            keyCreator.RemoveLast();
        }
    }
}
