using ImageProcessLib.OpenCL.Bindings;
using ImageProcessLib.OpenCL.Compute.Bindings;


namespace ImageProcessLib.OpenCL.Compute
{
    /// <summary>
    /// Represents an user created event.
    /// </summary>
    /// <remarks> Requires OpenCL 1.1. </remarks>
    public class ComputeUserEvent : ComputeEventBase
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ComputeUserEvent"/>.
        /// </summary>
        /// <param name="context"> The <see cref="ComputeContext"/> in which the <see cref="ComputeUserEvent"/> is created. </param>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        public ComputeUserEvent(ComputeContext context)
        {
            Handle = CL11.CreateUserEvent(context.Handle, out var error);
            ComputeException.ThrowOnError(error);
            
            SetID(Handle.Value);

            Type = (ComputeCommandType)GetInfo<CLEventHandle, ComputeEventInfo, uint>(Handle, ComputeEventInfo.CommandType, CL12.GetEventInfo);
            Context = context;
            //HookNotifier();

            //Debug.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        internal ComputeUserEvent(ComputeContext context, CLEventHandle handle, ComputeCommandType type)
        {
            Handle = handle;

            SetID(Handle.Value);

            Type = type;

            Context = context;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the new status of the <see cref="ComputeUserEvent"/>.
        /// </summary>
        /// <param name="status"> The new status of the <see cref="ComputeUserEvent"/>. Allowed value is <see cref="ComputeCommandExecutionStatus.Complete"/>. </param>
        public void SetStatus(ComputeCommandExecutionStatus status)
        {
            SetStatus((int)status);
        }

        /// <summary>
        /// Sets the new status of the <see cref="ComputeUserEvent"/> to an error value.
        /// </summary>
        /// <param name="status"> The error status of the <see cref="ComputeUserEvent"/>. This should be a negative value. </param>
        public void SetStatus(int status)
        {
            ComputeErrorCode error = CL11.SetUserEventStatus(Handle, status);
            ComputeException.ThrowOnError(error);
        }

        #endregion

        /// <summary>
        /// Clones the event. Because the event is retained the cloned event as well as the clone have to be disposed
        /// </summary>
        /// <returns>Cloned event</returns>
        public override ComputeEventBase Clone()
        {
            CL10.RetainEvent(Handle);
            return new ComputeUserEvent(Context, Handle, Type);
        }
    }
}