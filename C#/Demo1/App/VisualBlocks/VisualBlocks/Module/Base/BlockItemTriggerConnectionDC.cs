using Common;
using Common.SettingBackupAndRestore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace VisualBlocks.Module.Base
{
    internal class BlockItemTriggerConnectionDC : DCBase, ICanBackupAndRestore, ISelectable
    {
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; private set; }
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; private set; }

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


        readonly DependencyParams dependencyParams;


        public BlockItemTriggerConnectionDC(DependencyParams dependencyParams)
        {
            this.dependencyParams = dependencyParams;
        }

        public BlockItemTriggerConnectionDC(DependencyParams dependencyParams, BlockItemTriggerOutputDC sourceOutput, BlockItemTriggerInputDC destinationInput) : this(dependencyParams)
        {
            BlockItemTriggerOutputDC = sourceOutput;
            BlockItemTriggerInputDC = destinationInput;
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bool outFound = false;
            bool inFound = false;

            lock(dependencyParams.Items.Lock)
            {
                foreach (object obj in dependencyParams.Items)
                {
                    if (obj == this)
                        continue;

                    foreach (PropertyInfo pi in obj.GetType().GetProperties())
                    {
                        if (pi.PropertyType == typeof(BlockItemTriggerOutputDC) &&
                           pi.GetValue(obj) is BlockItemTriggerOutputDC blockItemTriggerOutputDC &&
                           blockItemTriggerOutputDC == BlockItemTriggerOutputDC &&
                           !outFound)
                        {
                            bar.SetData(nameof(BlockItemTriggerOutputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf), dependencyParams.Items.IndexOf(obj));
                            bar.SetData(nameof(BlockItemTriggerOutputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf) + nameof(PropertyInfo) + nameof(pi.Name), pi.Name);
                            outFound = true;
                        }

                        if (pi.PropertyType == typeof(BlockItemTriggerInputDC) &&
                           pi.GetValue(obj) is BlockItemTriggerInputDC blockItemTriggerInputDC &&
                           blockItemTriggerInputDC == BlockItemTriggerInputDC &&
                           !inFound)
                        {
                            bar.SetData(nameof(BlockItemTriggerInputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf), dependencyParams.Items.IndexOf(obj));
                            bar.SetData(nameof(BlockItemTriggerInputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf) + nameof(PropertyInfo) + nameof(PropertyInfo.Name), pi.Name);
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
                int? outputObjectOwnerIndex = bar.GetData<int?>(nameof(BlockItemTriggerOutputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf));

                if (outputObjectOwnerIndex.HasValue &&
                    dependencyParams.Items[outputObjectOwnerIndex.Value] is object outputOwner &&
                    bar.GetData<string>(nameof(BlockItemTriggerOutputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf) + nameof(PropertyInfo) + nameof(PropertyInfo.Name)) is string outputPropertyName &&
                    outputOwner.GetType().GetProperties().Where(p => p.Name == outputPropertyName).FirstOrDefault() is PropertyInfo outputProperty &&
                    outputProperty.GetValue(outputOwner) is BlockItemTriggerOutputDC blockItemTriggerOutputDC)
                {
                    BlockItemTriggerOutputDC = blockItemTriggerOutputDC;
                }

                int? inputObjectOwnerIndex = bar.GetData<int?>(nameof(BlockItemTriggerInputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf));

                if (inputObjectOwnerIndex.HasValue &&
                    dependencyParams.Items[inputObjectOwnerIndex.Value] is object inputOwner &&
                    bar.GetData<string>(nameof(BlockItemTriggerInputDC) + nameof(dependencyParams.Items) + nameof(dependencyParams.Items.IndexOf) + nameof(PropertyInfo) + nameof(PropertyInfo.Name)) is string inputPropertyName &&
                    inputOwner.GetType().GetProperties().Where(p => p.Name == inputPropertyName).FirstOrDefault() is PropertyInfo inputProperty &&
                    inputProperty.GetValue(inputOwner) is BlockItemTriggerInputDC blockItemTriggerInputDC)
                {
                    BlockItemTriggerInputDC = blockItemTriggerInputDC;
                }
            }
        }
    }
}
