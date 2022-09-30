using System.Runtime.InteropServices;


namespace MultiCamApp.DTOs
{
    public unsafe struct CommandDTO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Command;
    }
}
