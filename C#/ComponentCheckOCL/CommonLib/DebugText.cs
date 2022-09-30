using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace CommonLib
{
    public class DebugText
    {
        readonly string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "startupLog.txt");


        public DebugText()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }


        public void NewDebugInfo(string text)
        {
            List<string> log;

            if(File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                log = new List<string>(lines);
            }
            else
            {
                log = new List<string>();
            }

            log.Add(text);
            
            try
            {
                File.WriteAllLines(filePath, log.ToArray());
            }
            catch { }
        }
    }
}
