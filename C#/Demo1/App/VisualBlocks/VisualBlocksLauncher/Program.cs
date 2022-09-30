using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows;
using VisualBlocks;


namespace VisualBlocksLauncher
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                LoadBin();
                RunApp();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error:", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void LoadBin()
        {
            string[] b64s = DeCompress(File.ReadAllBytes(GetNewestVersion())).Split('_');

            foreach (string b64 in b64s)
                AppDomain.CurrentDomain.Load(Convert.FromBase64String(b64));

            AppDomain.CurrentDomain.AssemblyResolve += (s, e) => AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName == e.Name).FirstOrDefault();
        }

        private static void RunApp()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        private static string GetNewestVersion()
        {
            string[] binFilePaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.bin");

            if (binFilePaths.Length == 0)
                throw new FileNotFoundException("bin");

            List<Version> versions = new List<Version>();
            foreach (string binFilePath in binFilePaths)
                versions.Add(new Version(Path.GetFileNameWithoutExtension(binFilePath)));

            versions.Sort();

            return binFilePaths.Where(filePath => filePath.Contains(versions.Last().ToString())).FirstOrDefault();
        }

        private static string DeCompress(byte[] input)
        {
            using (MemoryStream output = new MemoryStream(input))
            {
                using (DeflateStream deCompressor = new DeflateStream(output, CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(deCompressor, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
