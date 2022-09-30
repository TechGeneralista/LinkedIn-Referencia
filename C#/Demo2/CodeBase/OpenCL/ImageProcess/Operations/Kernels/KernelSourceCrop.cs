namespace ImageProcess.Operations.Kernels
{
    public static class KernelSourceCrop
    {
        public static string FunctionName = "crop";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                __kernel void " + FunctionName + @"(__read_only image2d_t source, __write_only image2d_t destination, int sourceXoffset, int sourceYoffset) 
                {
                    int2 scoord = (int2)(get_global_id(0) + sourceXoffset, get_global_id(1) + sourceYoffset);
                    int2 dcoord = (int2)(get_global_id(0), get_global_id(1));                    

                    write_imageui(destination, dcoord, read_imageui(source, sampler, scoord));
                }
                
            ";
    }
}
