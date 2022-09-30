﻿using ImageProcessLib.OpenCL.Compute.Bindings;


namespace ImageProcessLib.OpenCL.Compute
{
    /// <summary>
    /// Represents an event created by an external library
    /// </summary>
    public class ComputeExternalEvent : ComputeEvent
    {
        /// <summary>
        /// Creates an see<see cref="ComputeExternalEvent"/> from an external handle
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="queue"></param>
        public ComputeExternalEvent(CLEventHandle handle, ComputeCommandQueue queue)
            : base(handle, queue)
        {
        }
    }
}