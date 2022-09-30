namespace OpenCLWrapperDevImage
{
    class KernelSourceTest0
    {
        public static readonly string Name = @"test0";

        public static readonly string Source =
        @"
            kernel void " + Name + @"(global uchar* input, global uchar* output, int stride, int bpp)
            {
                int x = get_global_id(0);
                int y = get_global_id(1);

                int addr = (x*bpp)+(y*stride);
                uint mv = (uint)((0.2126*(float)input[addr+2]) + (0.7152*(float)input[addr+1]) + (0.0722*(float)input[addr]));

                output[addr] = mv;     //b
                output[addr+1] = mv;   //g
                output[addr+2] = mv;   //r
                output[addr+3] = input[addr+3];  //a
            }
        ";
    }

    class KernelSourceTest1
    {
        public static readonly string Name = @"test1";

        public static readonly string Source =
        @"
            __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                           CLK_ADDRESS_CLAMP_TO_EDGE |
                                           CLK_FILTER_NEAREST;


            __kernel void " + Name + @"(__read_only image2d_t input, __write_only image2d_t output)
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
