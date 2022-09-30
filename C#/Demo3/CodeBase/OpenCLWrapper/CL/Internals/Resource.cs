using System;


namespace OpenCLWrapper.CL.Internals
{
    public class Resource : ErrorHandler, IDisposable
    {
        protected bool disposed = false;

        ~Resource() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool isDisposing)
        {
            if (disposed)
                return;

            if (isDisposing)
                FreeManagedResource();

            FreeUnmanagedResource();
            disposed = true;
        }

        protected virtual void FreeManagedResource() { }
        protected virtual void FreeUnmanagedResource() { }
    }
}
