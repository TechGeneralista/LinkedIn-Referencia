using System;


namespace Compute.CL.Internals
{
    public class Resource : HandlerPtr, IDisposable
    {
        bool disposed = false;

        ~Resource() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                FreeManagedResource();

            FreeUnmanagedResource();
            disposed = true;
        }

        protected virtual void FreeManagedResource() { }
        protected virtual void FreeUnmanagedResource() { }
    }
}
