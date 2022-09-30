using Common.Language;
using Common.NotifyProperty;
using System.IO;
using System.Windows.Media;


namespace Common.GetDriveFreeSpace
{
    public class GetDriveFreeSpaceDC
    {
        public LanguageDC LanguageDC { get; }

        public IReadOnlyProperty<long> TotalBytes { get; } = new Property<long>();
        public IReadOnlyProperty<long> UsedBytes { get; } = new Property<long>();
        public IReadOnlyProperty<string> ProgressBarColor { get; } = new Property<string>();

        public IReadOnlyProperty<double> Free { get; } = new Property<double>();
        public IReadOnlyProperty<string> FreeUnitOfMeasure { get; } = new Property<string>();
        public IReadOnlyProperty<double> Used { get; } = new Property<double>();
        public IReadOnlyProperty<string> UsedUnitOfMeasure { get; } = new Property<string>();
        public IReadOnlyProperty<double> Total { get; } = new Property<double>();
        public IReadOnlyProperty<string> TotalUnitOfMeasure { get; } = new Property<string>();


        readonly int alertInBytes;


        public GetDriveFreeSpaceDC(LanguageDC languageDC, int alertInBytes)
        {
            LanguageDC = languageDC;
            this.alertInBytes = alertInBytes;
        }

        internal void Refresh(string path)
        {
            string rootPath;

            try
            {
                rootPath = Path.GetPathRoot(path);
            }
            catch
            {
                TotalBytes.ToSettable().Value = 0;
                UsedBytes.ToSettable().Value = 0;
                ProgressBarColor.ToSettable().Value = Colors.Orange.ToString();

                Free.ToSettable().Value = 0;
                FreeUnitOfMeasure.ToSettable().Value = "";
                Used.ToSettable().Value = 0;
                UsedUnitOfMeasure.ToSettable().Value = "";
                Total.ToSettable().Value = 0;
                TotalUnitOfMeasure.ToSettable().Value = "";

                return;
            }

            DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(path));
            long freeBytes = driveInfo.TotalFreeSpace;
            long usedBytes = driveInfo.TotalSize - driveInfo.TotalFreeSpace;
            long totalBytes = driveInfo.TotalSize;

            TotalBytes.ToSettable().Value = totalBytes;
            UsedBytes.ToSettable().Value = usedBytes;

            if (freeBytes < alertInBytes)
                ProgressBarColor.ToSettable().Value = Colors.Red.ToString();
            else
                ProgressBarColor.ToSettable().Value = Colors.Green.ToString();

            Set(freeBytes, Free, FreeUnitOfMeasure);
            Set(usedBytes, Used, UsedUnitOfMeasure);
            Set(totalBytes, Total, TotalUnitOfMeasure);
        }

        private void Set(long bytes, IReadOnlyProperty<double> display, IReadOnlyProperty<string> displayUnitOfMeasure)
        {
            double mb = bytes / 1024 / 1024;
            double gb = mb / 1024;
            double tb = gb / 1024;

            if(tb > 1)
            {
                display.ToSettable().Value = tb;
                displayUnitOfMeasure.ToSettable().Value = "Tb";
            }

            else if (gb > 1)
            {
                display.ToSettable().Value = gb;
                displayUnitOfMeasure.ToSettable().Value = "Gb";
            }

            else
            {
                display.ToSettable().Value = mb;
                displayUnitOfMeasure.ToSettable().Value = "Mb";
            }
        }
    }
}
