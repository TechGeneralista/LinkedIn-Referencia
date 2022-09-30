using AppLog;
using Common;
using Common.NotifyProperty;
using Common.Settings;
using Language;
using Microsoft.Win32;
using System;
using System.IO;


namespace UniCamApp.Content.Settings.AutoLoader
{
    public class AutoLoaderDC
    {
        public event Action<string> AutoLoad;


        public LanguageDC LanguageDC { get; }
        public ISettableObservableProperty<bool> IsEnable { get; }
        public INonSettableObservableProperty<string> ProgramFilePath { get; }


        readonly ILog log;
        readonly string dialogFilter;


        public AutoLoaderDC(LanguageDC languageDC, ISettingsCollection settingsCollection, ILog log, string dialogFilter)
        {
            LanguageDC = languageDC;
            this.log = log;
            this.dialogFilter = dialogFilter;

            IsEnable = new StorableObservableProperty<bool>(false, settingsCollection, nameof(AutoLoaderDC), nameof(IsEnable));
            ProgramFilePath = new StorableObservableProperty<string>(string.Empty, settingsCollection, nameof(AutoLoaderDC), nameof(ProgramFilePath));
        }

        internal void Browse()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = dialogFilter;
            bool? result = openFileDialog.ShowDialog();

            if (result.IsNull() || (bool)result != true)
                ProgramFilePath.ForceSet(null);
            else
                ProgramFilePath.ForceSet(openFileDialog.FileName);
        }

        public void StartAutoLoad()
        {
            if (!IsEnable.CurrentValue || string.IsNullOrEmpty(ProgramFilePath.CurrentValue))
                return;

            if(File.Exists(ProgramFilePath.CurrentValue))
            {
                log.NewMessage(LogTypes.Information, LanguageDC.AutomaticLoadingStartedColon.CurrentValue, ProgramFilePath.CurrentValue);
                AutoLoad?.Invoke(ProgramFilePath.CurrentValue);
            }
            else
                log.NewMessage(LogTypes.Error, LanguageDC.FileNotExistColon.CurrentValue, ProgramFilePath.CurrentValue);
        }
    }
}
