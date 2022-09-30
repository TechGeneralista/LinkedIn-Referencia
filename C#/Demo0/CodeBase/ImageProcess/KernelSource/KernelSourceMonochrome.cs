namespace ImageProcess.KernelSource
{
    public static class KernelSourceMonochrome
    {
        public static string FunctionName = "monochrome";

        public static string FunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                kernel void "+ FunctionName + @"(__read_only image2d_t input, __write_only image2d_t output)
            {
                int2 gid = (int2)(get_global_id(0), get_global_id(1));
                uint4 pixel = read_imageui(input, sampler, gid);
                float4 color = convert_float4(pixel);
                color.xyz = 0.2126*color.x + 0.7152*color.y + 0.0722*color.z;
                pixel = convert_uint4_rte(color);
                write_imageui(output, gid, pixel);
            }
        ";
    }
}
