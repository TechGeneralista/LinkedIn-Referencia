using ImageProcessLib.OpenCL.Compute.Bindings;
using System;


namespace ImageProcessLib.OpenCL.Compute
{
    /// <summary>
    /// Represents the parent type to any ImageProcessLib.OpenCL buffer types.
    /// </summary>
    /// <typeparam name="T"> The type of the elements of the buffer. </typeparam>
    public abstract class ComputeBufferBase<T> : ComputeMemory where T : struct
    {
        #region Properties

        /// <summary>
        /// Gets the number of elements in the <see cref="ComputeBufferBase{T}"/>.
        /// </summary>
        /// <value> The number of elements in the <see cref="ComputeBufferBase{T}"/>. </value>
        public long Count { get; private set; }

        /// <summary>
        /// Gets the current reference count of the <see cref="ComputeBufferBase{T}"/>.
        /// </summary>
        /// <value> The current reference count of the <see cref="ComputeBufferBase{T}"/>. </value>
        public uint ReferenceCount => GetInfo<CLMemoryHandle, ComputeMemoryInfo, uint>(Handle, ComputeMemoryInfo.ReferenceCount, CL12.GetMemObjectInfo);

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="flags"></param>
        protected ComputeBufferBase(ComputeContext context, ComputeMemoryFlags flags)
            : base(context, flags)
        { }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        protected void Init(long size)
        {
            SetID(Handle.Value);

            Size = size;
            Count = Size / ComputeTools.SizeOf<T>();

            //Debug.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Init(long size, long count)
        {
            SetID(Handle.Value);

            Size = size;
            Count = count;

            //Debug.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Init()
        {
            SetID(Handle.Value);

            Size = (long)GetInfo<CLMemoryHandle, ComputeMemoryInfo, IntPtr>(Handle, ComputeMemoryInfo.Size, CL12.GetMemObjectInfo);
            Count = Size / ComputeTools.SizeOf<T>();

            //Debug.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        #endregion

        /// <summary>
        /// Clones the ComputeBuffer. Because the buffer is retained the cloned buffer as well as the clone have to be disposed
        /// </summary>
        /// <returns>Cloned buffer</returns>
        public abstract ComputeBufferBase<T> Clone();
    }
}