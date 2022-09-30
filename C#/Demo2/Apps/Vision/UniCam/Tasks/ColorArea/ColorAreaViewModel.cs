using Common.NotifyProperty;
using ImageProcess.Buffers;
using ImageProcess.Operations;
using OpenCLWrapper;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace UniCamApp.Tasks.ColorArea
{
    public class ColorAreaViewModel : TaskBase
    {
        public ISettableObservableProperty<uint> BlurSize { get; } = new ObservableProperty<uint>(3);
        public ObservableCollection<Color> RegisteredColors { get; } = new ObservableCollection<Color>();
        public ISettableObservableProperty<uint> ColorTolerance { get; } = new ObservableProperty<uint>(16);
        public ISettableObservableProperty<uint> DilateSize { get; } = new ObservableProperty<uint>(3);
        public ISettableObservableProperty<uint> ErodeSize { get; } = new ObservableProperty<uint>(3);
        public INonSettableObservableProperty<uint> DetectedValue { get; } = new ObservableProperty<uint>();
        public ISettableObservableProperty<int> MinimumLimit { get; } = new ObservableProperty<int>(0);
        public ISettableObservableProperty<int> MinimumRange { get; } = new ObservableProperty<int>(25);
        public ISettableObservableProperty<int> Registered { get; } = new ObservableProperty<int>(50);
        public ISettableObservableProperty<int> MaximumRange { get; } = new ObservableProperty<int>(75);
        public ISettableObservableProperty<int> MaximumLimit { get; } = new ObservableProperty<int>(100);
        public ISettableObservableProperty<bool> IsCountQuantity { get; } = new ObservableProperty<bool>();


        readonly Blur blur;
        readonly ColorDetector colorDetector;
        readonly Dilate dilate;
        readonly Erode erode;
        readonly NonBlackPixelCounter nonBlackPixelCounter;
        readonly WriteableBitmapBuffer croppedImageCopy;


        public ColorAreaViewModel(OpenCLAccelerator accelerator) : base(accelerator) 
        {
            TypeName = "Színterület";

            blur = new Blur(accelerator);
            BlurSize.ValueChanged += (o,n) => StartAndRegister();

            colorDetector = new ColorDetector(accelerator);
            ColorTolerance.ValueChanged += (o,n) => StartAndRegister();

            dilate = new Dilate(accelerator);
            DilateSize.ValueChanged += (o,n) => StartAndRegister();

            erode = new Erode(accelerator);
            ErodeSize.ValueChanged += (o,n) => StartAndRegister();

            nonBlackPixelCounter = new NonBlackPixelCounter(accelerator);
            croppedImageCopy = new WriteableBitmapBuffer(accelerator);

            PropertiesView = new ColorAreaView() { DataContext = this };
            ResultDrawBackColor.SelectedColor.ValueChanged += (o,n) => Start();

            Registered.ValueChanged += SetRange;
            IsCountQuantity.ValueChanged += (o,n) => StartAndRegister();
        }

        private void SetRange(int oldValue, int newValue)
        {
            if(!IsCountQuantity.Value)
            {
                MinimumLimit.Value = 0;
                MinimumRange.Value = (int)((double)Registered.Value * (double)0.75);
                MaximumRange.Value = (int)((double)Registered.Value * (double)1.25);
                MaximumLimit.Value = (int)Registered.Value * 2;
            }
            else
            {
                MinimumLimit.Value = 0;
                MinimumRange.Value = 1;
                MaximumRange.Value = 1;
                MaximumLimit.Value = 100;
            }
        }

        private void StartAndRegister()
        {
            lock (runLock)
            {
                Start();
                Registered.Value = (int)nonBlackPixelCounter.CountedPixels;
            }
        }

        protected override void Start()
        {
            lock(runLock)
            {
                blur.Create(preparedImage, BlurSize.Value);
                colorDetector.Start(blur.Output, RegisteredColors.ToArray(), ColorTolerance.Value);

                dilate.Start(colorDetector.Output, DilateSize.Value);
                erode.Start(dilate.Output, ErodeSize.Value);

                nonBlackPixelCounter.Count(erode.Output);

                if (!IsCountQuantity.Value)
                    DetectedValue.ForceSet(nonBlackPixelCounter.CountedPixels);
                else
                    DetectedValue.ForceSet((uint)Math.Round((double)nonBlackPixelCounter.CountedPixels / (double)Registered.Value, 0));

                croppedImageCopy.CreateCopyFrom(preparedImage);
                draw.Mask(erode.Output, ResultDrawBackColor.SelectedColor.Value, croppedImageCopy);

                ResultImage.ForceSet(croppedImageCopy.Upload());
            }
        }

        internal void ImageMouseDown(System.Drawing.Point position)
        {
            lock(runLock)
            {
                Color pickedColor = GetColorFromCroppedImage(position);

                int count = RegisteredColors.Where(c => c.R == pickedColor.R && c.G == pickedColor.G && c.B == pickedColor.B).Count();

                if (count > 0)
                    return;

                RegisteredColors.Add(pickedColor);
                StartAndRegister();
            }
        }

        internal void ClearRegisteredColorsList()
        {
            RegisteredColors.Clear();
            Start();
        }

        private unsafe Color GetColorFromCroppedImage(System.Drawing.Point position)
        {
            WriteableBitmap croppedImage = preparedImage.Upload();
            byte* ptr = ((byte*)croppedImage.BackBuffer.ToPointer()) + (position.X * croppedImage.Format.BitsPerPixel / 8) + (position.Y * croppedImage.BackBufferStride);
            return Color.FromRgb(*(ptr + 2), *(ptr + 1), *(ptr));
        }
    }
}
