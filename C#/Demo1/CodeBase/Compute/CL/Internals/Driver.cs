using System;
using System.Runtime.InteropServices;


namespace Compute.CL.Internals
{
    public delegate void LoggingFunction(IntPtr errinfo, IntPtr private_info, IntPtr cb, IntPtr user_data);
    public delegate void NotifyFunction(IntPtr program, IntPtr user_data);


    public class Driver
    {
        protected const string OPENCL_DLL_NAME = "OpenCL";

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clReleaseMemObject(IntPtr memobj);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern IntPtr clCreateBuffer(
            IntPtr context,
            CLMemFlags flags,
            SizeT size,
            IntPtr host_ptr,
            out CLError errcode_ret);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern IntPtr clCreateImage(
            IntPtr context,
            CLMemFlags flags,
            ref CLImageFormat image_format,
            ref CLImageDesc image_desc,
            IntPtr host_ptr,
            out CLError errcode_ret);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern IntPtr clCreateCommandQueue(
            IntPtr context,
            IntPtr device,
            CLCommandQueueProperties properties,
            out CLError errcode_ret);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clReleaseCommandQueue(IntPtr command_queue);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern IntPtr clCreateContext(
            IntPtr[] properties,
            uint num_devices,
            IntPtr[] devices,
            LoggingFunction pfn_notify,
            IntPtr user_data,
            out CLError errcode_ret);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clReleaseContext(IntPtr context);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clGetDeviceIDs(
            IntPtr platform_id,
            CLDeviceType device_type,
            uint num_entries,
            IntPtr[] devices,
            out uint num_devices);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clGetDeviceInfo(
            IntPtr device,
            CLDeviceInfo param_name,
            uint param_value_size,
            IntPtr param_value,
            out uint param_value_size_ret);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clEnqueueWriteBuffer(
            IntPtr command_queue,
            IntPtr buffer,
            CLBool blocking_write,
            uint offset,
            uint size,
            IntPtr ptr,
            uint num_events_in_wait_list,
            IntPtr[] event_wait_list,
            IntPtr e);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clEnqueueReadBuffer(
            IntPtr command_queue,
            IntPtr buffer,
            CLBool blocking_write,
            uint offset,
            uint size,
            IntPtr ptr,
            uint num_events_in_wait_list,
            IntPtr[] event_wait_list,
            IntPtr e);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clEnqueueNDRangeKernel(
            IntPtr command_queue,
            IntPtr kernel,
            uint work_dim,
            SizeT[] global_work_offset,
            SizeT[] global_work_size,
            SizeT[] local_work_size,
            uint num_events_in_wait_list,
            IntPtr[] event_wait_list,
            IntPtr e);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clEnqueueWriteImage(
            IntPtr command_queue,
            IntPtr image,
            CLBool blocking_write,
            SizeT[] origin,
            SizeT[] region,
            SizeT input_row_pitch,
            SizeT input_slice_pitch,
            IntPtr ptr,
            int num_events_in_wait_list,
            IntPtr[] event_wait_list,
            IntPtr e);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clEnqueueReadImage(
            IntPtr command_queue,
            IntPtr image,
            CLBool blocking_write,
            SizeT[] origin,
            SizeT[] region,
            SizeT input_row_pitch,
            SizeT input_slice_pitch,
            IntPtr ptr,
            int num_events_in_wait_list,
            IntPtr[] event_wait_list,
            IntPtr e);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clEnqueueCopyImage(
            IntPtr command_queue,
            IntPtr src_image,
            IntPtr dst_image,
            SizeT[] src_origin,
            SizeT[] dst_origin,
            SizeT[] region,
            uint num_events_in_wait_list,
            IntPtr[] event_wait_list,
            IntPtr e);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern IntPtr clCreateKernel(
            IntPtr program,
            string kernel_name,
            out CLError errcode_ret);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clReleaseKernel(IntPtr kernel);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            ref IntPtr arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            ref sbyte arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            [In] sbyte[] arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            ref byte arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            [In] byte[] arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            ref short arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            [In] short[] arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            ref ushort arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            [In] ushort[] arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            ref int arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            [In] int[] arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            ref uint arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            [In] uint[] arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            ref long arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            [In] long[] arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            ref ulong arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            [In] ulong[] arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            ref float arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            [In] float[] arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            ref double arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clSetKernelArg(
            IntPtr kernel,
            uint arg_index,
            uint arg_size,
            [In] double[] arg_value);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clGetPlatformIDs(
            uint num_entries,
            IntPtr[] platforms,
            out uint num_platforms);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clGetPlatformInfo(
            IntPtr platform,
            CLPlatformInfo param_name,
            uint param_value_size,
            IntPtr param_value,
            out uint param_value_size_ret);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern IntPtr clCreateProgramWithSource(
            IntPtr context,
            uint count,
            string[] strings,
            uint[] lengths,
            ref CLError errcode_ret);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern IntPtr clCreateProgramWithSource(
            IntPtr context,
            uint count,
            [In] IntPtr[] strings,
            [In] IntPtr[] lengths,
            ref CLError errcode_ret);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clReleaseProgram(IntPtr program);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clBuildProgram(
            IntPtr program,
            uint num_devices,
            [In] IntPtr[] device_list,
            string options,
            NotifyFunction func,
            IntPtr user_data);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clBuildProgram(
            IntPtr program,
            uint num_devices,
            [In] IntPtr[] device_list,
            IntPtr options,
            NotifyFunction func,
            IntPtr user_data);

        [DllImport(OPENCL_DLL_NAME)]
        protected static extern CLError clGetProgramBuildInfo(
            IntPtr program,
            IntPtr device,
            CLProgramBuildInfo param_name,
            IntPtr param_value_size,
            IntPtr param_value,
            ref IntPtr param_value_size_ret);

        [DllImport(OPENCL_DLL_NAME)]
        public static extern CLError clFlush(IntPtr command_queue);

        [DllImport(OPENCL_DLL_NAME)]
        public static extern CLError clFinish(IntPtr command_queue);
    }
}
