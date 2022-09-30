namespace ImageProcess.KernelSources
{
    public static class KernelSourceBGR24ToBGRA32
    {
        public static string FunctionName = "bgr24tobgra32";

        public static string FunctionSource =
            @"

                __kernel void " + FunctionName + @"(__global uchar* input, __write_only image2d_t output, int istride) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));
                    int iaddr = (istride * coord.y) + (3 * coord.x);
                    write_imageui(output, coord, (uint4)(input[iaddr], input[iaddr+1], input[iaddr+2], 255));
                }
                
            ";

        
    }
}
