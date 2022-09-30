using AppLog;
using Common;
using Common.Settings;
using CustomControl.PopupWindow;
using Language;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using UniCamApp.Content.Settings.AutoLoader;


namespace UniCamApp.Settings
{
    public class SettingsDC
    {
        public LanguageDC LanguageDC { get; }
        public AutoLoaderDC AutoLoaderDC { get; }


        readonly string dialogFilter = "UniCamConfig (.ucc)|*.ucc";
        readonly IPopupWindow popupWindow;
        readonly ICanSaveLoadSettings canSaveLoadSettings;
        readonly ILog log;
        readonly Action stopTriggers;
        readonly Func<Task> masterTriggerCycleAsync;


        public SettingsDC(LanguageDC languageDC, ISettingsCollection settingsCollection, ILog log, IPopupWindow popupWindow, ICanSaveLoadSettings canSaveLoadSettings, Action stopTriggers, Func<Task> masterTriggerCycleAsync)
        {
            LanguageDC = languageDC;
            this.popupWindow = popupWindow;
            this.canSaveLoadSettings = canSaveLoadSettings;
            this.log = log;
            this.stopTriggers = stopTriggers;
            this.masterTriggerCycleAsync = masterTriggerCycleAsync;

            AutoLoaderDC = new AutoLoaderDC(languageDC, settingsCollection, log, dialogFilter);
            AutoLoaderDC.AutoLoad += Open;
        }

        internal void OpenButton()
        {
            stopTriggers?.Invoke();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = dialogFilter;
            bool? result = openFileDialog.ShowDialog();

            if (result.IsNull() || (bool)result != true)
                return;

            Open(openFileDialog.FileName);
        }

        private async void Open(string fileName)
        {
            ISettingsStore settingsStore = new FileSettingsStore(fileName);
            ISettingsCollection settingsCollection = new SettingsCollection(settingsStore, true);

            if (settingsCollection.SettingsStore.IsErrorOccured)
                log.NewMessage(LogTypes.Error, LanguageDC.ReadFileColon.CurrentValue, fileName);
            else
                log.NewMessage(LogTypes.Successful, LanguageDC.ReadFileColon.CurrentValue, fileName);

            popupWindow.Show();
            await Task.Run(() => canSaveLoadSettings?.LoadSettings(settingsCollection));
            await masterTriggerCycleAsync?.Invoke();
            popupWindow.Close();

            if (settingsCollection.IsErrorOccured)
                log.NewMessage(LogTypes.Error, LanguageDC.LoadProgramColon.CurrentValue, fileName);
            else
                log.NewMessage(LogTypes.Successful, LanguageDC.LoadProgramColon.CurrentValue, fileName);
        }

        public async void SaveButton()
        {
            stopTriggers?.Invoke();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = dialogFilter;
            bool? result = saveFileDialog.ShowDialog();

            if (result.IsNull() || (bool)result != true)
                return;

            ISettingsStore settingsStore = new FileSettingsStore(saveFileDialog.FileName);
            ISettingsCollection settingsCollection = new SettingsCollection(settingsStore);

            popupWindow.Show();
            await Task.Run(() => canSaveLoadSettings?.SaveSettings(settingsCollection));
            popupWindow.Close();

            if (settingsCollection.IsErrorOccured)
                log.NewMessage(LogTypes.Error, LanguageDC.SaveProgramColon.CurrentValue, saveFileDialog.FileName);
            else
                log.NewMessage(LogTypes.Successful, LanguageDC.SaveProgramColon.CurrentValue, saveFileDialog.FileName);

            settingsCollection.Write();

            if (settingsCollection.SettingsStore.IsErrorOccured)
                log.NewMessage(LogTypes.Error, LanguageDC.WriteFileColon.CurrentValue, saveFileDialog.FileName);
            else
                log.NewMessage(LogTypes.Successful, LanguageDC.WriteFileColon.CurrentValue, saveFileDialog.FileName);
        }
    }
}
