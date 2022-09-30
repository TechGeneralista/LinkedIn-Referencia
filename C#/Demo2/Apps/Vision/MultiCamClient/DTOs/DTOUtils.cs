using System.Runtime.InteropServices;
using System.Text;


namespace MultiCamApp.DTOs
{
    public static class DTOUtils
    {
        public static byte[] ToBytes<T>(this T structure) where T : struct
        {
            byte[] data = new byte[Marshal.SizeOf(structure)];
            GCHandle gcHandle = default;

            try
            {
                gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                Marshal.StructureToPtr(structure, gcHandle.AddrOfPinnedObject(), false);
            }
            finally
            {
                if (gcHandle.IsAllocated)
                    gcHandle.Free();
            }

            return data;
        }

        public static T ToStructure<T>(this byte[] array) where T : struct
        {
            T structure = default;
            GCHandle gcHandle = default;

            try
            {
                gcHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
                structure = Marshal.PtrToStructure<T>(gcHandle.AddrOfPinnedObject());

            }
            finally
            {
                if (gcHandle.IsAllocated)
                    gcHandle.Free();
            }

            return structure;
        }

        public static string DTOMemberToString(this byte[] array) => Encoding.ASCII.GetString(array).Replace("\0", string.Empty);

        public static byte[] StringToDTOMember(this string str, int creationLength)
        {
            byte[] array = new byte[creationLength];
            Encoding.ASCII.GetBytes(str, 0, str.Length, array, 0);
            return array;
        }
    }
}
