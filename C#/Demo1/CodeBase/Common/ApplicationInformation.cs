using System;
using System.Collections.Generic;
using System.Reflection;


namespace Common
{
    public class ApplicationInformation
    {
        public string CompanyName { get; }
        public string ApplicationName { get; }
        public int MajorVersion { get; }
        public int MinorVersion { get; }
        public int BuildVersion { get; }
        public string ApplicationTitle { get; }
        public IEnumerable<string> SettingsCollectionRootKey { get; }


        public ApplicationInformation(Assembly assembly)
        {
            CompanyName = GetAssemblyAttribute<AssemblyCompanyAttribute>(assembly).Company;
            ApplicationName = GetAssemblyAttribute<AssemblyProductAttribute>(assembly).Product;

            Version version = assembly.GetName().Version;
            MajorVersion = version.Major;
            MinorVersion = version.Minor;
            BuildVersion = version.Build;

            ApplicationTitle = string.Format("{0} {1} V{2}.{3}.{4}", CompanyName, ApplicationName, MajorVersion, MinorVersion, BuildVersion);
            SettingsCollectionRootKey = new string[]
            {
                CompanyName.RemoveSpace(),
                ApplicationName.RemoveSpace()
            };
        }

        private static T GetAssemblyAttribute<T>(Assembly assembly) where T : Attribute
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(T), true);

            if ((attributes == null) || (attributes.Length == 0))
                return null;

            return (T)attributes[0];
        }
    }
}
