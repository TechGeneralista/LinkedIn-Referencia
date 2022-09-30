using CommonLib.Components;
using ComponentCheckApp.Views.ReferenceEditor;
using ComponentCheckApp.Views.Shape;
using ImageProcessLib.Components;
using ImageProcessLib.OpenCL;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ComponentCheckApp.Views.Reference
{
    public class ReferenceViewModel : ObservableProperty
    {
        public WriteableBitmap ReferenceImage
        {
            get => referenceImage;
            private set => SetField(value, ref referenceImage);
        }

        public OCLShapePixelCounter OCLShapePixelCounter { get; private set; }

        public int AngleTolerance
        {
            get => angleTolerance;
            set => SetField(value, ref angleTolerance);
        }

        public ShapeViewModel[] ShapeViewModels
        {
            get => shapeViewModels;
            private set => SetField(value, ref shapeViewModels);
        }

        public int MinimumMatch
        {
            get => minimumMatch;
            set => SetField(value, ref minimumMatch);
        }

        public string BorderColor
        {
            get => borderColor;
            private set => SetField(value, ref borderColor);
        }

        public ShapeViewModel BestMatch
        {
            get => bestMatch;
            private set => SetField(value, ref bestMatch);
        }


        string borderColor;
        ShapeViewModel[] shapeViewModels;
        ShapeViewModel bestMatch;
        int angleTolerance;
        WriteableBitmap referenceImage;
        OCLShapeBorderCrop oclShapeBorderCrop;
        ShapeRotate oclShapeRotate;
        int minimumMatch;


        public ReferenceViewModel(WriteableBitmap referenceImage)
        {
            ReferenceImage = referenceImage;
            borderColor = Colors.Orange.ToString();

            OpenCLDevice openCLDevice = (OpenCLDevice)Application.Current.Resources[ResourceKeys.OpenCLDeviceKey];
            oclShapeBorderCrop = new OCLShapeBorderCrop(openCLDevice);
            OCLShapePixelCounter = new OCLShapePixelCounter(openCLDevice);

            angleTolerance = 0;
            oclShapeRotate = new ShapeRotate();
            minimumMatch = 50;

            OCLShapePixelCounter.Start(referenceImage);
            oclShapeRotate.ReferenceImage = referenceImage;
            oclShapeRotate.Create(angleTolerance);
            RefreshShapeViewModels();
        }

        public void Find(WriteableBitmap input)
        {
            MainViewModel mainViewModel = ((MainViewModel)Application.Current.Resources[ResourceKeys.MainViewModelKey]);
            BorderColor = Colors.Orange.ToString();
            BestMatch = null;

            foreach (ShapeViewModel vm in shapeViewModels)
                vm.Reset();

            for(int i=0;i<shapeViewModels.Length;i++)
            {
                mainViewModel.SetProgressBar(i, shapeViewModels.Length);
                shapeViewModels[i].Find(input, minimumMatch);
            }

            ShapeViewModel best = shapeViewModels[0];

            foreach (ShapeViewModel vm in shapeViewModels)
            {
                if (vm.Match > best.Match)
                    best = vm;
            }

            BestMatch = best;

            if (best.Match >= minimumMatch)
                BorderColor = Colors.Green.ToString();
            else
                BorderColor = Colors.Red.ToString();
        }

        public void MouseDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                ReferenceEditorView referenceEditorView = new ReferenceEditorView(referenceImage);
                referenceEditorView.ShowDialog();
                oclShapeBorderCrop.InputImage = referenceEditorView.vm.ReferenceImage;
                ReferenceImage = oclShapeBorderCrop.OutputImage;

                OCLShapePixelCounter.Start(referenceImage);
                oclShapeRotate.ReferenceImage = referenceImage;
                oclShapeRotate.Create(angleTolerance);
                RefreshShapeViewModels();
            }
        }

        private void RefreshShapeViewModels()
        {
            List<ShapeViewModel> vms = new List<ShapeViewModel>();

            for (int i = 0; i < oclShapeRotate.Shapes.Length; i++)
            {
                vms.Add(new ShapeViewModel(oclShapeRotate.Shapes[i], oclShapeRotate.Angles[i]));
            }

            ShapeViewModels = vms.ToArray();
        }

        public void SliderDragCompleted()
        {
            oclShapeRotate.Create(angleTolerance);
            RefreshShapeViewModels();
        }
    }
}