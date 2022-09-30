using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;



namespace UVCLib.DirectShow.Internals
{
    internal static class Win32
    {
        [DllImport("ole32.dll")]
        public static extern
        int CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
        public static extern
        int MkParseDisplayName(IBindCtx pbc, string szUserName,
            ref int pchEaten, out IMoniker ppmk);

        [DllImport("ntdll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int memcpy(
            byte* dst,
            byte* src,
            int count);
    }
}
