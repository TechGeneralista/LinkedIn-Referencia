using OpenCLWrapper.CL.Internals;
using System;
using System.Runtime.InteropServices;


namespace OpenCLWrapper.CL
{
    public class Program : Resource
    {
        public Context Context { get; }
        public string FunctionName { get; }
        public string FunctionSource { get; }


        public Program(Context context, string functionName, string functionSource)
        {
            Context = context;
            FunctionName = functionName;
            FunctionSource = functionSource;

            CLError err = CLError.Success;
            Handle = clCreateProgramWithSource(context.Handle, 1, new string[] { functionSource }, new uint[] { (uint)functionSource.Length }, ref err);
            ThrowExceptionOnError(err);
        }

        protected override void FreeUnmanagedResource() => ThrowExceptionOnError(clReleaseProgram(Handle));

        public void Build()
        {
            try
            {
                ThrowExceptionOnError(clBuildProgram(Handle, 1, new IntPtr[] { Context.Device.Handle }, IntPtr.Zero, null, IntPtr.Zero));
            }
            catch
            {
                string buildInfo = (string)GetProgramBuildInfo(CLProgramBuildInfo.Log);
                throw new Exception(buildInfo);
            }
        }

        public object GetProgramBuildInfo(CLProgramBuildInfo info)
        {
            IntPtr param_value_size_ret = IntPtr.Zero;
            IntPtr ptr = IntPtr.Zero;
            object result = null;

            ThrowExceptionOnError(clGetProgramBuildInfo(Handle, Context.Device.Handle, info, IntPtr.Zero, IntPtr.Zero, ref param_value_size_ret));

            if ((int)param_value_size_ret == 0)
                return result;

            ptr = Marshal.AllocHGlobal(param_value_size_ret);

            try
            {
                clGetProgramBuildInfo(Handle, Context.Device.Handle, info, param_value_size_ret, ptr, ref param_value_size_ret);

                switch (info)
                {
                    case CLProgramBuildInfo.Status:
                        result = (CLBuildStatus)Marshal.ReadInt32(ptr);
                        break;
                    case CLProgramBuildInfo.Options:
                        result = Marshal.PtrToStringAnsi(ptr, (int)param_value_size_ret);
                        break;
                    case CLProgramBuildInfo.Log:
                        result = Marshal.PtrToStringAnsi(ptr, (int)param_value_size_ret);
                        break;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return result;
        }
    }
}
