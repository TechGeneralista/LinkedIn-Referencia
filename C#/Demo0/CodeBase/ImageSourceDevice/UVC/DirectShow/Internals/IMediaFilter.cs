using System;
using System.Runtime.InteropServices;
using System.Security;



namespace ImageSourceDevice.UVC.DirectShow.Internals
{
    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a86899-0ad4-11ce-b03a-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMediaFilter : IPersist
    {
        [PreserveSig]
        new int GetClassID([Out] out Guid pClassID);

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int Pause();

        [PreserveSig]
        int Run([In] long tStart);

        [PreserveSig]
        int GetState(
            [In] int dwMilliSecsTimeout,
            [Out] out FilterState filtState);

        [PreserveSig]
        int SetSyncSource([In] IReferenceClock pClock);

        [PreserveSig]
        int GetSyncSource([Out] out IReferenceClock pClock);
    }
}

