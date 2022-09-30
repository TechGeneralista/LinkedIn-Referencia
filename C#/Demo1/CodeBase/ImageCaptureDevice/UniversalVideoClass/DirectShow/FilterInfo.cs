using ImageCaptureDevice.UniversalVideoClass.DirectShow.Internals;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;


namespace ImageCaptureDevice.UniversalVideoClass.DirectShow
{
    public class FilterInfo : IComparable
    {
        public IMoniker Moniker { get; private set; }
        public string MonikerString { get; private set; }
        public string Guid { get; private set; }
        public string Name { get; private set; }


        internal FilterInfo(IMoniker moniker)
        {
            Moniker = moniker;

            SetMonikerString();
            SetGuid();
            SetName();
        }

        private void SetGuid()
        {
            string upperMonikerString = MonikerString.ToUpper();
            string keyString = @"SYSTEM\CurrentControlSet\Enum\USB";
            string[] subKeys = Registry.LocalMachine.OpenSubKey(keyString).GetSubKeyNames();
            List<string> containsList = new List<string>();

            foreach (string subKey in subKeys)
            {
                if (upperMonikerString.Contains(subKey))
                    containsList.Add(subKey);
            }

            string selectedSubKey = containsList.OrderByDescending(s => s.Length).First();
            keyString = string.Format(@"{0}\{1}", keyString, selectedSubKey);

            subKeys = Registry.LocalMachine.OpenSubKey(keyString).GetSubKeyNames();
            containsList = new List<string>();

            foreach (string subKey in subKeys)
            {
                if (MonikerString.Contains(subKey))
                    containsList.Add(subKey);
            }

            selectedSubKey = containsList.OrderByDescending(s => s.Length).First();
            keyString = string.Format(@"{0}\{1}", keyString, selectedSubKey);

            Guid = Registry.LocalMachine.OpenSubKey(keyString).GetValue("LocationInformation") as string;

            if (string.IsNullOrEmpty(Guid))
                throw new NotSupportedException();
        }

        public int CompareTo(object value)
        {
            FilterInfo f = (FilterInfo)value;

            if (f == null)
                return 1;

            return (Name.CompareTo(f.Name));
        }

        public static object CreateFilter( string filterMoniker )
        {
            object filterObject = null;
            int n = 0;
            IBindCtx bindCtx;
            IMoniker moniker;

            if(Win32.CreateBindCtx(0, out bindCtx) == 0)
            {
                if(Win32.MkParseDisplayName(bindCtx, filterMoniker, ref n, out moniker) == 0)
                {
                    Guid filterId = typeof(IBaseFilter).GUID;
                    moniker.BindToObject(null, null, ref filterId, out filterObject);

                    Marshal.ReleaseComObject(moniker);
                }

                Marshal.ReleaseComObject(bindCtx);
            }

            return filterObject;
        }

        private void SetMonikerString()
        {
            Moniker.GetDisplayName(null, null, out string str);
            MonikerString = str;
        }

        private void SetName()
        {
            Object bagObj = null;
            IPropertyBag bag;

            try
            {
                Guid bagId = typeof( IPropertyBag ).GUID;
                Moniker.BindToStorage( null, null, ref bagId, out bagObj );
                bag = (IPropertyBag) bagObj;

                object val = "";
                int hr = bag.Read("FriendlyName", ref val, IntPtr.Zero );

                if ( hr != 0 )
                    Marshal.ThrowExceptionForHR(hr);

                string ret = (string) val;

                if ( ( ret == null ) || ( ret.Length < 1 ) )
                    throw new ApplicationException();

                Name = ret;
            }

            catch ( Exception )
            {
                Name = "";
            }

            finally
            {
                if( bagObj != null )
                    Marshal.ReleaseComObject( bagObj );
            }
        }
    }
}
