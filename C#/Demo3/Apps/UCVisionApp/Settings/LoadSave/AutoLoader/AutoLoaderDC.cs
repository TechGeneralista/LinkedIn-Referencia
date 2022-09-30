using Common;
using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using Common.Settings;
using Microsoft.Win32;
using System;
using System.IO;


namespace UCVisionApp.Settings.LoadSave.AutoLoader
{
    public class AutoLoaderDC
    {
        public event Action<string> AutoLoad;


        public LanguageDC LanguageDC { get; }
        public IProperty<bool> IsEnable { get; }
        public IReadOnlyProperty<string> ProgramFilePath { get; }


        readonly LogDC logDC;
        readonly string dialogFilter;


        public AutoLoaderDC(LanguageDC languageDC, ISettingsCollection settingsCollection, LogDC logDC, string dialogFilter)
        {
            LanguageDC = languageDC;
            this.logDC = logDC;
            this.dialogFilter = dialogFilter;

            IsEnable = new StorableProperty<bool>(false, settingsCollection, nameof(AutoLoaderDC), nameof(IsEnable));
            ProgramFilePath = new StorableProperty<string>(string.Empty, settingsCollection, nameof(AutoLoaderDC), nameof(ProgramFilePath));
        }

        internal void Browse()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = dialogFilter;
            bool? result = openFileDialog.ShowDialog();

            if (result.IsNull() || (bool)result != true)
                ProgramFilePath.ToSettable().Value = null;
            else
                ProgramFilePath.ToSettable().Value = openFileDialog.FileName;
        }

        public void StartAutoLoad()
        {
            if (!IsEnable.Value || string.IsNullOrEmpty(ProgramFilePath.Value))
                return;

            if(File.Exists(ProgramFilePath.Value))
            {
                logDC.NewMessage(LogTypes.Information, LanguageDC.AutomaticLoadingStartedColon, ProgramFilePath.Value);
                AutoLoad?.Invoke(ProgramFilePath.Value);
            }
            else
                logDC.NewMessage(LogTypes.Error, LanguageDC.FileNotExistColon, ProgramFilePath.Value);
        }
    }
}
