using Common;
using Common.NotifyProperty;
using OpenCLWrapper;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniCamV2.Content.View;
using UniCamV2.Tools;


namespace UniCamV2.Content.Model
{
    public class TaskListDC
    {
        public event Action<object, UserControl> SetMainContent;


        public INonSettableObservableProperty<AreaDC[]> Areas { get; } = new ObservableProperty<AreaDC[]>(new AreaDC[0]);
        public ISettableObservableProperty<AreaDC> SelectedArea { get; } = new ObservableProperty<AreaDC>();


        readonly ImagePreparator imagePreparator;
        readonly MainDC mainDC;
        readonly OpenCLAccelerator openCLAccelerator;


        public TaskListDC(ImagePreparator imagePreparator, MainDC mainDC, OpenCLAccelerator openCLAccelerator)
        {
            this.imagePreparator = imagePreparator;
            this.mainDC = mainDC;
            this.openCLAccelerator = openCLAccelerator;

            SelectedArea.ValueChanged += SelectedArea_ValueChanged;
        }

        private void SelectedArea_ValueChanged(AreaDC oldValue, AreaDC newValue)
        {
            if (newValue.IsNotNull())
                newValue.Selected();
        }

        internal void AddNewArea()
        {
            AreaDC newAreaDC = new AreaDC(imagePreparator, mainDC.UserScreen, openCLAccelerator);
            Areas.ForceSet(Areas.Value.Add(newAreaDC));
        }

        internal void RemoveSelectedArea()
        {
            if (SelectedArea.Value.IsNotNull())
            {
                Areas.ForceSet(Areas.Value.Remove(SelectedArea.Value));
                SelectedArea.Value = null;
            }
        }

        internal void RemoveAllAreas() => Areas.ForceSet(Areas.Value.Clear());

        internal void ShowAreaProperties()
        {
            SetMainContent?.Invoke(this, new AreaV() { DataContext = SelectedArea.Value });
            SelectedArea.Value.ShowAreaProperties();
        }

        internal void MainDisplayMouseDown(object sender, Point position, MouseButtonEventArgs e)
        {
            if (SelectedArea.Value.IsNotNull())
                SelectedArea.Value.MainDisplayMouseDown(sender, position, e);
        }

        internal void MainDisplayMouseMove(object sender, Point position, Vector moveVector, MouseEventArgs e)
        {
            if (SelectedArea.Value.IsNotNull())
                SelectedArea.Value.MainDisplayMouseMove(sender, position, moveVector, e);
        }

        internal void MainDisplayMouseUp(object sender, Point position, MouseButtonEventArgs e)
        {
            if (SelectedArea.Value.IsNotNull())
                SelectedArea.Value.MainDisplayMouseUp(sender, position, e);
        }

        internal void NewImage()
        {

        }
    }
}
