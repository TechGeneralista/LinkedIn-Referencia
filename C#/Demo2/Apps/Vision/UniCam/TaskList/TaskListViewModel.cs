using Common;
using Common.MouseTool;
using Common.NotifyProperty;
using ImageProcess.Buffers;
using OpenCLWrapper;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using UniCamApp.Tasks;
using UniCamApp.Tasks.ColorArea;


namespace UniCamApp.TaskList
{
    public class TaskListViewModel
    {
        public ObservableCollection<ITask> TaskList { get; } = new ObservableCollection<ITask>();
        public ISettableObservableProperty<ITask> SelectedTask { get; set; } = new ObservableProperty<ITask>();
        public WriteableBitmapBuffer LastImageBuffer { get; set; }
        public WriteableBitmapBuffer ResultImageBuffer { get; }


        object runLock = new object();


        public TaskListViewModel()
        {
            SelectedTask.ValueChanged += (o, n) => RunSelectedOrAllAndDrawAll();
            ResultImageBuffer = new WriteableBitmapBuffer(ObjectContainer.Get<OpenCLAccelerator>());
        }

        internal void ClearSelectionButtonClick() => SelectedTask.Value = null;

        internal void AddColorAreaV1ButtonClick()
        {
            lock(runLock)
            {
                ColorAreaViewModel colorAreaViewModel = new ColorAreaViewModel(ObjectContainer.Get<OpenCLAccelerator>());
                colorAreaViewModel.Start(LastImageBuffer);
                colorAreaViewModel.SelectionRectangle.Changed += RunSelectedOrAllAndDrawAll;
                TaskList.Add(colorAreaViewModel);
                SelectedTask.Value = colorAreaViewModel;
            }
        }

        internal void ClearSelection()
        {
            if (SelectedTask.Value.IsNotNull())
                SelectedTask.Value = null;
        }

        private void RunSelectedOrAllAndDrawAll()
        {
            lock(runLock)
            {
                ResultImageBuffer.CreateCopyFrom(LastImageBuffer);

                if (SelectedTask.Value.IsNull())
                {
                    foreach (ITask task in TaskList)
                        task.Start(LastImageBuffer);
                }
                else
                {
                    SelectedTask.Value.Start(LastImageBuffer);
                }

                DrawAll();

                ObjectContainer.Get<MainViewModel>().ImageSource.Value = ResultImageBuffer.Upload();
            }
        }

        private void RunAllAndDrawAll()
        {
            lock(runLock)
            {
                ResultImageBuffer.CreateCopyFrom(LastImageBuffer);

                foreach (ITask task in TaskList)
                    task.Start(LastImageBuffer);
                
                DrawAll();

                ObjectContainer.Get<MainViewModel>().ImageSource.Value = ResultImageBuffer.Upload();
            }
        }

        private void DrawAll()
        {
            foreach (ITask task in TaskList)
            {
                if (task == SelectedTask.Value)
                    task.DrawSelecionRectangle(ResultImageBuffer, true);
                else
                    task.DrawSelecionRectangle(ResultImageBuffer, false);
            }
        }

        internal void RunTasks(WriteableBitmapBuffer imageBuffer)
        {
            LastImageBuffer = imageBuffer;
            RunAllAndDrawAll();
        }

        internal void ImageMouseMove(Image sender, MouseEventArgs e)
        {
            if (SelectedTask.Value.IsNotNull())
            {
                ImageMousePosition.GetPosition(sender, e);
                MouseVectorCalculator.Calculate(ImageMousePosition.Position);

                if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
                    SelectedTask.Value.SelectionRectangle.Move(MouseVectorCalculator.Vector);

                if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed)
                    SelectedTask.Value.SelectionRectangle.Resize(MouseVectorCalculator.Vector);
            }
        }

        internal void ImageMouseDown(Image sender, MouseButtonEventArgs e)
        {
            ImageMousePosition.GetPosition(sender, e);
            ITask taskUnderMouse = null;

            foreach(ITask task in TaskList)
            {
                if(task.SelectionRectangle.IsUnderMouse(ImageMousePosition.Position))
                {
                    taskUnderMouse = task;
                    break;
                }
            }

            SelectedTask.Value = taskUnderMouse;

            if (SelectedTask.Value.IsNotNull())
            {
                MouseVectorCalculator.MouseDown(ImageMousePosition.Position);
                e.MouseDevice.Capture(sender);
                TaskListMouseDoubleClick();
            }
            else
                ObjectContainer.Get<MainViewModel>().ShowTasksContentButtonClick();
        }

        internal void DeleteButtonClick()
        {
            lock(runLock)
            {
                if (SelectedTask.Value.IsNotNull())
                    TaskList.Remove(SelectedTask.Value);
            }
        }

        internal void TaskListMouseDoubleClick()
        {
            if (SelectedTask.Value.IsNull())
                return;

            ObjectContainer.Get<MainViewModel>().CurrentContent.Value = SelectedTask.Value.PropertiesView;
        }
    }
}
