using CommonLib.Components;
using ImageProcessLib.OpenCL;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ComponentCheckApp.Views.ReferenceEditor
{
    public class ReferenceEditorViewModel : ObservableProperty
    {
        public WriteableBitmap ReferenceImage
        {
            get => referenceImage;
            private set => SetField(value, ref referenceImage);
        }

        public int EditSize
        {
            get => editSize;
            set => SetField(value, ref editSize);
        }


        WriteableBitmap referenceImage;
        ImagePositionConverter imagePositionConverter = new ImagePositionConverter();
        OCLDraw oclDraw;
        int editSize;


        public ReferenceEditorViewModel(WriteableBitmap referenceImage)
        {
            ReferenceImage = referenceImage;

            OpenCLDevice openCLDevice = (OpenCLDevice)Application.Current.Resources[ResourceKeys.OpenCLDeviceKey];
            oclDraw = new OCLDraw(openCLDevice);
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            imagePositionConverter.GetPosition((Image)sender, e);

            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                oclDraw.InputImage = referenceImage;
                oclDraw.Start();

                if (editSize == 0)
                {
                    if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Released)
                        oclDraw.Pixel((int)Math.Round(imagePositionConverter.Position.X, 0), (int)Math.Round(imagePositionConverter.Position.Y, 0), Colors.White);
                    else if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed)
                        oclDraw.Pixel((int)Math.Round(imagePositionConverter.Position.X, 0), (int)Math.Round(imagePositionConverter.Position.Y, 0), Colors.Black);
                } 
                else
                {
                    int fromY = (int)(imagePositionConverter.Position.Y - editSize);
                    int fromX = (int)(imagePositionConverter.Position.X - editSize);
                    int toY = (int)(imagePositionConverter.Position.Y + editSize);
                    int toX = (int)(imagePositionConverter.Position.X + editSize);

                    for (int y = fromY; y < toY; y++)
                    {
                        for (int x = fromX; x < toX; x++)
                        {
                            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Released)
                                oclDraw.Pixel(x, y, Colors.White);
                            else if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed)
                                oclDraw.Pixel(x, y, Colors.Black);
                        }
                    }
                }

                oclDraw.End();
                ReferenceImage = oclDraw.DrawedImage;
            }
        }
    }
}
