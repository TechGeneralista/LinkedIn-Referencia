using Common.NotifyProperty;
using System.Collections.Generic;
using System.Windows.Media;


namespace Common
{
    public class ColorPicker8
    {
        public Color[] Colors { get; }
        public ISettableObservableProperty<Color> SelectedColor { get; } = new ObservableProperty<Color>();


        public ColorPicker8()
        {
            List<Color> temp = new List<Color>();
            temp.Add(Color.FromRgb(255, 0, 0));
            temp.Add(Color.FromRgb(0, 255, 0));
            temp.Add(Color.FromRgb(255, 255, 0));
            temp.Add(Color.FromRgb(0, 0, 255));
            temp.Add(Color.FromRgb(255, 0, 255));
            temp.Add(Color.FromRgb(0, 255, 255));
            temp.Add(Color.FromRgb(255, 255, 255));
            temp.Add(Color.FromRgb(0, 0, 0));
            Colors = temp.ToArray();

            SelectedColor.Value = Colors[0];
        }
    }
}