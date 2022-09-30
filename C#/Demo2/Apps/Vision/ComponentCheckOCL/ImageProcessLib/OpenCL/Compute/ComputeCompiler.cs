using ImageProcessLib.OpenCL.Compute.Bindings;
using System;


namespace ImageProcessLib.OpenCL.Compute
{
    /// <summary>
    /// Represents the OpenCL compiler.
    /// </summary>
    public class ComputeCompiler
    {
        #region Public methods

        /// <summary>
        /// Unloads the OpenCL compiler.
        /// </summary>
        [Obsolete("Deprecated in OpenCL 1.2.")]
        public static void Unload()
        {
            ComputeErrorCode error = CL12.UnloadCompiler();
            ComputeException.ThrowOnError(error);
        }

        #endregion
    }
}