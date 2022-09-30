using System;


namespace Common.Tool
{
    public static class Extensions
    {
        public static bool IsNotNull(this object o) => o != null;
        public static bool IsNull(this object o) => o == null;
        public static string NewGuid(int charCount) => Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0,charCount).ToUpper();
    }
}
