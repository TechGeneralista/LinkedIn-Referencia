namespace ImageProcess.KernelSource
{
    public static class KernelSourceBarrelDistortion
    {
        public static string FunctionName = "barrel_distortion";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP |
                                CLK_FILTER_NEAREST;            

                __kernel void " + FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output, float correctionRadius, float strength, float zoom) 
                {
                    float halfWidth = (float)get_image_width(output) / (float)2;
                    float halfHeight = (float)get_image_height(output) / (float)2;
                    int dX = get_global_id(0);
                    int dY = get_global_id(1);

                    float newX = dX - halfWidth;
                    float newY = dY - halfHeight;
                    
                    float distance = sqrt(pow(newX, 2) + pow(newY, 2));
                    float r = distance / correctionRadius;
                    float theta;

                    if (r == 0)
                        theta = 1;
                    else
                        theta = atan(r) / r;

                    float sX = halfWidth + (theta * newX * zoom);
                    float sY = halfHeight + (theta * newY * zoom);
                    
                    write_imageui(output, (int2)(dX, dY), read_imageui(input, sampler, (float2)(sX, sY)));
                }
                
            ";
    }
}
