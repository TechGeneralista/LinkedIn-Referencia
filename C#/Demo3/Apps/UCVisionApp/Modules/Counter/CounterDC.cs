using Common.Interfaces;
using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using Common.SaveResult;
using Common.Settings;
using ImageProcess.ContourScanner.ContourDetector;
using ImageProcess.Modules.Counter;
using System.Collections.Generic;
using System.Windows.Media;


namespace UCVisionApp.Modules.Counter
{
    public class CounterDC : IHasName, IHasId, IHasResult, ICanProcessCycle, ICanAfterProcessCycle, IHasIsSelected, ICanRemote, ICanSaveLoadSettings, ICanSaveResult
    {
        public IProperty<bool> IsSelected { get; } = new Property<bool>();
        public IReadOnlyProperty<ImageSource> ResultImage { get; } = new Property<ImageSource>();
        public IReadOnlyProperty<string> Name { get; protected set; }
        public IProperty<string> Id => CounterModuleDC.IdDC.Id;
        public IReadOnlyProperty<bool?> Result => CounterModuleDC.Result;
        public CounterModuleDC CounterModuleDC { get; }


        readonly CounterResultDrawer counterResultDrawer;


        public CounterDC(LanguageDC languageDC, LogDC logDC, DetectorResultDC detectorResultDC)
        {
            Name = languageDC.CounterModule;

            CounterModuleDC = new CounterModuleDC(languageDC, logDC, detectorResultDC);
            counterResultDrawer = new CounterResultDrawer(detectorResultDC, ResultImage, CounterModuleDC.Result);
        }

        public void ProcessCycle()
        {
            CounterModuleDC.ProcessCycle();
        }

        public void AfterProcessCycle()
        {
            counterResultDrawer.Draw();
        }

        public string Remote(string command, string[] ids) => CounterModuleDC.Remote(command, ids);

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            CounterModuleDC.SaveSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            CounterModuleDC.LoadSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }

        public void SaveResult(KeyCreator keyCreator, List<SaveResultDTO> list)
        {
            keyCreator.AddNew(Id.Value);

            list.Add(new SaveResultDTO(keyCreator, Result.Value, ResultImage.Value));

            keyCreator.RemoveLast();
        }
    }
}
