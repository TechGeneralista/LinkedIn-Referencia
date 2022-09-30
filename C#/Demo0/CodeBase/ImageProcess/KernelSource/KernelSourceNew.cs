namespace ImageProcess.KernelSource
{
    public static class KernelSourceNew
    {
        public static string FunctionName = "copy";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                __kernel void " + FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));
                    write_imageui(output, coord, read_imageui(input, coord));
                }
                
            ";
    }
}
