using OpenCLWrapper.Internals.Exceptions;


namespace OpenCLWrapper.Internals
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
