using Common;
using Common.NotifyProperty;
using ImageProcess.ContourFinder.Detector;
using ImageProcess.Modules.Counter;
using Language;
using System.Collections.Generic;
using System.Windows.Media;
using OpenCLWrapper;
using ImageProcess.Source;
using Common.Interface;
using Common.Settings;
using System;
using AppLog;
using System.Linq;


namespace ImageProcess.Modules
{
    public class ModulesDC : ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public INonSettableObservableProperty<ModuleInformation[]> AvailableModules { get; } = new ObservableProperty<ModuleInformation[]>();
        public ISettableObservableProperty<ModuleInformation> SelectedNewModule { get; } = new ObservableProperty<ModuleInformation>();
        public INonSettableObservablePropertyArray<object> Modules { get; } = new ObservablePropertyArray<object>();
        public ISettableObservableProperty<object> SelectedModule { get; } = new ObservableProperty<object>();


        readonly OpenCLAccelerator openCLAccelerator;
        readonly ISettableObservableProperty<ImageSource> mainDisplaySource;
        IDetectorResult lastDetectorResult;
        IImageProcessSource lastInput;
        readonly ILog log;


        public ModulesDC(LanguageDC languageDC, OpenCLAccelerator openCLAccelerator, ISettableObservableProperty<ImageSource> mainDisplaySource, ILog log)
        {
            LanguageDC = languageDC;
            this.openCLAccelerator = openCLAccelerator;
            this.mainDisplaySource = mainDisplaySource;
            this.log = log;

            CreateAvailableModules();
            SelectedNewModule.CurrentValue = AvailableModules.CurrentValue[0];

            SelectedModule.CurrentValueChanged += (o, n) => ShowResultImage();
        }

        private void CreateAvailableModules()
        {
            List<ModuleInformation> moduleInformations = new List<ModuleInformation>();
            moduleInformations.Add(new ModuleInformation(nameof(CounterDC), LanguageDC.Counter, true));
            AvailableModules.ForceSet(moduleInformations.ToArray());
        }

        public void AddNewModule()
        {
            if (SelectedNewModule.CurrentValue.IsNull() || !SelectedNewModule.CurrentValue.IsEnable)
                return;

            if (SelectedNewModule.CurrentValue.TypeName == nameof(CounterDC))
                Modules.ForceAdd(new CounterDC(LanguageDC, lastDetectorResult, mainDisplaySource, log));

            SelectedModule.CurrentValue = Modules.CurrentValue.Last();
        }

        public void RemoveSelected()
        {
            if (SelectedNewModule.CurrentValue.IsNull())
                return;

            Modules.ForceRemove(SelectedModule.CurrentValue);
            SelectedModule.CurrentValue = null;
        }

        public void Cycle(IDetectorResult detectorResult, IImageProcessSource input)
        {
            lastDetectorResult = detectorResult;
            lastInput = input;

            foreach (object o in Modules.CurrentValue)
                (o as ICanEvulateDetectorResult)?.Evulate(detectorResult);
        }

        public void ShowResultImage() => (SelectedModule.CurrentValue as ICanShowResultImage)?.ShowResultImage();

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(ModulesDC));
            settingsCollection.KeyCreator.AddNew(nameof(Modules));
            settingsCollection.KeyCreator.AddNew(nameof(Modules.CurrentValue.Length));
            settingsCollection.SetValue(Modules.CurrentValue.Length);

            for(int i=0;i<Modules.CurrentValue.Length;i++)
            {
                settingsCollection.KeyCreator.ReplaceLast(i.ToString());
                (Modules.CurrentValue[i] as ICanSaveLoadSettings)?.SaveSettings(settingsCollection);
            }

            settingsCollection.KeyCreator.RemoveLast(3);
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(ModulesDC));
            settingsCollection.KeyCreator.AddNew(nameof(Modules));
            settingsCollection.KeyCreator.AddNew(nameof(Modules.CurrentValue.Length));
            int length = settingsCollection.GetValue<int>();

            for (int i = 0; i < length; i++)
            {
                settingsCollection.KeyCreator.ReplaceLast(i.ToString());
                settingsCollection.KeyCreator.AddNew(nameof(Type));
                string typeName = settingsCollection.GetValue<string>();
                settingsCollection.KeyCreator.RemoveLast();

                if(typeName == nameof(CounterDC))
                    Modules.ForceAdd(new CounterDC(LanguageDC, lastDetectorResult, mainDisplaySource, log));

                (Modules.CurrentValue[i] as ICanSaveLoadSettings).LoadSettings(settingsCollection);
            }

            settingsCollection.KeyCreator.RemoveLast(3);
        }
    }
}
