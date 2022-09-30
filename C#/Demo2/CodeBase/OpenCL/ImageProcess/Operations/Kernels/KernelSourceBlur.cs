namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceBlur
    {
        public static string FunctionName = "blur";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                kernel void "+ FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));

                    int fromX   = coord.x - (int)1;
                    int toX     = coord.x + (int)1;
                    int fromY   = coord.y - (int)1;
                    int toY     = coord.y + (int)1;
                    uint4 sum   = 0;
                    int divider = 0;

                    for (int x = fromX; x <= toX; x++)
                    {
                        for (int y = fromY; y <= toY; y++)
                        {
                            if(x != 0 && y != 0)
                            {
                                uint4 px = read_imageui(input, sampler, (int2)(x,y));
                                sum += px;
                                divider++;
                            }
                        }
                    }

                    sum /= divider;
                    write_imageui(output, coord, sum);
                }
                
            ";
    }
}
