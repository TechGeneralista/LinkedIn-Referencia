using Common;
using Common.NonGenerics;
using Common.SettingBackupAndRestore;
using Compute;
using CustomControl.VisualEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VisualBlocks.Module;
using VisualBlocks.Module.Base;
using VisualBlocks.Module.ImageSource;
using VisualBlocks.Project;


namespace VisualBlocks.VisualEditor
{
    internal class VisualEditorDC : DCBase, ICanBackupAndRestore
    {
        public double X
        {
            get => x;
            set
            {
                dependencyParams.NotifyProjectChanged();
                SetField(ref x, value);
            }
        }
        double x = 0;

        public double Y
        {
            get => y;
            set
            {
                dependencyParams.NotifyProjectChanged();
                SetField(ref y, value);
            }
        }
        double y = 0;

        public double Scale
        {
            get => scale;
            set
            {
                dependencyParams.NotifyProjectChanged();
                SetField(ref scale, value);
            }
        }
        double scale = 1;

        public ThreadSafeObservableCollection Items { get; } = new ThreadSafeObservableCollection();


        readonly DependencyParams dependencyParams;
        readonly ProjectDC projectDC;
        Dictionary<string, object> clipboardContainer;


        public VisualEditorDC(ComputeAccelerator computeAccelerator, ProjectDC projectDC, Window window)
        {
            dependencyParams = new DependencyParams(window, Items, projectDC, computeAccelerator);
            this.projectDC = projectDC;
        }

        internal void BlocksEditor_NewDataConnection(NewDataConnectionAddedEventArgs e)
        {
            if(e.SourceOutput.GetType() is Type sourceType &&
               e.DestinationInput.GetType() is Type destinationType &&
               sourceType.IsGenericType && destinationType.IsGenericType &&
               sourceType.GetGenericArguments() is Type[] sourceGenericTypes &&
               destinationType.GetGenericArguments() is Type[] destinationGenericTypes &&
               sourceGenericTypes.SequenceEqual(destinationGenericTypes))
            {
                lock (Items.Lock)
                    Items.Add(Activator.CreateInstance(typeof(BlockItemDataConnectionDC<>).MakeGenericType(sourceGenericTypes), new[] { dependencyParams, e.SourceOutput, e.DestinationInput }));

                dependencyParams.NotifyProjectChanged();
            }
        }

        internal void BlocksEditor_NewTriggerConnection(NewTriggerConnectionAddedEventArgs e)
        {
            if (e.SourceOutput is BlockItemTriggerOutputDC blockItemTriggerOutputDC &&
                e.DestinationInput is BlockItemTriggerInputDC blockItemTriggerInputDC)
            {
                lock (Items.Lock)
                    Items.Add(new BlockItemTriggerConnectionDC(dependencyParams, blockItemTriggerOutputDC, blockItemTriggerInputDC));
                
                dependencyParams.NotifyProjectChanged();
            }
        }

        internal void RemoveItems(RemoveItemsEventArgs e)
        {
            lock(Items.Lock)
            {
                foreach(object itemToRemove in e.ItemsToRemove)
                {
                    if (itemToRemove is IShutdownable canShutdown)
                        canShutdown.Shutdown();

                    Items.Remove(itemToRemove);
                }
            }

            dependencyParams.NotifyProjectChanged();
        }

        internal void KeyPressOrReleaseOccurred(KeyEventArgs e, Point? currentMousePosition)
        {
            HandleCopyPaste(e, currentMousePosition);
            SendKeyPress(e);
        }

        private void SendKeyPress(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                Items.ForEach((blockItemDC) => (blockItemDC as ICanKeyPress)?.KeyPress(e));
                e.Handled = true;
            }
        }

        internal void Shutdown()
        {
            lock(Items.Lock)
            {
                foreach(object item in Items)
                {
                    if (item is IShutdownable canShutdown)
                        canShutdown.Shutdown();
                }
            }
        }

        internal void New()
        {
            Shutdown();

            lock (Items.Lock)
                Items.Clear();

            ResetView();
        }

        public void ResetView()
        {
            X = 0;
            Y = 0;
            Scale = 1;
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(X), X);
            bar.SetData(nameof(Y), Y);
            bar.SetData(nameof(Scale), Scale);
            
            lock(Items.Lock)
                bar.SetList(nameof(Items), Items);

            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            New();

            BackupAndRestore bar = new BackupAndRestore(container);
            X = bar.GetData<double>(nameof(X), 0);
            Y = bar.GetData<double>(nameof(Y), 0);
            Scale = bar.GetData<double>(nameof(Scale), 1);

            lock (Items.Lock)
            {
                bar.GetList(nameof(Items), Items, CreateInstanceMethod);
                Items.ForEach(item => (item as IInitializable)?.Initialize());
            }
        }

        private object CreateInstanceMethod(Type type)
            => Activator.CreateInstance(type, dependencyParams);

        internal void BlocksEditor_ChildRemoved()
            => dependencyParams.NotifyProjectChanged();

        internal void AddNew(Type type)
        {
            object newObject = CreateInstanceMethod(type);

            if (newObject is ICanShowDeviceSelector canShowDeviceSelector)
            {
                if (!canShowDeviceSelector.ShowDeviceSelector())
                    return;
            }

            lock(Items.Lock)
            {
                Items.Add(newObject);
            }
        }

        private void HandleCopyPaste(KeyEventArgs e, Point? currentMousePosition)
        {
            if (!e.Handled)
            {
                if (!e.IsRepeat && e.IsDown && e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.C)
                {
                    ThreadSafeObservableCollection clipBoard = new ThreadSafeObservableCollection();

                    lock(Items.Lock)
                    {
                        foreach (object obj in Items)
                        {
                            if (obj is ISelectable selectable &&
                               selectable.IsSelected)
                            {
                                clipBoard.Add(obj);
                            }
                        }
                    }

                    BackupAndRestore bar = new BackupAndRestore();
                    bar.SetList(nameof(Items), clipBoard);
                    clipboardContainer = bar.Container;

                    e.Handled = true;
                }

                else if (!e.IsRepeat && e.IsDown && e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.V)
                {
                    if(clipboardContainer != null)
                    {
                        PasteToPosition(clipboardContainer, currentMousePosition);
                    }

                    e.Handled = true;
                }
            }
        }

        internal void Import(Point? lastContextMenuOpenPosition)
        {
            Dictionary<string, object> importContainer = projectDC.Import();

            if (importContainer == null)
                return;

            PasteToPosition(importContainer, lastContextMenuOpenPosition);
        }

        private void PasteToPosition(Dictionary<string, object> container, Point? position)
        {
            ThreadSafeObservableCollection restoredItems = new ThreadSafeObservableCollection();
            DependencyParams restoredItemsDependencyParams = new DependencyParams(dependencyParams.MainWindow, restoredItems, projectDC, dependencyParams.ComputeAccelerator);

            BackupAndRestore bar = new BackupAndRestore(container);
            bar.GetList(nameof(Items), restoredItems, (t) => Activator.CreateInstance(t, restoredItemsDependencyParams));

            if (position.HasValue)
            {
                Point leftTopPoint = new Point();
                foreach (object obj in restoredItems)
                {
                    if (obj is BlockItemDC blockItemDC)
                    {
                        if (blockItemDC.Left > leftTopPoint.X)
                            leftTopPoint.X = blockItemDC.Left;

                        if (blockItemDC.Top > leftTopPoint.Y)
                            leftTopPoint.Y = blockItemDC.Top;
                    }
                }

                foreach (object obj in restoredItems)
                {
                    if (obj is BlockItemDC blockItemDC)
                    {
                        if (blockItemDC.Left < leftTopPoint.X)
                            leftTopPoint.X = blockItemDC.Left;

                        if (blockItemDC.Top < leftTopPoint.Y)
                            leftTopPoint.Y = blockItemDC.Top;
                    }
                }


                foreach (object obj in restoredItems)
                {
                    if (obj is BlockItemDC blockItemDC)
                    {
                        blockItemDC.Left = (blockItemDC.Left - leftTopPoint.X) + position.Value.X;
                        blockItemDC.Top = (blockItemDC.Top - leftTopPoint.Y) + position.Value.Y;
                    }
                }
            }

            restoredItemsDependencyParams.ChangeItems(Items);

            lock (Items.Lock)
                Items.AddRange(restoredItems);

            restoredItems.ForEach(item => (item as IInitializable)?.Initialize());
        }
    }
}
