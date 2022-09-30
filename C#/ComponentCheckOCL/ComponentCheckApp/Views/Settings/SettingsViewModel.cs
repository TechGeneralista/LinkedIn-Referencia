using CommonLib.Components;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;


namespace ComponentCheckApp.Views.Settings
{
    public class SettingsViewModel : ObservableProperty
    {
        public string WorkplaceName
        {
            get => workplaceName;
            set { SetField(value, ref workplaceName); CheckworkplaceName(); SaveSettings(); }
        }

        public string SavePath
        {
            get => savePath;
            private set { SetField(value, ref savePath); CheckSavePath(); SaveSettings(); }
        }

        string workplaceName, savePath;
        string settingsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ImageBackup.save");
        string currentSaveDirectory;


        public SettingsViewModel()
        {
            Application.Current.Resources[ResourceKeys.SettingsViewModelKey] = this;
            LoadSettings();
        }

        public void BrowseButtonClick()
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
                SavePath = folderBrowserDialog.SelectedPath;
        }

        public void DefaultButtonClick()
        {
            SavePath = Directory.GetCurrentDirectory();
        }

        private void SaveSettings()
        {
            string[] lines = new string[] { workplaceName, savePath };
            File.WriteAllLines(settingsFilePath, lines);
        }

        private void LoadSettings()
        {
            string[] lines = null;

            try
            {
                lines = File.ReadAllLines(settingsFilePath);
                WorkplaceName = lines[0];
                SavePath = lines[1];
            }
            catch
            {
                WorkplaceName = null;
                SavePath = null;
            }
        }

        public void SaveImage(WriteableBitmap image, string label)
        {
            if (label == null)
                label = string.Empty;

            CreateCurrentSaveDirectory();

            try
            {
                using (var fileStream = new FileStream(Path.Combine(currentSaveDirectory, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + label + ".jpg"), FileMode.Create))
                {
                    BitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(fileStream);
                }
            }
            catch
            {

            }
        }

        private void CreateCurrentSaveDirectory()
        {
            currentSaveDirectory = Path.Combine(savePath, workplaceName);

            if (!Directory.Exists(currentSaveDirectory))
                Directory.CreateDirectory(currentSaveDirectory);
        }

        private void CheckworkplaceName()
        {
            if (string.IsNullOrWhiteSpace(workplaceName))
                WorkplaceName = "workplace";
        }

        private void CheckSavePath()
        {
            if (string.IsNullOrWhiteSpace(savePath))
                DefaultButtonClick();
        }

        public void OpenFolderButtonClick()
        {
            CreateCurrentSaveDirectory();
            Process.Start(currentSaveDirectory);
        }
    }
}