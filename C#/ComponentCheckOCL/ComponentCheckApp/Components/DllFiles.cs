using System.IO;


namespace ComponentCheckApp.Components
{
    public class DllFiles
    {
        public FileExistChecker FileExistChecker { get; }

        public DllFiles()
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            string[] filesToCheck = new string[]
            {
                Path.Combine(currentDirectory, nameof(CommonLib) + FileExistChecker.DllFileExtension),
                Path.Combine(currentDirectory, nameof(ImageProcessLib) + FileExistChecker.DllFileExtension),
                Path.Combine(currentDirectory, nameof(UVCLib) + FileExistChecker.DllFileExtension)
            };

            FileExistChecker = new FileExistChecker(filesToCheck);
        }
    }
}