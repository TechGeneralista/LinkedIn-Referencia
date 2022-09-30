using Common.SettingBackupAndRestore;
using System.Collections.Generic;


namespace VisualBlocks.Module.Base
{
    internal class BlockItemGroupDC : BlockItemDC, ICanBackupAndRestore
    {
        public double Width
        {
            get => width;
            set
            {
                SetField(ref width, value);
                dependencyParams.NotifyProjectChanged();
            }
        }
        double width;

        public double Height
        {
            get => height;
            set 
            {
                SetField(ref height, value);
                dependencyParams.NotifyProjectChanged();
            }
        }
        double height;

        public string Name
        {
            get => name;
            set 
            {
                SetField(ref name, value);
                dependencyParams.NotifyProjectChanged();
            }
        }
        string name = "Csoport";


        public BlockItemGroupDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            ZIndex = 1;
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(Left), Left);
            bar.SetData(nameof(Top), Top);
            bar.SetData(nameof(Width), Width);
            bar.SetData(nameof(Height), Height);
            bar.SetData(nameof(Name), Name);
            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);
            Left = bar.GetData<double>(nameof(Left));
            Top = bar.GetData<double>(nameof(Top));
            Width = bar.GetData<double>(nameof(Width));
            Height = bar.GetData<double>(nameof(Height));
            Name = bar.GetData<string>(nameof(Name));
        }
    }
}
