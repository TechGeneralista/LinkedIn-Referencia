using System;


namespace Compute.CL.Internals
{
    public class HandlerPtr : Driver
    {
        public IntPtr Handle { get; protected set; }
        protected bool HandleIsValid => Handle != IntPtr.Zero;
    }
}