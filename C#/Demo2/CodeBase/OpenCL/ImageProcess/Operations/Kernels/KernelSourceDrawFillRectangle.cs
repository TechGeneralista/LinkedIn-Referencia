namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceDrawFillRectangle
    {
        public static string FunctionName = "draw_fill_rectangle";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            
                
                void byteLimit(int* value)
                {
                    if(*value < 0)
                        *value = 0;

                    else if(*value > 255)
                        *value = 255;
                }

                __kernel void " + FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output, int nb, int ng, int nr, int a) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));
                    int4 pixel = read_imagei(input, sampler, coord);
                    
                    int ob = pixel.x;
                    int og = pixel.y;
                    int or = pixel.z;

                    ob += ((nb - ob) * a) / (int)255;
                    og += ((ng - og) * a) / (int)255;
                    or += ((nr - or) * a) / (int)255;
                    
                    byteLimit(&ob);
                    byteLimit(&og);
                    byteLimit(&or);

                    write_imagei(output, coord, (int4)(ob, og, or, 255));
                }
                
            ";
    }
}
