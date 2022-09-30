using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ImageSourceDevice.UVC
{
    public class UVCDeviceInfo
    {
        public string Location { get; }


        public UVCDeviceInfo(string monikerString)
        {
            try
            {
                string upperMonikerString = monikerString.ToUpper();
                string keyString = @"SYSTEM\CurrentControlSet\Enum\USB";
                string[] subKeys = Registry.LocalMachine.OpenSubKey(keyString).GetSubKeyNames();
                List<string> containsList = new List<string>();

                foreach (string subKey in subKeys)
                {
                    if (upperMonikerString.Contains(subKey))
                        containsList.Add(subKey);
                }

                string selectedSubKey = containsList.OrderByDescending(s => s.Length).First();
                keyString = string.Format(@"{0}\{1}",keyString,selectedSubKey);

                subKeys = Registry.LocalMachine.OpenSubKey(keyString).GetSubKeyNames();
                containsList = new List<string>();

                foreach (string subKey in subKeys)
                {
                    if (monikerString.Contains(subKey))
                        containsList.Add(subKey);
                }

                selectedSubKey = containsList.OrderByDescending(s => s.Length).First();
                keyString = string.Format(@"{0}\{1}", keyString, selectedSubKey);

                Location = Registry.LocalMachine.OpenSubKey(keyString).GetValue("LocationInformation") as string;

                if (string.IsNullOrEmpty(Location))
                    throw new NotSupportedException();
            }

            catch
            {
                Location = monikerString;
            }
        }
    }
}
