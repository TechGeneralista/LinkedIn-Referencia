using Common.NotifyProperty;


namespace UCVisionApp.Inspection
{
    public class ModuleInformation
    {
        public bool IsEnable { get; }
        public IReadOnlyProperty<string> Name { get; }
        public string TypeName { get; }


        public ModuleInformation(string typeName, IReadOnlyProperty<string> name, bool isEnable)
        {
            IsEnable = isEnable;
            Name = name;
            TypeName = typeName;
        }
    }
}