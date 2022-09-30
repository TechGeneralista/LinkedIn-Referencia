using Common;
using Common.Interfaces;
using Common.Language;
using Common.Log;
using Common.PopupWindow;
using Common.Settings;
using Microsoft.Win32;
using System.Threading.Tasks;
using UCVisionApp.Settings.LoadSave.AutoLoader;


namespace UCVisionApp.Settings.LoadSave
{
    public class LoadSaveDC
    {
        public LanguageDC LanguageDC { get; }
        public AutoLoaderDC AutoLoaderDC { get; }


        readonly string dialogFilter = "UniCamConfig (.ucc)|*.ucc";
        readonly LogDC logDC;
        readonly PopupWindowDC popupWindowDC;
        readonly ICanSaveLoadSettings canSaveLoadSettings;
        readonly ICanStopContinousTrigger canStopContinousTrigger;
        readonly ICanShootAsync canShootAsync;


        public LoadSaveDC(LanguageDC languageDC, LogDC logDC, PopupWindowDC popupWindowDC, ICanSaveLoadSettings canSaveLoadSettings, ICanStopContinousTrigger canStopContinousTrigger, ICanShootAsync canShootAsync, ISettingsCollection settingsCollection)
        {
            LanguageDC = languageDC;
            this.logDC = logDC;
            this.popupWindowDC = popupWindowDC;
            this.canSaveLoadSettings = canSaveLoadSettings;
            this.canStopContinousTrigger = canStopContinousTrigger;
            this.canShootAsync = canShootAsync;

            AutoLoaderDC = new AutoLoaderDC(languageDC, settingsCollection, logDC, dialogFilter);
            AutoLoaderDC.AutoLoad += Open;
        }

        public void OpenButtonClick()
        {
            canStopContinousTrigger.StopContinousTrigger();

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
                logDC.NewMessage(LogTypes.Error, LanguageDC.ReadFileColon, fileName);
            else
            {
                logDC.NewMessage(LogTypes.Successful, LanguageDC.ReadFileColon, fileName);

                popupWindowDC.Show();
                await Task.Run(() => canSaveLoadSettings.LoadSettings(settingsCollection));
                await canShootAsync.ShootAsync(true);
                popupWindowDC.Close();

                if (settingsCollection.IsErrorOccured)
                    logDC.NewMessage(LogTypes.Error, LanguageDC.LoadProgramColon, fileName);
                else
                    logDC.NewMessage(LogTypes.Successful, LanguageDC.LoadProgramColon, fileName);
            }
        }

        public async void SaveButtonClick()
        {
            canStopContinousTrigger.StopContinousTrigger();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = dialogFilter;
            bool? result = saveFileDialog.ShowDialog();

            if (result.IsNull() || (bool)result != true)
                return;

            ISettingsStore settingsStore = new FileSettingsStore(saveFileDialog.FileName);
            ISettingsCollection settingsCollection = new SettingsCollection(settingsStore);

            popupWindowDC.Show();
            await Task.Run(() => canSaveLoadSettings.SaveSettings(settingsCollection));
            popupWindowDC.Close();

            if (settingsCollection.IsErrorOccured)
                logDC.NewMessage(LogTypes.Error, LanguageDC.SaveProgramColon, saveFileDialog.FileName);
            else
            {
                logDC.NewMessage(LogTypes.Successful, LanguageDC.SaveProgramColon, saveFileDialog.FileName);
                settingsCollection.Write();

                if (settingsCollection.SettingsStore.IsErrorOccured)
                    logDC.NewMessage(LogTypes.Error, LanguageDC.WriteFileColon, saveFileDialog.FileName);
                else
                    logDC.NewMessage(LogTypes.Successful, LanguageDC.WriteFileColon, saveFileDialog.FileName);
            }
        }
    }
}
