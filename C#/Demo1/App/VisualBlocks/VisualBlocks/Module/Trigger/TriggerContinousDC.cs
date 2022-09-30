using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.Trigger
{
    internal class TriggerContinousDC : BlockItemDC, ICanBackupAndRestore, IShutdownable
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<bool?> BlockItemDataInputDC_IsEnabled { get; }
        public BlockItemDataInputDC<int?> BlockItemDataInputDC_DelayMs { get; }


        readonly AutoResetEvent startEvent, endEvent;
        readonly CancellationTokenSource cancellationTokenSource;
        readonly Task task;
        volatile int delayMs;
        volatile bool isRunning;


        public TriggerContinousDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemTriggerOutputDC = new BlockItemTriggerOutputDC(dependencyParams);

            BlockItemDataInputDC_IsEnabled = new BlockItemDataInputDC<bool?>(dependencyParams);
            BlockItemDataInputDC_DelayMs = new BlockItemDataInputDC<int?>(dependencyParams);

            startEvent = new AutoResetEvent(false);
            endEvent = new AutoResetEvent(false);
            cancellationTokenSource = new CancellationTokenSource();
            task = Task.Run(() => continousTriggerMethod(), cancellationTokenSource.Token);
        }

        private void BlockItemTriggerInputDC_TriggerAction(object sender, TriggerActionArgs e)
            => ExecuteOperation(e);

        private void ExecuteOperation(TriggerActionArgs e)
        {
            bool? isEnabled = BlockItemDataInputDC_IsEnabled.Value;

            if (!isEnabled.HasValue)
            {
                SetStatus(Status.Error, "Az 'engedélyezve' bemenetre nem érkezett kiértékelhető eredmény!");
                return;
            }

            int? delayMs = BlockItemDataInputDC_DelayMs.Value;

            if (!delayMs.HasValue)
            {
                SetStatus(Status.Error, "A 'ms' bemenetre nem érkezett kiértékelhető eredmény!");
                return;
            }

            if (delayMs.Value < 0)
            {
                SetStatus(Status.Error, "A 'ms' értéknek nagyobbnak vagy egyenlőnek kell lennie nullánál!");
                return;
            }

            this.delayMs = delayMs.Value;

            if (isEnabled.Value && !isRunning)
                startEvent.Set();
            else if (!isEnabled.Value && isRunning)
                endEvent.Set();

            SetStatus(Status.Ok);
        }

        private void continousTriggerMethod()
        {
            while(true)
            {
                isRunning = false;
                
                if(startEvent.WaitOne(100))
                {
                    while (true)
                    {
                        isRunning = true;

                        if (endEvent.WaitOne(delayMs))
                            break;

                        BlockItemTriggerOutputDC.Trigger();
                    }
                }

                if (cancellationTokenSource.IsCancellationRequested)
                    break;
            }
        }

        public void Shutdown()
        {
            endEvent.Set();
            cancellationTokenSource.Cancel();
            task.Wait(3000);
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(Left), Left);
            bar.SetData(nameof(Top), Top);
            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);
            Left = bar.GetData<double>(nameof(Left));
            Top = bar.GetData<double>(nameof(Top));
        }
    }
}
