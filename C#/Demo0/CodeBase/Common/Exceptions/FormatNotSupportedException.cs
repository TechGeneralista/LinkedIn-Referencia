using System;


namespace Common.Exceptions
{
    public class FormatNotSupportedException : Exception
    {
        public FormatNotSupportedException() { }
        public FormatNotSupportedException(string message) : base(message) { }
    }
}
