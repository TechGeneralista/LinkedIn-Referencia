using CommonLib.Components;
using ComponentCheckApp.Views.Reference;
using D4I4OLib;
using ImageProcessLib.OpenCL;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ComponentCheckApp.Views.ShapeFinder
{
    public class ShapeFinderViewModel : ObservableProperty
    {
        public WriteableBitmap Source
        {
            get => source;
            private set => SetField(value, ref source);
        }

        public OCLShapeBorderCrop OCLBorderCrop { get; private set; }

        public ObservableCollection<ReferenceViewModel> Items { get; private set; } = new ObservableCollection<ReferenceViewModel>();
        public ReferenceViewModel SelectedItem
        {
            get => selectedItem;
            set => SetField(value, ref selectedItem);
        }


        WriteableBitmap source;
        ReferenceViewModel selectedItem;
        D4I4O d4I4O;


        public ShapeFinderViewModel()
        {
            Application.Current.Resources[ResourceKeys.ShapeFinderViewModelKey] = this;

            OpenCLDevice openCLDevice = (OpenCLDevice)Application.Current.Resources[ResourceKeys.OpenCLDeviceKey];
            OCLBorderCrop = new OCLShapeBorderCrop(openCLDevice);

            d4I4O = (D4I4O)Application.Current.Resources[ResourceKeys.D4I4OKey];
        }

        public void NewImage(WriteableBitmap input)
        {
            Source = input;

            foreach (ReferenceViewModel svm in Items)
                svm.Find(input);

            if(Items.Count == 0)
            {
                d4I4O.Out0 = false;
                d4I4O.Out1 = true;
            }
            else
            {
                if(Items[0].BorderColor == Colors.Green.ToString())
                {
                    d4I4O.Out0 = true;
                    d4I4O.Out1 = false;
                }
                else
                {
                    d4I4O.Out0 = false;
                    d4I4O.Out1 = true;
                }
            }
        }

        public void Create()
        {
            if (Items.Count == 1)
                return;

            OCLBorderCrop.InputImage = source;
            ReferenceViewModel shapeViewModel = new ReferenceViewModel(OCLBorderCrop.OutputImage);
            Items.Add(shapeViewModel);
        }

        public void RemoveSelected()
        {
            if (selectedItem is null)
                return;

            Items.Remove(selectedItem);
            SelectedItem = null;
        }
    }
}