using UVC.Internals;
using System;
using System.Runtime.InteropServices;



namespace UVC.DirectShow.Internals
{
    [ComImport, Guid("C6E13370-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAMCameraControl
    {
        [PreserveSig]
        int GetRange(
            [In] CameraControlProperties Property,
            [Out] out int pMin,
            [Out] out int pMax,
            [Out] out int pSteppingDelta,
            [Out] out int pDefault,
            [Out] out ControlFlags pCapsFlags
            );

        [PreserveSig]
        int Set(
            [In] CameraControlProperties Property,
            [In] int lValue,
            [In] ControlFlags Flags
            );

        [PreserveSig]
        int Get(
            [In] CameraControlProperties Property,
            [Out] out int lValue,
            [Out] out ControlFlags Flags
            );
    }
}
