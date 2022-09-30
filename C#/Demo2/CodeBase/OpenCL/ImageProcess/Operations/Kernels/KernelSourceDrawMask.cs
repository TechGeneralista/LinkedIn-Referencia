namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceDrawMask
    {
        public static string FunctionName = "draw_mask";

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

                __kernel void " + FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output, uchar nb, uchar ng, uchar nr, uchar a) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));
                    uint4 ipx = read_imageui(input, sampler, coord);
                    
                    if(ipx.x != 0 || ipx.y != 0 || ipx.z != 0)
                    {
                        int ob = ipx.x;
                        int og = ipx.y;
                        int or = ipx.z;

                        ob += (((int)nb - (int)ob) * (int)a) / (int)255;
                        og += (((int)ng - (int)og) * (int)a) / (int)255;
                        or += (((int)nr - (int)or) * (int)a) / (int)255;
                    
                        byteLimit(&ob);
                        byteLimit(&og);
                        byteLimit(&or);

                        write_imageui(output, coord, (uint4)(ob, og, or, 255));
                    }
                }
                
            ";
    }
}
