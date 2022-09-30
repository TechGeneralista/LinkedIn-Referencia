using OpenCLWrapper.CL.Internals;


namespace OpenCLWrapper.CL
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

        public void Flush() => ThrowExceptionOnError(clFlush(Handle));
        public void Finish() => ThrowExceptionOnError(clFinish(Handle));

        protected override void FreeUnmanagedResource() => ThrowExceptionOnError(clReleaseCommandQueue(Handle));
    }
}
