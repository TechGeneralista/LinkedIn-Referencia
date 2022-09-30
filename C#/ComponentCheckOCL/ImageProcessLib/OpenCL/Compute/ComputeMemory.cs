using ImageProcessLib.OpenCL.Compute.Bindings;


namespace ImageProcessLib.OpenCL.Compute
{
    using System.Diagnostics;

    /// <summary>
    /// Represents an OpenCL memory object.
    /// </summary>
    /// <remarks> A memory object is a handle to a region of global memory. </remarks>
    /// <seealso cref="ComputeBuffer{T}"/>
    /// <seealso cref="ComputeImage"/>
    public abstract class ComputeMemory : ComputeResource
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComputeContext _context;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComputeMemoryFlags _flags;

        private CLMemoryHandle _handle;

        #endregion

        #region Properties

        /// <summary>
        /// The handle of the <see cref="ComputeMemory"/>.
        /// </summary>
        public CLMemoryHandle Handle
        {
            get => _handle;
            protected set => _handle = value;
        }

        /// <summary>
        /// Gets the <see cref="ComputeContext"/> of the <see cref="ComputeMemory"/>.
        /// </summary>
        /// <value> The <see cref="ComputeContext"/> of the <see cref="ComputeMemory"/>. </value>
        public ComputeContext Context => _context;

        /// <summary>
        /// Gets the <see cref="ComputeMemoryFlags"/> of the <see cref="ComputeMemory"/>.
        /// </summary>
        /// <value> The <see cref="ComputeMemoryFlags"/> of the <see cref="ComputeMemory"/>. </value>
        public ComputeMemoryFlags Flags => _flags;

        /// <summary>
        /// Gets or sets (protected) the size in bytes of the <see cref="ComputeMemory"/>.
        /// </summary>
        /// <value> The size in bytes of the <see cref="ComputeMemory"/>. </value>
        public long Size { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="flags"></param>
        protected ComputeMemory(ComputeContext context, ComputeMemoryFlags flags)
        {
            _context = context;
            _flags = flags;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Releases the associated OpenCL object.
        /// </summary>
        /// <param name="manual"> Specifies the operation mode of this method. </param>
        /// <remarks> <paramref name="manual"/> must be <c>true</c> if this method is invoked directly by the application. </remarks>
        protected override void Dispose(bool manual)
        {
            if (Handle.IsValid)
            {
                //Debug.WriteLine("Dispose " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
                CL12.ReleaseMemObject(Handle);
                _handle.Invalidate();
            }
        }

        #endregion
    }
}