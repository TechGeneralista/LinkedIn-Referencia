using System;


namespace OpenCLWrapper.Internals
{
    public class HandlerPtr : Wrapper
    {
        public IntPtr Handle { get; protected set; }
        public bool IsHandleValid => Handle != IntPtr.Zero;
    }
}