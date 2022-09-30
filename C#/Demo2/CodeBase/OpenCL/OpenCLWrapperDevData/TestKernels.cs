namespace OpenCLWrapperDevData
{
    class KernelSourceTest0
    {
        public static readonly string Name = @"test0";

        public static readonly string Source =
        @"
            __kernel void " + Name + @"(__global int* input, __global int* output)
            {
                int i = get_global_id(0);
                output[i] = input[i] * input[i];
            }
        ";
    }

    class KernelSourceTest1
    {
        public static readonly string Name = @"test1";

        public static readonly string Source =
        @"
            __kernel void " + Name + @"(__global int* input, __global int* output, __global int* sum)
            {
                int i = get_global_id(0);
                int value = input[i] * input[i];
                output[i] = value;
                atomic_add(sum, value);
            }
        ";
    }
}
