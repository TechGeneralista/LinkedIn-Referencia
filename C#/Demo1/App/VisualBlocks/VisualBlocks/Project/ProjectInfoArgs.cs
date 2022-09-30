using System;


namespace VisualBlocks.Project
{
    public class ProjectInfoArgs : EventArgs
    {
        public bool IsModified { get; }
        public string FilePath { get; }

        public ProjectInfoArgs(bool isModified, string filePath)
        {
            IsModified = isModified;
            FilePath = filePath;
        }
    }
}