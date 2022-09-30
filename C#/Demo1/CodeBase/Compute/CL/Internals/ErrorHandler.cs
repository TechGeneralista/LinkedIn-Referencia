using Compute.CL.Internals.Exceptions;


namespace Compute.CL.Internals
{
    public class ErrorHandler : Resource
    {
        protected void ThrowExceptionOnError(CLError clError)
        {
            if (clError != CLError.Success)
                throw new OpenCLException(clError);
        }
    }
}
