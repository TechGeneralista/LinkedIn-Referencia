using System.Runtime.InteropServices;


namespace MultiCamApp.DTOs
{
    public unsafe struct IPAddressDTO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Command;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] IPAddress;
    }
}
