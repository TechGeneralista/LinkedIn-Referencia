namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceNonMaxSupress
    {
        public static string FunctionName = "non_max_supress";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                kernel void "+ FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output)
            {
                
            }
        ";
    }
}
