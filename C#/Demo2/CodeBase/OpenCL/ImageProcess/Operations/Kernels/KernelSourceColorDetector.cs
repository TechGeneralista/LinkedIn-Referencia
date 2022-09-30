namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceColorDetector
    {
        public static string FunctionName = "color_detector";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;

                __kernel void " + FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output, uint tolerance, uchar rb, uchar rg, uchar rr) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));                    
                    
                    uint4 cp = read_imageui(input, sampler, coord);
                    int db = (int)rb - (int)cp.x;
                    int dg = (int)rg - (int)cp.y;
                    int dr = (int)rr - (int)cp.z;
                    uint diff = (uint)(sqrt( (float)(db * db) + (float)(dg * dg) + (float)(dr * dr) ));
                    
                    //tolerance = tolerance * tolerance;

                    if(diff < tolerance)
                        write_imageui(output, coord, (uint4)(255, 255, 255, 255));
                        //atomic_inc((volatile __global uint*)counter);
                }
                
            ";
    }
}
