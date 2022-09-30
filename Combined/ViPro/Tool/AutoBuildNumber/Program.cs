using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System;
using System.Linq;

namespace AutoBuildNumber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string prefix = "        public readonly static int Number = ";
            string suffix = ";";
            string file = "Build.cs";

            try
            {
                List<string> lines = new List<string>(File.ReadAllLines(file, Encoding.UTF8));
                int indexOfBuildNumber = lines.IndexOf(lines.Where(x => x.Contains(prefix)).ElementAt(0));
                string buildLine = lines[indexOfBuildNumber];
                buildLine = buildLine.Replace(prefix, string.Empty).Replace(suffix, string.Empty);
                int buildNumber = int.Parse(buildLine) + 1;
                lines[indexOfBuildNumber] = $"{prefix}{buildNumber}{suffix}";
                File.WriteAllLines(file, lines, Encoding.UTF8);
                Console.WriteLine($"Next build number of {lines[0].Replace("namespace ", string.Empty)}: {buildNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}