namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceDrawLine
    {
        public static string FunctionName = "draw_line";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                __kernel void " + FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output, uint sx, uint sy, float xinc, float yinc, uint b, uint g, uint r, uint a) 
                {
                    int currentStep = get_global_id(0);
                    int2 coord = (int2)(sx + (int)(((float)xinc * (float)currentStep)), sy + (int)(((float)yinc * (float)currentStep)));
                    uint4 pixel = read_imageui(input, sampler, coord);
                    
                    pixel.x += (uint)(((float)b-(float)pixel.x)*((float)a/(float)255));
                    pixel.y += (uint)(((float)g-(float)pixel.y)*((float)a/(float)255));
                    pixel.z += (uint)(((float)r-(float)pixel.z)*((float)a/(float)255));

                    write_imageui(output, coord, pixel);
                }
                
            ";
    }
}
