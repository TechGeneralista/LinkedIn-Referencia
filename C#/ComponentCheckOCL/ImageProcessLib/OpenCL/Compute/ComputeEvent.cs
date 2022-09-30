using ImageProcessLib.OpenCL.Compute.Bindings;
using System.Runtime.InteropServices;


namespace ImageProcessLib.OpenCL.Compute
{
    /// <summary>
    /// Represents an OpenCL event.
    /// </summary>
    /// <remarks> An event encapsulates the status of an operation such as a command. It can be used to synchronize operations in a context. </remarks>
    /// <seealso cref="ComputeUserEvent"/>
    /// <seealso cref="ComputeCommandQueue"/>
    /// <seealso cref="ComputeContext"/>
    public class ComputeEvent : ComputeEventBase
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="ComputeCommandQueue"/> associated with the <see cref="ComputeEvent"/>.
        /// </summary>
        /// <value> The <see cref="ComputeCommandQueue"/> associated with the <see cref="ComputeEvent"/>. </value>
        public ComputeCommandQueue CommandQueue { get; }

        #endregion

        #region Constructors

        internal ComputeEvent(CLEventHandle handle, ComputeCommandQueue queue) : this (handle, queue, 0)
        {
            Type = (ComputeCommandType)GetInfo<CLEventHandle, ComputeEventInfo, int>(Handle, ComputeEventInfo.CommandType, CL12.GetEventInfo);
        }

        internal ComputeEvent(CLEventHandle handle, ComputeCommandQueue queue, ComputeCommandType type)
        {
            Handle = handle;
            SetID(Handle.Value);

            CommandQueue = queue;
            Type = type;
            Context = queue.Context;

            //if (ComputeTools.ParseVersionString(CommandQueue.Device.Platform.Version, 1) > new Version(1, 0))
            //    HookNotifier();

            //Debug.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }



        #endregion

        #region Internal methods

        internal void TrackGCHandle(GCHandle gcHandle)
        {
            var freeDelegate = new ComputeCommandStatusChanged((s, e) =>
            {
                if (gcHandle.IsAllocated && gcHandle.Target != null) gcHandle.Free();
            });

            Completed += freeDelegate;
            Aborted += freeDelegate;
        }

        #endregion
        
        /// <summary>
        /// Clones the event. Because the event is retained the cloned event as well as the clone have to be disposed
        /// </summary>
        /// <returns>Cloned event</returns>
        public override ComputeEventBase Clone()
        {
            CL10.RetainEvent(Handle);
            return new ComputeEvent(Handle, CommandQueue, Type);
        }
    }
}