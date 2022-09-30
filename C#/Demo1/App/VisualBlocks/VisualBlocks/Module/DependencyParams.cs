using Common.NonGenerics;
using Compute;
using System;
using System.Windows;
using VisualBlocks.Project;


namespace VisualBlocks.Module
{
    internal class DependencyParams
    {
        public Window MainWindow { get; }
        public ThreadSafeObservableCollection Items { get; private set; }
        public ComputeAccelerator ComputeAccelerator { get; }


        readonly ProjectDC projectDC;


        public DependencyParams(Window mainWindow, ThreadSafeObservableCollection items, ProjectDC projectDC, ComputeAccelerator computeAccelerator)
        {
            MainWindow = mainWindow;
            Items = items;
            this.projectDC = projectDC;
            ComputeAccelerator = computeAccelerator;
        }

        internal void NotifyProjectChanged()
            => projectDC.IsModified = true;

        internal void ChangeItems(ThreadSafeObservableCollection items)
            => Items = items;
    }
}
