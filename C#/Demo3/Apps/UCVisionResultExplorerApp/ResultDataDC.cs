using Common.NotifyProperty;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace UCVisionResultExplorerApp
{
    public class ResultDataDC
    {
        public DateTime DateTime { get; }
        public IReadOnlyPropertyArray<ResultDataImageDC> Images { get; } = new PropertyArray<ResultDataImageDC>();
        public ResultDataTextDC[] ResultLines { get; }


        readonly string name;


        public ResultDataDC(string path, int imageHeight)
        {
            name = new DirectoryInfo(path).Name;

            string[] temp = name.Split('_');

            if(temp.Length == 7)
            {
                int year, month, day, hour, minute, second, millisecond;

                try
                {
                    year = int.Parse(temp[0]);
                    month = int.Parse(temp[1]);
                    day = int.Parse(temp[2]);
                    hour = int.Parse(temp[3]);
                    minute = int.Parse(temp[4]);
                    second = int.Parse(temp[5]);
                    millisecond = int.Parse(temp[6]);
                    DateTime = new DateTime(year, month, day, hour, minute, second, millisecond);
                }
                catch { }
            }

            string[] images = Directory.GetFiles(path, "*.jpg");

            foreach (string imageFilePath in images)
                Images.ToSettable().Add(new ResultDataImageDC(imageFilePath, imageHeight));

            string txtResultsFilePath = Path.Combine(path, "Results.txt");

            if(File.Exists(txtResultsFilePath))
            {
                string[] lines = File.ReadAllLines(txtResultsFilePath, Encoding.UTF8);
                List<ResultDataTextDC> resultTexts = new List<ResultDataTextDC>();

                foreach (string line in lines)
                    resultTexts.Add(new ResultDataTextDC(line));

                ResultLines = resultTexts.ToArray();
            }
        }

        internal void SetHeight(int n)
            => Images.ForEach(x => x.SetHeight(n));

        internal bool CheckFilter(DateTime from, DateTime to, string text)
        {
            if (DateTime >= from && DateTime <= to)
            {
                if (string.IsNullOrEmpty(text))
                    return true;
                else
                {
                    foreach (ResultDataTextDC resultDataTextDC in ResultLines)
                    {
                        if (resultDataTextDC.CheckFilter(text))
                            return true;
                    }
                }
            }

            return false;
        }
    }
}