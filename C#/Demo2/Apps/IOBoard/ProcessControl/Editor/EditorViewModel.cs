using Common.Prop;
using Common.Tool;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;


namespace Editor
{
    public class EditorViewModel
    {
        public ISettableObservableProperty<string> Code { get; } = new ObservableProperty<string>();
        public ISettableObservableProperty<bool> TexBoxIsReadOnly { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<bool> NewButtonIsEnable { get; } = new ObservableProperty<bool>(true);
        public ISettableObservableProperty<bool> OpenButtonIsEnable { get; } = new ObservableProperty<bool>(true);
        public EditorView View { get; set; }
        public bool CodeIsNotSaved { get; private set; }


        const string fileFilter = "Process control code file (*.pcc)|*.pcc";
        const string defaultProgramName = "DefaultProgram.pcc";


        public EditorViewModel()
        {
            Code.ValueChanged += (string o) => CodeIsNotSaved = true;

            string defaultProgramFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultProgramName);

            if (File.Exists(defaultProgramFilePath))
                LoadFile(defaultProgramFilePath);
            else
                ShowDefaultCodeTemplate();
        }

        private void ShowDefaultCodeTemplate()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Init:");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    ");
            stringBuilder.AppendLine("}");
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.AppendLine("Loop:");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    ");
            stringBuilder.AppendLine("}");

            Code.Value = stringBuilder.ToString();
            CodeIsNotSaved = false;
        }

        public void NewButtonClick()
        {
            if (CodeIsNotSaved && MessageBox.Show("A program nincs mentve! Szeretné menteni?", "Figyelem:", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                SaveButtonClick();

            ShowDefaultCodeTemplate();
        }

        public void OpenButtonClick()
        {
            if (CodeIsNotSaved && MessageBox.Show("A program nincs mentve! Szeretné menteni?", "Figyelem:", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                SaveButtonClick();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = fileFilter;
            bool? result = openFileDialog.ShowDialog();

            if (result.IsNull() || result == false)
                return;

            LoadFile(openFileDialog.FileName);
        }

        public void LoadFile(string fileName)
        {
            string[] codeArray = File.ReadAllLines(fileName);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < codeArray.Length; i++)
            {
                stringBuilder.Append(codeArray[i]);
                stringBuilder.Append("\r\n");
            }

            Code.Value = stringBuilder.ToString();
            CodeIsNotSaved = false;
        }

        public void SaveButtonClick()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = fileFilter;
            bool? result = saveFileDialog.ShowDialog();

            if (result.IsNull() || result == false)
                return;

            File.WriteAllLines(saveFileDialog.FileName, Code.Value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            CodeIsNotSaved = false;
        }

        public bool Closing()
        {
            if (CodeIsNotSaved && MessageBox.Show("A program nincs mentve! Biztos ki szeretne lépni?", "Figyelem:", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return true;

            return false;
        }
    }
}
