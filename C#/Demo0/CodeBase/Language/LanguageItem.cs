using System;
using System.Windows.Media.Imaging;


namespace Language
{
    public class LanguageItem
    {
        public BitmapSource Icon { get; }
        public string Name { get; }


        public LanguageItem(string iconName, string name)
        {
            Icon = new BitmapImage(new Uri(string.Format("Images/{0}Flag.png", iconName), UriKind.Relative));
            Name = name;
        }
    }
}
