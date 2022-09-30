namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceDilate
    {
        public static string FunctionName = "dilate";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                kernel void "+ FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output) 
                {
                    int2 centerCoord = (int2)(get_global_id(0), get_global_id(1));
                    int2 leftCoord   = (int2)(centerCoord.x - (int)1, centerCoord.y         );
                    int2 topCoord    = (int2)(centerCoord.x         , centerCoord.y - (int)1);
                    int2 rightCoord  = (int2)(centerCoord.x + (int)1, centerCoord.y         );
                    int2 bottomCoord = (int2)(centerCoord.x         , centerCoord.y + (int)1);
                    
                    uint4 centerPix = read_imageui(input, sampler, centerCoord);
                    uint4 leftPix   = read_imageui(input, sampler, leftCoord);
                    uint4 topPix    = read_imageui(input, sampler, topCoord);
                    uint4 rightPix  = read_imageui(input, sampler, rightCoord);
                    uint4 bottomPix = read_imageui(input, sampler, bottomCoord);

                    if(  (centerPix.x == 0 && centerPix.y == 0 && centerPix.z == 0) &&
                        ((leftPix.x   != 0 || leftPix.y   != 0 || leftPix.z   != 0) ||
                         (topPix.x    != 0 || topPix.y    != 0 || topPix.z    != 0) ||
                         (rightPix.x  != 0 || rightPix.y  != 0 || rightPix.z  != 0) ||
                         (bottomPix.x != 0 || bottomPix.y != 0 || bottomPix.z != 0)) )
                    {
                        write_imageui(output, centerCoord, (uint4)(255,255,255,255));
                    }
                }
                
            ";
    }
}
