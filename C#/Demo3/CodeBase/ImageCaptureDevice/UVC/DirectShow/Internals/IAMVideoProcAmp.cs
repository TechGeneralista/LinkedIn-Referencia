using System;
using System.Runtime.InteropServices;



namespace ImageCaptureDevice.UVC.DirectShow.Internals
{
    [ComImport,
    Guid("C6E13360-30AC-11D0-A18C-00A0C9118956"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAMVideoProcAmp
    {
        [PreserveSig]
        int GetRange(
            [In] VideoProcAmpProperties Property,
            [Out] out int pMin,
            [Out] out int pMax,
            [Out] out int pSteppingDelta,
            [Out] out int pDefault,
            [Out] out ControlFlags pCapsFlags
            );

        [PreserveSig]
        int Set(
            [In] VideoProcAmpProperties Property,
            [In] int lValue,
            [In] ControlFlags Flags
            );

        [PreserveSig]
        int Get(
            [In] VideoProcAmpProperties Property,
            [Out] out int lValue,
            [Out] out ControlFlags Flags
            );
    }
}