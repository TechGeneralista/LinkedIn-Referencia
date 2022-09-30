using Common.Language;
using Common.NotifyProperty;
using Common.ObservableProperty;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;


namespace Common.Controls.PanZoomCanvasView.ModuleContainer
{
    public class ModuleContainerDC
    {
        #region Static members

        public static ModuleContainerDC GetInstance(object directlyOverMouseDevice)
            => (directlyOverMouseDevice as FrameworkElement)?.DataContext as ModuleContainerDC;

        #endregion

        public LanguageDC LanguageDC { get; }
        public IObservableValue<string> ModuleName { get; } = new ObservableValue<string>();
        public IObservableValue<double> X { get; } = new ObservableValue<double>();
        public IObservableValue<double> Y { get; } = new ObservableValue<double>();
        public double ActualWidth => containerView.ActualWidth;
        public double ActualHeight => containerView.ActualHeight;
        public IObservableCollection<ModuleOutput> Outputs { get; } = new ObservableCollection<ModuleOutput>();
        public IObservableValue<object> Input { get; } = new ObservableCollection<ModuleInput>();


        readonly IObservableCollection<object> modules;
        Point lastPosition;
        readonly string moveRectangleText = "moveRectangle";
        readonly string workSheetText = "workSheet";
        FrameworkElement containerView;
        FrameworkElement workSheet;

        #region Initialize

        public ModuleContainerDC(IObservableCollection<object> modules, LanguageDC languageDC, IReadOnlyProperty<string> triggerColon)
        {
            this.modules = modules;
            LanguageDC = languageDC;

            ModuleName.Value = triggerColon.Value;
            triggerColon.OnValueChanged += (o, n) => ModuleName.Value = n;
        }

        internal void Initialize(FrameworkElement view)
        {
            containerView = view;
            workSheet = view.FindParent<FrameworkElement>(workSheetText);

            X.BeforeValueChanged += (s, e) => 
            {
                if (e.NewValue < 0 ||
                    (e.NewValue + containerView.ActualWidth) > workSheet.ActualWidth)
                {
                    e.ChangeEnabled = false;
                    return;
                }

                foreach (object module in modules.Collection)
                {
                    ModuleContainerDC moduleContainer = module as ModuleContainerDC;

                    if (moduleContainer.IsNull() || moduleContainer == this)
                        continue;

                    if (new Rect(e.NewValue, Y.Value, ActualWidth, ActualHeight).RectOverlap(new Rect(moduleContainer.X.Value, moduleContainer.Y.Value, moduleContainer.ActualWidth, moduleContainer.ActualHeight)))
                    {
                        e.ChangeEnabled = false;
                        return;
                    }
                }
            };

            Y.BeforeValueChanged += (s, e) =>
            {
                if (e.NewValue < 0 ||
                    (e.NewValue + containerView.ActualHeight) > workSheet.ActualHeight)
                {
                    e.ChangeEnabled = false;
                    return;
                }

                foreach (object module in modules.Collection)
                {
                    ModuleContainerDC moduleContainer = module as ModuleContainerDC;

                    if (moduleContainer.IsNull() || moduleContainer == this)
                        continue;

                    if (new Rect(X.Value, e.NewValue, ActualWidth, ActualHeight).RectOverlap(new Rect(moduleContainer.X.Value, moduleContainer.Y.Value, moduleContainer.ActualWidth, moduleContainer.ActualHeight)))
                    {
                        e.ChangeEnabled = false;
                        return;
                    }
                }
            };
        }

        #endregion

        internal void DeleteButtonClick()
            => modules.Remove(this);

        #region Move methods

        public void MouseDown(MouseButtonEventArgs e)
        {
            FrameworkElement interactElement = e.MouseDevice.DirectlyOver as FrameworkElement;

            if (interactElement.IsNotNull() && interactElement.Name == moveRectangleText)
            {
                lastPosition = e.GetPosition(workSheet);
                interactElement.CaptureMouse();
            }
        }

        public void MouseMove(MouseEventArgs e)
        {
            FrameworkElement interactElement = e.MouseDevice.DirectlyOver as FrameworkElement;

            if (interactElement.IsNotNull() && interactElement.Name == moveRectangleText)
            {
                Point newPosition = e.GetPosition(workSheet);

                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    Vector moveVector = newPosition - lastPosition;
                    X.Value += moveVector.X;
                    Y.Value += moveVector.Y;
                    lastPosition = newPosition;
                }
            }
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
            Rectangle rectangle = e.MouseDevice.Captured as Rectangle;

            if (rectangle.IsNotNull() && rectangle.Name == moveRectangleText)
                e.MouseDevice.Capture(null);
        }

        #endregion
    }
}
