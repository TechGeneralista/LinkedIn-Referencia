namespace OpenCLWrapper.KernelSources
{
    public static class KernelSourceFillBuffer
    {
        public static string FunctionName = "fill_buffer";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                __kernel void " + FunctionName + @"(__write_only image2d_t output, uchar b, uchar g, uchar r, uchar a) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));
                    write_imageui(output, coord, (uint4)(b,g,r,a));
                }
                
            ";
    }
}
