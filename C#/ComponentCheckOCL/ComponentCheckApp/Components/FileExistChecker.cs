using System;
using System.IO;
using System.Windows;


namespace ComponentCheckApp.Components
{
    public class FileExistChecker
    {
        public const string DllFileExtension = ".dll";

        string[] files;

        public FileExistChecker(params string[] files)
        {
            this.files = files;
        }

        public bool AreExist()
        {
            foreach (string file in files)
            {
                if (!File.Exists(file))
                {
                    MessageBox.Show(string.Format("{0} is missing", file), "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }
    }
}
