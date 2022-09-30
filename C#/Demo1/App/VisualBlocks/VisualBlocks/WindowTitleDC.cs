using Common;
using System.IO;


namespace VisualBlocks
{
    public class WindowTitleDC : DCBase
    {
        public string Title
        {
            get => title;
            private set => SetField(ref title, value);
        }
        string title;


        readonly string applicationTitle;


        public WindowTitleDC(string applicationTitle, string versionNumber)
        {
            this.applicationTitle = applicationTitle + $" {versionNumber}";
        }

        internal void UpdateTextTitle(bool isModified = false, string filePath = null)
        {
            if(filePath == null && !isModified)
                Title = string.Format("{0}", applicationTitle);
            else if (filePath == null && isModified)
                Title = string.Format("{0} - ({1})", applicationTitle, "Nincs mentve");
            if (filePath != null && !isModified)
                Title = string.Format("{0} - {1} ({2})", applicationTitle, Path.GetFileNameWithoutExtension(filePath), "Mentve");
            else if (filePath != null && isModified)
                Title = string.Format("{0} - {1} ({2})", applicationTitle, Path.GetFileNameWithoutExtension(filePath), "Nincs mentve");
        }
    }
}
