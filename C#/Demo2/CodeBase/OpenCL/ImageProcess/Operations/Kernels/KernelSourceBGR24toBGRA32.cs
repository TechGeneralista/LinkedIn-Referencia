namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceBGR24toBGRA32
    {
        public static string FunctionName = "bgr24tobgra32";

        public static string FunctionSource =
            @"

                __kernel void " + FunctionName + @"(__global uchar* input, __write_only image2d_t output, int istride, int ibpp) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));
                    int iaddr = (istride * coord.y) + (ibpp * coord.x);
                    write_imageui(output, coord, (uint4)(input[iaddr], input[iaddr+1], input[iaddr+2], 255));
                }
                
            ";

        
    }
}
