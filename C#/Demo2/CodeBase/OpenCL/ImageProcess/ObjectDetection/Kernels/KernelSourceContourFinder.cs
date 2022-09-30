namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceContourFinder
    {
        public static string FunctionName = "contour_finder";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                kernel void "+ FunctionName + @"(   __read_only image2d_t input,
                                                    global int* brighterX, global int* brighterY,
                                                    global int* darkerX, global int* darkerY,
                                                    int samplePointsLength, global uchar* result) 
                {
                    int2 centerCoord = (int2)(get_global_id(0), get_global_id(1));
                    int resultPos = get_global_id(0) + (get_global_id(1) * get_global_size(0));

                    for(int i = 0; i < samplePointsLength; i++)
                    {
                        int2 brigtherPos = (int2)(centerCoord.x + brighterX[i], centerCoord.y + brighterY[i]);
                        int2 darkerPos = (int2)(centerCoord.x + darkerX[i], centerCoord.y + darkerY[i]);
                        uint4 brigtherPixel = read_imageui(input, sampler, brigtherPos);
                        uint4 darkerPixel = read_imageui(input, sampler, darkerPos);
                        
                        if(brigtherPixel.x <= darkerPixel.x)
                        {
                            result[resultPos] = 0;
                            return;
                        }
                    }
                    
                    result[resultPos] = 1;
                }
                
            ";
    }
}
