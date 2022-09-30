using Common.Language;
using Common.NotifyProperty;
using Common.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using WinForms = System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using Common.GetDriveFreeSpace;


namespace Common.SaveResult
{
    public class SaveResultDC : ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public IProperty<bool> IsEnable { get; }
        public IProperty<int> ImageQuality { get; }
        public IReadOnlyProperty<string> SavePath { get; }
        public IProperty<bool> SaveOkImage { get; }
        public IProperty<bool> SaveNokImage { get; }
        public GetDriveFreeSpaceDC GetDriveFreeSpaceDC { get; }


        public SaveResultDC(LanguageDC languageDC)
        {
            LanguageDC = languageDC;

            IsEnable = new Property<bool>(false);
            ImageQuality = new Property<int>(50);
            SavePath = new Property<string>(Utils.GetPath("UniCamSaveDatas"));
            SaveOkImage = new Property<bool>(true);
            SaveNokImage = new Property<bool>(true);

            GetDriveFreeSpaceDC = new GetDriveFreeSpaceDC(languageDC, (1 * 1024 * 1024) * 100);
            GetDriveFreeSpaceDC.Refresh(SavePath.Value);

            SaveOkImage.OnValueChanged += SaveOk_OnValueChanged;
            SaveNokImage.OnValueChanged += SaveNok_OnValueChanged;
        }

        private void SaveOk_OnValueChanged(bool o, bool n)
        {
            if (!n && !SaveNokImage.Value)
            {
                SaveNokImage.DisableNextOnValueChangeEvents();
                SaveNokImage.Value = true;
            }
        }

        private void SaveNok_OnValueChanged(bool o, bool n)
        {
            if (!n && !SaveOkImage.Value)
            {
                SaveOkImage.DisableNextOnValueChangeEvents();
                SaveOkImage.Value = true;
            }
        }

        internal void Browse()
        {
            WinForms.FolderBrowserDialog folderBrowserDialog = new WinForms.FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = SavePath.Value;
            WinForms.DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult != WinForms.DialogResult.OK)
                return;

            SavePath.ToSettable().Value = folderBrowserDialog.SelectedPath;
        }

        internal void OpenFolder()
        {
            if(Directory.Exists(SavePath.Value))
                Process.Start(SavePath.Value);
        }

        internal void WriteDatas(List<SaveResultDTO> list)
        {
            if (list.Count == 0)
                return;

            string saveDirectory = Path.Combine(SavePath.Value, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff"));

            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            List<string> resultTexts = new List<string>();

            foreach(SaveResultDTO saveResultDTO in list)
            {
                string resultText = string.Format("{0}={1}", saveResultDTO.Id, saveResultDTO.Result);
                resultTexts.Add(resultText);

                if(saveResultDTO.Image.IsNotNull() && ((saveResultDTO.Result == Result.Ok.ToString() && SaveOkImage.Value) || (saveResultDTO.Result == Result.Nok.ToString() && SaveNokImage.Value)))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(saveResultDTO.Image.ToBitmapSource()));
                    encoder.QualityLevel = ImageQuality.Value;

                    using (FileStream file = File.OpenWrite(Path.Combine(saveDirectory, string.Format("{0}.jpg", resultText.Replace('/', '_').Replace('=', '_')))))
                        encoder.Save(file);
                }
            }

            File.WriteAllLines(Path.Combine(saveDirectory, "Results.txt"), resultTexts.ToArray());
        }

        public Task RefreshFreeSpaceBarAsync()
            => Task.Run(()=> GetDriveFreeSpaceDC.Refresh(SavePath.Value));

        #region SaveLoad
        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);

            settingsCollection.SetProperty(IsEnable.Value, nameof(IsEnable));
            settingsCollection.SetProperty(ImageQuality.Value, nameof(ImageQuality));
            settingsCollection.SetProperty(SavePath.Value, nameof(SavePath));
            settingsCollection.SetProperty(SaveOkImage.Value, nameof(SaveOkImage));
            settingsCollection.SetProperty(SaveNokImage.Value, nameof(SaveNokImage));

            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);

            IsEnable.Value = settingsCollection.GetProperty<bool>(nameof(IsEnable));
            ImageQuality.Value = settingsCollection.GetProperty<int>(nameof(ImageQuality));
            SavePath.ToSettable().Value = settingsCollection.GetProperty<string>(nameof(SavePath));

            SaveOkImage.DisableNextOnValueChangeEvents();
            SaveOkImage.Value = settingsCollection.GetProperty<bool>(nameof(SaveOkImage));

            SaveNokImage.DisableNextOnValueChangeEvents();
            SaveNokImage.Value = settingsCollection.GetProperty<bool>(nameof(SaveNokImage));

            settingsCollection.ExitPoint();
        }
        #endregion
    }
}
