using System;


namespace OpenCLWrapper.CL.Internals
{
    public class HandlerPtr : Driver
    {
        public IntPtr Handle { get; protected set; }
        public bool IsHandleValid => Handle != IntPtr.Zero;
    }
}