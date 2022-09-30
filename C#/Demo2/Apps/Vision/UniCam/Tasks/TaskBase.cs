using Common.NotifyProperty;
using ImageProcess.Buffers;
using ImageProcess.Operations;
using OpenCLWrapper;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using ImageProcess;
using System;

namespace UniCamApp.Tasks
{
    public abstract class TaskBase : ITask
    {
        public string TypeName { get; protected set; } = nameof(TaskBase);
        public ISettableObservableProperty<string> Id { get; } = new ObservableProperty<string>();
        public WriteableBitmapBuffer LastInputImageBuffer { get; private set; }
        public SelectionRectangle SelectionRectangle { get; } = new SelectionRectangle();
        public INonSettableObservableProperty<WriteableBitmap> ResultImage { get; } = new ObservableProperty<WriteableBitmap>();
        public INonSettableObservableProperty<bool> Result { get; } = new ObservableProperty<bool>();
        public UserControl PropertiesView { get; protected set; }
        public ColorPicker8 ResultDrawBackColor { get; } = new ColorPicker8();
        public ISettableObservableProperty<bool> IsMonochrome { get; } = new ObservableProperty<bool>();


        protected readonly Color selectionColorFill = Color.FromArgb(64, 255, 165, 0);
        protected readonly Color selectionColorOutline = Color.FromArgb(255, 255, 165, 0);
        protected readonly Color okColorFill = Color.FromArgb(64, 0, 255, 0);
        protected readonly Color okColorOutline = Color.FromArgb(255, 0, 255, 0);
        protected readonly Color nokColorFill = Color.FromArgb(64, 255, 0, 0);
        protected readonly Color nokColorOutline = Color.FromArgb(255, 255, 0, 0);
        readonly int selectionRectangleBorderThickness = 5;
        protected readonly Crop crop;
        protected readonly Monochrome monochrome;
        protected readonly Draw draw;
        protected readonly object runLock = new object();
        protected WriteableBitmapBuffer preparedImage;


        protected TaskBase(OpenCLAccelerator accelerator)
        {
            Id.Value = Utils.NewGuid(10);
            Id.ValueChanged += Id_ValueChanged;

            crop = new Crop(accelerator);
            monochrome = new Monochrome(accelerator);
            draw = new Draw(accelerator);

            IsMonochrome.ValueChanged += (o,n) => RefreshPreparedImage();
        }

        private void RefreshPreparedImage()
        {
            lock (runLock)
            {
                SetPreparedImage();
                Start();
            }
        }

        private void Id_ValueChanged(string oldValue, string newValue)
        {
            if(string.IsNullOrEmpty(newValue) || string.IsNullOrWhiteSpace(newValue))
                Id.Value = Utils.NewGuid(10);
        }

        public void Start(WriteableBitmapBuffer source)
        {
            lock(runLock)
            {
                LastInputImageBuffer = source;
                SelectionRectangle.Initialize(new System.Drawing.Size((int)source.Data.Descriptor.Width, (int)source.Data.Descriptor.Height));
                crop.Start(LastInputImageBuffer, SelectionRectangle.Rectangle);
                RefreshPreparedImage();
            }
        }

        private void SetPreparedImage()
        {
            if (!IsMonochrome.Value)
                preparedImage = crop.Output;
            else
            {
                monochrome.Convert(crop.Output);
                preparedImage = monochrome.Output;
            }
        }

        protected virtual void Start() => Debug.WriteLine(nameof(Start) + " of " + nameof(TaskBase) + " not implemented!");

        public void DrawSelecionRectangle(WriteableBitmapBuffer destination, bool isSelected)
        {
            if (isSelected)
                draw.Rectangle(destination, SelectionRectangle.Rectangle, selectionColorFill, selectionColorOutline, selectionRectangleBorderThickness);
            else
            {
                if(Result.Value)
                    draw.Rectangle(destination, SelectionRectangle.Rectangle, okColorFill, okColorOutline, selectionRectangleBorderThickness);
                else
                    draw.Rectangle(destination, SelectionRectangle.Rectangle, nokColorFill, nokColorOutline, selectionRectangleBorderThickness);
            }
        }

        public virtual void GetPropertiesView() => Debug.WriteLine(nameof(GetPropertiesView) + " of " + nameof(TaskBase) + " not implemented!");
    }
}
