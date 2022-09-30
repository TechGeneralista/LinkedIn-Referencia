using System.Runtime.InteropServices;


namespace MultiCamApp.DTOs
{
    public struct DeviceInfoDTO0
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Command;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] IPAddress;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Version;
    }
}
