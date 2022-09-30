using Common;
using Common.SettingBackupAndRestore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace VisualBlocks.Module.Base
{
    internal class BlockItemDataConnectionBaseDC : DCBase, ISelectable
    {
        public bool Active
        {
            get => active;
            set => SetField(ref active, value);
        }
        bool active;

        public int ZIndex
        {
            get => zIndex;
            protected set => SetField(ref zIndex, value);
        }
        int zIndex = 0;

        public bool IsSelected
        {
            get => isSelected;
            set => SetField(ref isSelected, value);
        }
        bool isSelected;


        protected readonly DependencyParams dependencyParams;


        public BlockItemDataConnectionBaseDC(DependencyParams dependencyParams)
            => this.dependencyParams = dependencyParams;
    }

    internal class BlockItemDataConnectionDC<T> : BlockItemDataConnectionBaseDC, ICanBackupAndRestore
    {
        public BlockItemDataOutputDC<T> BlockItemDataOutputDC { get; protected set; }
        public BlockItemDataInputDC<T> BlockItemDataInputDC { get; protected set; }


        public BlockItemDataConnectionDC(DependencyParams dependencyParams) : base(dependencyParams) { }

        public BlockItemDataConnectionDC(DependencyParams dependencyParams, BlockItemDataOutputDC<T> sourceOutput, BlockItemDataInputDC<T> destinationInput) : base(dependencyParams)
        {
            BlockItemDataOutputDC = sourceOutput;
            BlockItemDataInputDC = destinationInput;
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bool outFound = false;
            bool inFound = false;

            lock (dependencyParams.Items.Lock)
            {
                foreach (object obj in dependencyParams.Items)
                {
                    if (obj == this)
                        continue;

                    foreach (PropertyInfo pi in obj.GetType().GetProperties())
                    {
                        if (pi.GetValue(obj) == BlockItemDataOutputDC && !outFound)
                        {
                            bar.SetData(nameof(BlockItemDataOutputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf), dependencyParams.Items.IndexOf(obj));
                            bar.SetData(nameof(BlockItemDataOutputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf) + nameof(PropertyInfo) + nameof(pi.Name), pi.Name);
                            outFound = true;
                        }

                        if (pi.GetValue(obj) == BlockItemDataInputDC && !inFound)
                        {
                            bar.SetData(nameof(BlockItemDataInputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf), dependencyParams.Items.IndexOf(obj));
                            bar.SetData(nameof(BlockItemDataInputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf) + nameof(PropertyInfo) + nameof(PropertyInfo.Name), pi.Name);
                            inFound = true;
                        }

                        if (outFound && inFound)
                            break;
                    }
                }
            }

            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);

            lock (dependencyParams.Items.Lock)
            {
                int? outputObjectOwnerIndex = bar.GetData<int?>(nameof(BlockItemDataOutputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf));

                if (outputObjectOwnerIndex.HasValue &&
                dependencyParams.Items[outputObjectOwnerIndex.Value] is object outputOwner &&
                bar.GetData<string>(nameof(BlockItemDataOutputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf) + nameof(PropertyInfo) + nameof(PropertyInfo.Name)) is string outputPropertyName &&
                outputOwner.GetType().GetProperties().Where(p => p.Name == outputPropertyName).FirstOrDefault() is PropertyInfo outputProperty &&
                outputProperty.GetValue(outputOwner) is BlockItemDataOutputDC<T> blockItemDataOutputDC)
                {
                    BlockItemDataOutputDC = blockItemDataOutputDC;
                }

                int? inputObjectOwnerIndex = bar.GetData<int?>(nameof(BlockItemDataInputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf));

                if (inputObjectOwnerIndex.HasValue &&
                dependencyParams.Items[inputObjectOwnerIndex.Value] is object inputOwner &&
                bar.GetData<string>(nameof(BlockItemDataInputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf) + nameof(PropertyInfo) + nameof(PropertyInfo.Name)) is string inputPropertyName &&
                inputOwner.GetType().GetProperties().Where(p => p.Name == inputPropertyName).FirstOrDefault() is PropertyInfo inputProperty &&
                inputProperty.GetValue(inputOwner) is BlockItemDataInputDC<T> blockItemDataInputDC)
                {
                    BlockItemDataInputDC = blockItemDataInputDC;
                }
            }
        }
    }
}
