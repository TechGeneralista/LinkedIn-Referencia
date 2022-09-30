using OpenCLWrapper.CL.Internals;
using System;
using System.Runtime.InteropServices;


namespace OpenCLWrapper.CL
{
    public class Kernel : Resource
    {
        public Program Program { get; }
        public uint WorkGroupSize { get; }


        public Kernel(Program program)
        {
            Program = program;

            Handle = clCreateKernel(program.Handle, program.FunctionName, out CLError err);
            ThrowExceptionOnError(err);

            WorkGroupSize = (uint)GetKernelWorkGroupInfo(CLKernelWorkGroupInfo.WorkGroupSize);
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

        public void SetArg(uint index, int[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(int) * alignment), value));
        }

        public void SetArg(uint index, long[] value)
        {
            int alignment = value.Length;

            if (alignment == 3)
                alignment = 4;

            ThrowExceptionOnError(clSetKernelArg(Handle, index, (uint)(sizeof(long) * alignment), value));
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

        private object GetKernelWorkGroupInfo(CLKernelWorkGroupInfo info)
        {
            IntPtr value = IntPtr.Zero;
            object result = null;

            ThrowExceptionOnError(clGetKernelWorkGroupInfo(Handle, Program.Context.Device.Handle, info, 0, value, out uint size));

            if (size == 0)
                return result;

            value = Marshal.AllocHGlobal(new IntPtr(size));

            try
            {
                clGetKernelWorkGroupInfo(Handle, Program.Context.Device.Handle, info, size, value, out size);

                switch (info)
                {
                    case CLKernelWorkGroupInfo.WorkGroupSize:
                        result = (uint)Marshal.ReadInt32(value);
                        break;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(value);
            }

            return result;
        }
    }
}
