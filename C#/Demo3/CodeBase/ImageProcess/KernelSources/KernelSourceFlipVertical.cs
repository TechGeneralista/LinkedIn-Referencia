namespace ImageProcess.KernelSources
{
    public static class KernelSourceFlipVertical
    {
        public static string FunctionName = "flip_vertical";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                __kernel void " + FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output, int height) 
                {
                    int2 icoord = (int2)(get_global_id(0), get_global_id(1));
                    int2 ocoord = (int2)(get_global_id(0), height - get_global_id(1));

                    uint4 pixel = read_imageui(input, sampler, icoord);
                    write_imageui(output, ocoord, pixel);
                }
                
            ";
    }
}
