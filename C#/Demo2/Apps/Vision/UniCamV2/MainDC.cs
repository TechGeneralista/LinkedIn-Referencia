using Common.NotifyProperty;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniCamV2.Content.Model;
using UniCamV2.Content.View;
using UniCamV2.Trigger;
using UVC;
using UVC.Internals;


namespace UniCamV2
{
    public class MainDC
    {
        public event Action<object, Point, MouseButtonEventArgs> MainDisplayMouseDown;
        public event Action<object, Point, Vector, MouseEventArgs> MainDisplayMouseMove;
        public event Action<object, Point, MouseButtonEventArgs> MainDisplayMouseUp;


        public string Title { get; }
        public UserScreen UserScreen { get; }
        public ISettableObservableProperty<UserControl> CurrentContent { get; } = new ObservableProperty<UserControl>();
        public INonSettableObservableProperty<bool> ShowImageOptimizationContentButtonIsEnable { get; } = new ObservableProperty<bool>();
        public INonSettableObservableProperty<bool> ShowTasksContentButtonIsEnable { get; } = new ObservableProperty<bool>();


        readonly UserControl[] contents;
        readonly TriggerAutoStarter triggerAutoStarter;


        public MainDC(UserControl[] contents, TriggerAutoStarter triggerAutoStarter)
        {
            this.contents = contents;
            this.triggerAutoStarter = triggerAutoStarter;

            Title = "UniCam V2.0";
            UserScreen = new UserScreen();

            ShowImageSourceSelectContent();
        }

        internal void ShowImageSourceSelectContent()
        {
            triggerAutoStarter.Reset(CurrentContent.Value);
            CurrentContent.Value = contents.First(x => x.GetType() == typeof(ImageSourceV));
        }

        internal void ShowImageOptimizationContent()
        {
            CurrentContent.Value = contents.First(x => x.GetType() == typeof(DevicePropertiesV));
            triggerAutoStarter.Start();
        }

        internal void ShowTasksContent()
        {
            triggerAutoStarter.Reset(CurrentContent.Value);
            CurrentContent.Value = contents.First(x => x.GetType() == typeof(TaskListV));
        }

        internal void Enable(object sender, IImageSource device)
        {
            ShowImageOptimizationContentButtonIsEnable.ForceSet(true);
            ShowTasksContentButtonIsEnable.ForceSet(true);
        }

        internal void Disable(object sender, IImageSource device)
        {
            ShowImageOptimizationContentButtonIsEnable.ForceSet(false);
            ShowTasksContentButtonIsEnable.ForceSet(false);
            UserScreen.Display.Value = null;
        }

        internal void DisplayMouseDown(Point mousePositionOnImage, MouseButtonEventArgs e) => MainDisplayMouseDown?.Invoke(this, mousePositionOnImage, e);
        internal void DisplayMouseMove(Point mousePositionOnImage, Vector mouseMoveVectorOnImage, MouseEventArgs e) => MainDisplayMouseMove?.Invoke(this, mousePositionOnImage, mouseMoveVectorOnImage, e);
        internal void DisplayMouseUp(Point mousePositionOnImage, MouseButtonEventArgs e) => MainDisplayMouseUp(this, mousePositionOnImage, e);
    }
}