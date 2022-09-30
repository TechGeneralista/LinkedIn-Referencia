using Compute.CL.Internals;
using System;
using System.Runtime.InteropServices;


namespace Compute.CL
{
    public class Kernel : ErrorHandler
    {
        public Program Program { get; }


        public Kernel(Program program)
        {
            Program = program;

            Handle = clCreateKernel(program.Handle, program.FunctionName, out CLError err);
            ThrowExceptionOnError(err);
        }

        protected override void FreeUnmanagedResource() => ThrowExceptionOnError(clReleaseKernel(Handle));

        public void SetArg(uint index, sbyte value) => ThrowExceptionOnError(clSetKernelArg(Handle, index, sizeof(sbyte), ref value));
        public void SetArg(uint index, byte value) => ThrowExceptionOnError(clSetKernelArg(Handle, index, sizeof(byte), ref value));
        public void SetArg(uint index, short value) => ThrowExceptionOnError(clSetKernelArg(Handle, index, sizeof(short), ref value));
        public void SetArg(uint index, ushort value) => ThrowExceptionOnError(clSetKernelArg(Handle, index, sizeof(ushort), ref value));
        public void SetArg(uint index, int value) => ThrowExceptionOnError(clSetKernelArg(Handle, index, sizeof(int), ref value));
        public void SetArg(uint index, uint value) => ThrowExceptionOnError(clSetKernelArg(Handle, index, sizeof(uint), ref value));
        public void SetArg(uint index, long value) => ThrowExceptionOnError(clSetKernelArg(Handle, index, sizeof(long), ref value));
        public void SetArg(uint index, ulong value) => ThrowExceptionOnError(clSetKernelArg(Handle, index, sizeof(ulong), ref value));
        public void SetArg(uint index, float value) => ThrowExceptionOnError(clSetKernelArg(Handle, index, sizeof(float), ref value));
        public void SetArg(uint index, double value) => ThrowExceptionOnError(clSetKernelArg(Handle, index, sizeof(double), ref value));

        public void SetArg(uint index, Resource value)
        {
            IntPtr handle = value.Handle;
            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)Marshal.SizeOf(handle), ref handle));
        }

        public void SetArg(uint index, sbyte[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(sbyte) * alignment), value));
        }

        public void SetArg(uint index, byte[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(byte) * alignment), value));
        }

        public void SetArg(uint index, short[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(short) * alignment), value));
        }

        public void SetArg(uint index, ushort[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(ushort) * alignment), value));
        }

        public void SetArg(uint index, int[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(int) * alignment), value));
        }

        public void SetArg(uint index, uint[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(uint) * alignment), value));
        }

        public void SetArg(uint index, long[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(long) * alignment), value));
        }

        public void SetArg(uint index, ulong[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(ulong) * alignment), value));
        }

        public void SetArg(uint index, float[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(float) * alignment), value));
        }

        public void SetArg(uint index, double[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(double) * alignment), value));
        }
    }
}
