using System;
using System.Diagnostics;


namespace ImageProcessLib.OpenCL.Compute.Bindings
{
    /// <summary>
    /// Represents the <see cref="ComputeKernel"/> ID.
    /// </summary>
    public struct CLKernelHandle
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IntPtr _value;

        /// <summary>
        /// Gets a logic value indicating whether the handle is valid.
        /// </summary>
        public bool IsValid => _value != IntPtr.Zero;

        /// <summary>
        /// Gets the value of the handle.
        /// </summary>
        public IntPtr Value => _value;

        /// <summary>
        /// Invalidates the handle.
        /// </summary>
        public void Invalidate()
        {
            _value = IntPtr.Zero;
        }
    }
}