namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceSobel
    {
        public static string FunctionName = "sobel";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;

                constant int sobelFilter[] = {1, 2, 1, -1, -2, -1};

                kernel void " + FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output, __global uint* mOutput)
                {
                    int2 gid = (int2)(get_global_id(0), get_global_id(1));

                    int hValue =  (int)(((int)(read_imageui(input, sampler, (int2)(gid.x-1, gid.y-1)).x) * sobelFilter[0]) + 
                                        ((int)(read_imageui(input, sampler, (int2)(gid.x-1, gid.y  )).x) * sobelFilter[1]) + 
                                        ((int)(read_imageui(input, sampler, (int2)(gid.x-1, gid.y+1)).x) * sobelFilter[2]) + 
                                        ((int)(read_imageui(input, sampler, (int2)(gid.x+1, gid.y-1)).x) * sobelFilter[3]) + 
                                        ((int)(read_imageui(input, sampler, (int2)(gid.x+1, gid.y  )).x) * sobelFilter[4]) + 
                                        ((int)(read_imageui(input, sampler, (int2)(gid.x+1, gid.y+1)).x) * sobelFilter[5]));

                    int vValue =  (int)(((int)(read_imageui(input, sampler, (int2)(gid.x-1, gid.y-1)).x) * sobelFilter[0]) + 
                                        ((int)(read_imageui(input, sampler, (int2)(gid.x  , gid.y-1)).x) * sobelFilter[1]) + 
                                        ((int)(read_imageui(input, sampler, (int2)(gid.x+1, gid.y-1)).x) * sobelFilter[2]) + 
                                        ((int)(read_imageui(input, sampler, (int2)(gid.x-1, gid.y+1)).x) * sobelFilter[3]) + 
                                        ((int)(read_imageui(input, sampler, (int2)(gid.x  , gid.y+1)).x) * sobelFilter[4]) + 
                                        ((int)(read_imageui(input, sampler, (int2)(gid.x+1, gid.y+1)).x) * sobelFilter[5]));
                
                    int value = (int)sqrt((float)(hValue*hValue) + (float)(vValue*vValue));
                
                    if(value > 255)
                        value = 255;
                
                    write_imageui(output, gid, (uint4)(value,value,value,255));

                    float rad = atan2((float)vValue, (float)hValue);
                    int deg = (int)((rad * (float)180) / M_PI_F);

                    if(deg < 0)
                        deg += 180;

                    mOutput[gid.x + (get_image_width(input) * gid.y)] = (uint)deg;
                }
        ";
    }
}
