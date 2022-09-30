using Common;
using Compute;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using VisualBlocks.VisualEditor;


namespace VisualBlocks.Project
{
    internal class ProjectDC : DCBase
    {
        public event EventHandler<ProjectInfoArgs> ProjectChanged;

        public bool IsModified
        {
            get => isModified;
            set
            {
                if (!value.Equals(isModified))
                {
                    ProjectChanged?.Invoke(this, new ProjectInfoArgs(value, filePath));
                    SetField(ref isModified, value);
                }
            }
        }
        bool isModified;

        public string FilePath
        {
            get => filePath;
            private set
            {
                if ((value != null && !value.Equals(filePath)) || (value == null && filePath != null))
                {
                    ProjectChanged?.Invoke(this, new ProjectInfoArgs(isModified, value));
                    SetField(ref filePath, value);
                }
            }
        }
        string filePath;

        public VisualEditorDC VisualEditorDC { get; }


        readonly Window mainWindow;
        readonly string fileFilterString = "AVB file (*.avb)|*.avb";


        public ProjectDC(ComputeAccelerator computeAccelerator, Window mainWindow)
        {
            this.mainWindow = mainWindow;
            VisualEditorDC = new VisualEditorDC(computeAccelerator, this, mainWindow);
        }

        internal void New()
        {
            if (CheckSaveStatus())
                return;

            VisualEditorDC.New();
            FilePath = null;
            IsModified = false;
        }

        public void Open(string filePathToOpen = null)
        {
           if (CheckSaveStatus())
                return;

            string openFilePath = null;

            if (filePathToOpen == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = fileFilterString;
                bool? dialogResult = openFileDialog.ShowDialog(mainWindow);

                if (dialogResult.HasValue && dialogResult.Value)
                    openFilePath = openFileDialog.FileName;
                else
                    return;
            }
            else
                openFilePath = filePathToOpen;

            Dictionary<string, object> container = null;

            try
            {
                container = File.ReadAllBytes(openFilePath).Deserialize<Dictionary<string, object>>();
                VisualEditorDC.Restore(container);
                FilePath = openFilePath;
            }
            catch (Exception ex)
            {
                VisualEditorDC.Items.Clear();
                MessageBox.Show(mainWindow, ex.Message, "Hiba, a megnyitás nem sikerült", MessageBoxButton.OK, MessageBoxImage.Error);
                FilePath = null;
            }

            IsModified = false;
        }

        internal Dictionary<string, object> Import()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = fileFilterString;
            bool? dialogResult = openFileDialog.ShowDialog(mainWindow);

            string openFilePath;

            if (dialogResult.HasValue && dialogResult.Value)
                openFilePath = openFileDialog.FileName;
            else
                return null;

            Dictionary<string, object> container = null;

            try
            {
                container = File.ReadAllBytes(openFilePath).Deserialize<Dictionary<string, object>>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(mainWindow, ex.Message, "Hiba, a megnyitás nem sikerült", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return container;
        }

        public void SaveAs(bool callSave = true)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = fileFilterString;
            bool? dialogResult = saveFileDialog.ShowDialog(mainWindow);

            if (dialogResult.HasValue && dialogResult.Value)
                FilePath = saveFileDialog.FileName;

            if (callSave && filePath != null)
                Save();
        }

        public void Save()
        {
            if (filePath is null)
                SaveAs(false);

            if (filePath is null)
                return;

            Dictionary<string, object> container = VisualEditorDC.Backup();

            try
            {
                File.WriteAllBytes(filePath, container.Serialize());
                IsModified = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(mainWindow, $"A mentés nem sikerült: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal bool Shutdown()
        {
            if (CheckSaveStatus())
                return true;

            VisualEditorDC.Shutdown();

            return false;
        }

        private bool CheckSaveStatus()
            => isModified &&
               MessageBox.Show(mainWindow, "Az aktuális projekt nincs mentve, folytatod mentés nélkül?", "Figyelmeztetés", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes;
    }
}
