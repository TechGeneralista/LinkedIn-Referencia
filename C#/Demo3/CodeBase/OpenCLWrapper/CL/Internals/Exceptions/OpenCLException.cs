using System;


namespace OpenCLWrapper.CL.Internals.Exceptions
{
    public class OpenCLException : Exception
    {
        public CLError OpenCLErrorCode { get; }


        public OpenCLException(CLError error)
        {
            OpenCLErrorCode = error;
        }

        public override string Message
        {
            get
            {
                return string.Format("OpenCL error {0} (Code: {1})", OpenCLErrorCode.ToString(), (int)OpenCLErrorCode);
            }
        }
    }
}
