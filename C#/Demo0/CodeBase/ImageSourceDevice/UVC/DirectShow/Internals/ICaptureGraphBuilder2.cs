using System;
using System.Runtime.InteropServices;



namespace ImageSourceDevice.UVC.DirectShow.Internals
{ 
    [ComImport,
    Guid("93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICaptureGraphBuilder2
    {
        [PreserveSig]
        int SetFiltergraph([In] IGraphBuilder graphBuilder);

        [PreserveSig]
        int GetFiltergraph([Out] out IGraphBuilder graphBuilder);

        [PreserveSig]
        int SetOutputFileName(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid type,
            [In, MarshalAs(UnmanagedType.LPWStr)] string fileName,
            [Out] out IBaseFilter baseFilter,
            [Out] out IntPtr fileSinkFilter
            );

        [PreserveSig]
        int FindInterface(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid category,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid type,
            [In] IBaseFilter baseFilter,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid interfaceID,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object retInterface
            );

        [PreserveSig]
        int RenderStream(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid category,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid mediaType,
            [In, MarshalAs(UnmanagedType.IUnknown)] object source,
            [In] IBaseFilter compressor,
            [In] IBaseFilter renderer
            );

        [PreserveSig]
        int ControlStream(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid category,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid mediaType,
            [In, MarshalAs(UnmanagedType.Interface)] IBaseFilter filter,
            [In] long start,
            [In] long stop,
            [In] short startCookie,
            [In] short stopCookie
            );

        [PreserveSig]
        int AllocCapFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string fileName,
            [In] long size
            );

        [PreserveSig]
        int CopyCaptureFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string oldFileName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string newFileName,
            [In, MarshalAs(UnmanagedType.Bool)] bool allowEscAbort,
            [In] IntPtr callback
            );

        [PreserveSig]
        int FindPin(
            [In, MarshalAs(UnmanagedType.IUnknown)] object source,
            [In] PinDirection pinDirection,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid category,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid mediaType,
            [In, MarshalAs(UnmanagedType.Bool)] bool unconnected,
            [In] int index,
            [Out, MarshalAs(UnmanagedType.Interface)] out IPin pin
            );
    }
}
