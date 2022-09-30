using Common;
using System.Collections.Generic;
using System.IO;
using System.Windows;


namespace UCVisionResultExplorerApp
{
    public class DLLFiles
    {
        List<string> fileNames = new List<string>();
        readonly string fileExtension = ".dll";


        public void AddFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
                return;

            fileNames.Add(Utils.GetPath(string.Format("{0}{1}", fileName, fileExtension)));
        }

        public bool CheckExistence()
        {
            foreach (string fileName in fileNames)
            {
                if (!File.Exists(fileName))
                {
                    MessageBox.Show(string.Format("{0}: {1}", "The application cannot be started, the file is missing", fileName), "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }
    }
}
