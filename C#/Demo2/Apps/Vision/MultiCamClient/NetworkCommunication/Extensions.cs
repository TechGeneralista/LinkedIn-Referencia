using System;
using System.Linq;
using System.Text;


namespace MultiCamApp.NetworkCommunication
{
    public static class Extensions
    {
        static byte[] eomPattern = Encoding.ASCII.GetBytes("<EOM>");

        public static bool IsEOMAtEnd(this byte[] bytes)
        {
            int toIndex = bytes.Length - eomPattern.Length;
            
            for(int i=0; i <= toIndex;i++)
            {
                if (bytes.Skip(i).Take(eomPattern.Length).SequenceEqual(eomPattern))
                    return true;
            }

            return false;
        }

        public static byte[] AddEOMToEnd(this byte[] bytes)
        {
            int oldLength = bytes.Length;
            Array.Resize(ref bytes, oldLength + eomPattern.Length);
            Array.Copy(eomPattern, 0, bytes, oldLength, eomPattern.Length);
            return bytes;
        }

        public static byte[] RemoveEOMFromEnd(this byte[] bytes)
        {
            return bytes.Take(bytes.Length - eomPattern.Length).ToArray();
        }
    }
}
