using ImageCaptureDevice.UniversalVideoClass.DirectShow.Internals;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;




namespace ImageCaptureDevice.UniversalVideoClass.DirectShow
{
    public class FilterInfoCollection : CollectionBase
    {
        public FilterInfo this[int index] => (FilterInfo)InnerList[index];


        public FilterInfoCollection( Guid category )
            => CollectFilters(category);

        private void CollectFilters(Guid category)
        {
            object comObj = null;
            IEnumMoniker enumMon = null;
            IMoniker[] devMon = new IMoniker[1];
            int hr;

            ICreateDevEnum enumDev;

            try
            {
                Type srvType = Type.GetTypeFromCLSID(Clsid.SystemDeviceEnum);
                
                if(srvType == null)
                    throw new ApplicationException("Failed creating device enumerator");

                comObj = Activator.CreateInstance(srvType);
                enumDev = (ICreateDevEnum)comObj;

                hr = enumDev.CreateClassEnumerator(ref category, out enumMon, 0);

                if(hr != 0)
                    throw new ApplicationException("No devices of the category");
                    
                IntPtr n = IntPtr.Zero;

                while(true)
                {
                    hr = enumMon.Next(1, devMon, n);

                    if((hr != 0) || (devMon[0] == null))
                        break;

                    FilterInfo filter = new FilterInfo(devMon[0]);
                    InnerList.Add(filter);

                    Marshal.ReleaseComObject(devMon[0]);
                    devMon[0] = null;
                }

                InnerList.Sort();
            }

            catch { }

            finally
            {
                if(comObj != null)
                    Marshal.ReleaseComObject(comObj);

                if(enumMon != null)
                    Marshal.ReleaseComObject(enumMon);

                if(devMon[0] != null)
                {
                    Marshal.ReleaseComObject(devMon[0]);
                    devMon[0] = null;
                }
            }
        }
    }
}
