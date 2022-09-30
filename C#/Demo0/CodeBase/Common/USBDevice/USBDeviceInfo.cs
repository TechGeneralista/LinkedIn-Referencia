using System.Management;


namespace Common.USBDevice
{
    public class USBDeviceInfo
    {
        public string Availability { get; }
        public string Caption { get; }
        public string ClassCode { get; }
        public uint ConfigManagerErrorCode { get; }
        public bool ConfigManagerUserConfig { get; }
        public string CreationClassName { get; }
        public string CurrentAlternateSettings { get; }
        public string CurrentConfigValue { get; }
        public string Description { get; }
        public string DeviceID { get; }
        public string ErrorCleared { get; }
        public string ErrorDescription { get; }
        public string GangSwitched { get; }
        public string InstallDate { get; }
        public string LastErrorCode { get; }
        public string Name { get; }
        public string NumberOfConfigs { get; }
        public string NumberOfPorts { get; }
        public string PNPDeviceID { get; }
        public string PowerManagementCapabilities { get; }
        public string PowerManagementSupported { get; }
        public string ProtocolCode { get; }
        public string Status { get; }
        public string StatusInfo { get; }
        public string SubclassCode { get; }
        public string SystemCreationClassName { get; }
        public string SystemName { get; }
        public string USBVersion { get; }


        public USBDeviceInfo(ManagementObject device)
        {
            Availability = (string)device.GetPropertyValue("Availability");
            Caption = (string)device.GetPropertyValue("Caption");
            ClassCode = (string)device.GetPropertyValue("ClassCode");
            ConfigManagerErrorCode = (uint)device.GetPropertyValue("ConfigManagerErrorCode");
            ConfigManagerUserConfig = (bool)device.GetPropertyValue("ConfigManagerUserConfig");
            CreationClassName = (string)device.GetPropertyValue("CreationClassName");
            CurrentAlternateSettings = (string)device.GetPropertyValue("CurrentAlternateSettings");
            CurrentConfigValue = (string)device.GetPropertyValue("CurrentConfigValue");
            Description = (string)device.GetPropertyValue("Description");
            DeviceID = (string)device.GetPropertyValue("DeviceID");
            ErrorCleared = (string)device.GetPropertyValue("ErrorCleared");
            ErrorDescription = (string)device.GetPropertyValue("ErrorDescription");
            GangSwitched = (string)device.GetPropertyValue("GangSwitched");
            InstallDate = (string)device.GetPropertyValue("InstallDate");
            LastErrorCode = (string)device.GetPropertyValue("LastErrorCode");
            Name = (string)device.GetPropertyValue("Name");
            NumberOfConfigs = (string)device.GetPropertyValue("NumberOfConfigs");
            NumberOfPorts = (string)device.GetPropertyValue("NumberOfPorts");
            PNPDeviceID = (string)device.GetPropertyValue("PNPDeviceID");
            PowerManagementCapabilities = (string)device.GetPropertyValue("PowerManagementCapabilities");
            PowerManagementSupported = (string)device.GetPropertyValue("PowerManagementSupported");
            ProtocolCode = (string)device.GetPropertyValue("ProtocolCode");
            Status = (string)device.GetPropertyValue("Status");
            StatusInfo = (string)device.GetPropertyValue("StatusInfo");
            SubclassCode = (string)device.GetPropertyValue("SubclassCode");
            SystemCreationClassName = (string)device.GetPropertyValue("SystemCreationClassName");
            SystemName = (string)device.GetPropertyValue("SystemName");
            USBVersion = (string)device.GetPropertyValue("USBVersion");
        }
    }
}
