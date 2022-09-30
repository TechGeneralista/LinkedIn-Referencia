using Common.SettingBackupAndRestore;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.Trigger
{
    internal class TriggerKeyPressDC : BlockItemDC, ICanKeyPress, ICanBackupAndRestore
    {
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC_Pressed { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC_Released { get; }
        public BlockItemDataOutputDC<string> BlockItemDataOutputDC { get; }

        public bool RepeatIsEnabled
        {
            get => repeatIsEnabled;
            set
            {
                SetField(ref repeatIsEnabled, value);
                dependencyParams.NotifyProjectChanged();
            }
        }
        bool repeatIsEnabled;


        Task pressTask, releaseTask;


        public TriggerKeyPressDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemTriggerOutputDC_Pressed = new BlockItemTriggerOutputDC(dependencyParams);
            BlockItemTriggerOutputDC_Released = new BlockItemTriggerOutputDC(dependencyParams);
            BlockItemDataOutputDC = new BlockItemDataOutputDC<string>();
        }

        public async void KeyPress(KeyEventArgs e)
        {
            if(e.Key == Key.None ||
               e.Key == Key.LWin || e.Key == Key.RWin ||
               e.Key == Key.LeftShift || e.Key == Key.RightShift ||
               e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
               e.Key == Key.LeftAlt || e.Key == Key.RightAlt || (e.IsRepeat && !repeatIsEnabled))
            {
                return;
            }

            StringBuilder stringBuilder = new StringBuilder();

            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
                stringBuilder.Append($"{nameof(ModifierKeys.Windows)}+");
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                stringBuilder.Append($"{nameof(ModifierKeys.Shift)}+");
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                stringBuilder.Append($"{nameof(ModifierKeys.Control)}+");
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                stringBuilder.Append($"{nameof(ModifierKeys.Alt)}+");

            stringBuilder.Append(e.Key.ToString());
            BlockItemDataOutputDC.Value = stringBuilder.ToString();

            if (e.IsDown)
            {
                if (pressTask == null || (pressTask != null && pressTask.Status == TaskStatus.RanToCompletion))
                    await (pressTask = Task.Run(() => BlockItemTriggerOutputDC_Pressed.Trigger()));
            }
            else if (e.IsUp)
            {
                if (releaseTask == null || (releaseTask != null && releaseTask.Status == TaskStatus.RanToCompletion))
                    await (releaseTask = Task.Run(() => BlockItemTriggerOutputDC_Released.Trigger()));
            }
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(Left), Left);
            bar.SetData(nameof(Top), Top);
            bar.SetData(nameof(RepeatIsEnabled), RepeatIsEnabled);
            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);
            Left = bar.GetData<double>(nameof(Left));
            Top = bar.GetData<double>(nameof(Top));
            RepeatIsEnabled = bar.GetData<bool>(nameof(RepeatIsEnabled));
        }
    }
}
