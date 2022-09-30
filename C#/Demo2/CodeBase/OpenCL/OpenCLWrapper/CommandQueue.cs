using OpenCLWrapper.Internals;


namespace OpenCLWrapper
{
    public class CommandQueue : Resource
    {
        public Context Context { get; }



        public CommandQueue(Context context)
        {
            Context = context;

            Handle = clCreateCommandQueue(context.Handle, context.Device.Handle, 0, out CLError err);
            ThrowExceptionOnError(err);
        }

        protected override void FreeUnmanagedResource() => ThrowExceptionOnError(clReleaseCommandQueue(Handle));
    }
}
