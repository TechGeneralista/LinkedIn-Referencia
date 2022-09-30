using System;
using System.Runtime.InteropServices;



namespace UVC.DirectShow.Internals
{
    [ComImport,
    Guid("B196B28B-BAB4-101A-B69C-00AA00341D07"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISpecifyPropertyPages
    {
        [PreserveSig]
        int GetPages(out CAUUID pPages);
    }
}
