using System.Runtime.InteropServices;


namespace MultiCamApp.DTOs
{
    public struct DeviceInfoDTO1
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Command;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Name;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] SerialNo;
    }
}
