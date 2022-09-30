namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceNonBlackPixelCounter
    {
        public static string FunctionName = "non_black_pixelCounter";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;

                __kernel void " + FunctionName + @"(__read_only image2d_t input, __global uint* output) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));                    
                    
                    uint4 cp = read_imageui(input, sampler, coord);

                    if(cp.x != 0 || cp.y != 0 || cp.z != 0)
                        atomic_inc((volatile __global uint*)output);
                }
                
            ";
    }
}
