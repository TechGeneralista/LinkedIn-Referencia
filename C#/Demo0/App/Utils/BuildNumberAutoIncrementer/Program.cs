using System;
using System.IO;
using System.Text;


namespace BuildNumberAutoIncrementer
{
    class Program
    {
        //[assembly: AssemblyVersion("1.0.0.0")]

        static readonly string startString = "[assembly: AssemblyVersion(\"";
        static readonly string endString = "\")]";

        static void Main(string[] args)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AssemblyInfo.cs");
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            int lineIndex = -1;

            for(int i=0;i<lines.Length;i++)
            {
                string line = lines[i];

                if(line.StartsWith(startString) && line.EndsWith(endString))
                {
                    lineIndex = i;
                    break;
                }
            }

            if (lineIndex == -1)
                return;

            string[] numbers = lines[lineIndex].Replace(startString, string.Empty).Replace(endString, string.Empty).Split('.');
            int buildInfo = int.Parse(numbers[2]) + 1;

            lines[lineIndex] = string.Format("{0}{1}.{2}.{3}.{4}{5}",startString,numbers[0],numbers[1], buildInfo, numbers[3], endString);

            File.WriteAllLines(filePath, lines);
        }
    }
}
