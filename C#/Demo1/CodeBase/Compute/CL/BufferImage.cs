using Compute.CL.Internals;
using System;


namespace Compute.CL
{
    public class BufferImage : ErrorHandler
    {
        public CLMemFlags MemFlags { get; private set; }

        public CLImageFormat ImageFormat
        {
            get => imageFormat;
            private set => imageFormat = value;
        }
        protected CLImageFormat imageFormat;

        public CLImageDesc ImageDesc
        {
            get => imageDesc;
            private set => imageDesc = value;
        }
        protected CLImageDesc imageDesc;


        protected readonly ComputeAccelerator computeAccelerator;


        protected BufferImage(ComputeAccelerator computeAccelerator)
        {
            this.computeAccelerator = computeAccelerator;
        }

        public void CreateIfNeed(BufferImage bufferImage)
            => CreateIfNeed(bufferImage.MemFlags, bufferImage.imageFormat, bufferImage.imageDesc);

        public void CreateIfNeed(CLMemFlags memFlags, CLImageFormat imageFormat, CLImageDesc imageDesc)
        {
            if (HandleIsValid && (MemFlags != memFlags || this.imageFormat != imageFormat || this.imageDesc != imageDesc))
            {
                FreeUnmanagedResource();
            }

            if (!HandleIsValid)
            {
                MemFlags = memFlags;
                this.imageFormat = imageFormat;
                this.imageDesc = imageDesc;

                Handle = clCreateImage(computeAccelerator.Context.Handle, memFlags, ref this.imageFormat, ref this.imageDesc, IntPtr.Zero, out CLError err);
                ThrowExceptionOnError(err);
            }
        }

        protected override void FreeUnmanagedResource()
        {
            if(HandleIsValid)
            {
                ThrowExceptionOnError(clReleaseMemObject(Handle));
                Handle = IntPtr.Zero;
                imageDesc = new CLImageDesc();
            }
        }
    }
}
