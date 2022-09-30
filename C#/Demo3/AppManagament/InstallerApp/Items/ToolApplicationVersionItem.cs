using Common.NotifyProperty;


namespace InstallerApp.Items
{
    public class ToolApplicationVersionItem
    {
        public IProperty<bool> IsSelected { get; } = new Property<bool>();
        public string Version { get; }
        public string Major { get; }
        public string Minor { get; }
        public string Build { get; }


        public ToolApplicationVersionItem(string toolAvMajor, string toolAvMinor, string toolAvBuild)
        {
            Major = toolAvMajor;
            Minor = toolAvMinor;
            Build = toolAvBuild;
            Version = string.Format("V{0}.{1}.{2}", toolAvMajor, toolAvMinor, toolAvBuild);
        }
    }
}
