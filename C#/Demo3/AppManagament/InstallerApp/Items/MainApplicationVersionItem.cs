using Common;
using Common.Language;
using Common.License;
using Common.NotifyProperty;
using System.Collections.Generic;


namespace InstallerApp.Items
{
    public class MainApplicationVersionItem
    {
        public IProperty<bool> IsSelected { get; } = new Property<bool>();
        public string Version { get; }
        public string Major { get; }
        public string Minor { get; }
        public string Build { get; }
        public ToolApplicationItem[] ToolApps { get; }


        public MainApplicationVersionItem(LanguageDC languageDC, ApplicationClient installerClient, string anName, string major, string minor, string build)
        {
            Version = string.Format("V{0}.{1}.{2}", major, minor, build);
            Major = major;
            Minor = minor;
            Build = build;

            installerClient.SendTextArray(Constants.GetToolApps, anName, major, minor, build);
            string[] toolAppNames = installerClient.ReceiveTextArray();

            List<ToolApplicationItem> toolApps = new List<ToolApplicationItem>();
            foreach (string toolAppName in toolAppNames)
                toolApps.Add(new ToolApplicationItem(languageDC, installerClient, anName, major, minor, build, toolAppName));

            ToolApps = toolApps.ToArray();
        }
    }
}
