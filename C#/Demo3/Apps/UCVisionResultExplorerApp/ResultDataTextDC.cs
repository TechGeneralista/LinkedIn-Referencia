using System;
using System.Windows.Media;


namespace UCVisionResultExplorerApp
{
    public class ResultDataTextDC
    {
        public string Text { get; }
        public string TextForeground { get; }


        public ResultDataTextDC(string text)
        {
            Text = text;

            if (text.Contains("Ok"))
                TextForeground = Colors.Green.ToString();

            else if (text.Contains("Nok"))
                TextForeground = Colors.Red.ToString();

            else
                TextForeground = Colors.Black.ToString();
        }

        internal bool CheckFilter(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            return Text.ToLower().Contains(text.ToLower());
        }
    }
}
