using Common.NotifyProperty;
using System;


namespace ImageProcess.Modules
{
    public class ModuleInformation
    {
        public bool IsEnable { get; }
        public INonSettableObservableProperty<string> Name { get; }
        public string TypeName { get; }


        public ModuleInformation(string typeName, INonSettableObservableProperty<string> name, bool isEnable)
        {
            IsEnable = isEnable;
            Name = name;
            TypeName = typeName;
        }
    }
}