using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeFloat
{
    internal class FloatMonitorDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<float?> BlockItemDataInputDC { get; }
        public BlockItemDataOutputDC<float?> BlockItemDataOutputDC { get; }

        public bool IsEnabled
        {
            get => isEnabled;
            set 
            {
                SetField(ref isEnabled, value);
                dependencyParams.NotifyProjectChanged();
            }
        }
        bool isEnabled = true;

        public string Value
        {
            get => value;
            private set => SetField(ref this.value, value);
        }
        string value;

        public int SelectedRoundOption
        {
            get => selectedRoundOption;
            set 
            {
                SetField(ref selectedRoundOption, value);
                dependencyParams.NotifyProjectChanged();
            }
        }
        int selectedRoundOption = 4;


        public FloatMonitorDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataInputDC = new BlockItemDataInputDC<float?>(dependencyParams);
            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemDataOutputDC = new BlockItemDataOutputDC<float?>();
            BlockItemDataOutputDC.PullAction += BlockItemDataOutputDC_PullAction;

            BlockItemTriggerOutputDC = new BlockItemTriggerOutputDC(dependencyParams);
        }

        private void BlockItemTriggerInputDC_TriggerAction(object sender, TriggerActionArgs e)
        {
            ExecuteOperation();
            BlockItemTriggerOutputDC.Trigger(e.Token);
            BlockItemDataOutputDC.Value = null;
        }

        private void BlockItemDataOutputDC_PullAction(object sender, EventArgs e)
        {
            // Húzásra csak akkor indítjuk a műveletet ha a trigger nincs csatlakoztatva
            if (!BlockItemTriggerInputDC.Connected)
                ExecuteOperation();
        }

        private void ExecuteOperation()
        {
            float? value = BlockItemDataInputDC.Value;

            if (!value.HasValue)
                SetStatus(Status.Error, "A bemenetre nem érkezett kiértékelhető eredmény!");
            else
                SetStatus(Status.Ok);

            if (isEnabled && value.HasValue)
            {
                switch (selectedRoundOption)
                {
                    case 0:
                        Value = value.ToString();
                        break;

                    case 1:
                        Value = string.Format(string.Format("{{0:0.{0}}}", new string('0', 0)), value);
                        break;

                    case 2:
                        Value = string.Format(string.Format("{{0:0.{0}}}", new string('0', 1)), value);
                        break;

                    case 3:
                        Value = string.Format(string.Format("{{0:0.{0}}}", new string('0', 2)), value);
                        break;

                    case 4:
                        Value = string.Format(string.Format("{{0:0.{0}}}", new string('0', 3)), value);
                        break;
                }
            }
            else
                Value = string.Empty;

            BlockItemDataOutputDC.Value = value;
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(Left), Left);
            bar.SetData(nameof(Top), Top);
            bar.SetData(nameof(IsEnabled), IsEnabled);
            bar.SetData(nameof(SelectedRoundOption), SelectedRoundOption);
            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);
            Left = bar.GetData<double>(nameof(Left));
            Top = bar.GetData<double>(nameof(Top));
            IsEnabled = bar.GetData<bool>(nameof(IsEnabled));
            SelectedRoundOption = bar.GetData<int>(nameof(SelectedRoundOption));
        }
    }
}
