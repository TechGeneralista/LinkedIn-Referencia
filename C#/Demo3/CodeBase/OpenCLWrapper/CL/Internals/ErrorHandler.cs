using OpenCLWrapper.CL.Internals.Exceptions;


namespace OpenCLWrapper.CL.Internals
{
    public class ErrorHandler : HandlerPtr
    {
        protected void ThrowExceptionOnError(CLError clError)
        {
            if (clError != CLError.Success)
                throw new OpenCLException(clError);
        }
    }
}
