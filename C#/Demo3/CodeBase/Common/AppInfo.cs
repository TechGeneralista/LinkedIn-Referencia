using System;
using System.IO;
using System.Reflection;


namespace Common
{
    public class AppInfo
    {
        public string ManufacturerName { get; }
        public string ApplicationName { get; }
        public int MajorVersion { get; }
        public int MinorVersion { get; }
        public int BuildVersion { get; }
        public string ApplicationNameVersion { get; }
        public string ApplicationDataPath { get; }
        public string ConfigFilePath { get; }


        readonly string configFileName = "config.bin";


        public AppInfo(string manufacturerName, string applicatonName, Version version)
        {
            ManufacturerName = manufacturerName;
            ApplicationName = applicatonName;
            MajorVersion = version.Major;
            MinorVersion = version.Minor;
            BuildVersion = version.Build;

            ApplicationNameVersion = string.Format("{0} V{1}.{2}.{3}", ApplicationName, MajorVersion, MinorVersion, BuildVersion);
            ApplicationDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ManufacturerName, ApplicationNameVersion);
            ConfigFilePath = Path.Combine(ApplicationDataPath, configFileName);
        }
    }
}