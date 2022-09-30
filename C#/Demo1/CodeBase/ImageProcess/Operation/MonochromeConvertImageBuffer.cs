using Compute;
using Compute.CL;
using Compute.CL.Internals;
using ImageProcess.Buffer;


namespace ImageProcess.Operation
{
    public class MonochromeConvertImageBuffer : ImageBufferBGRA32
    {
        const string functionName = "monochrome";

        const string functionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                kernel void " + functionName + @"(__read_only image2d_t input, __write_only image2d_t output)
                {
                    int2 gid = (int2)(get_global_id(0), get_global_id(1));
                    uint4 pixel = read_imageui(input, sampler, gid);
                    float4 color = convert_float4(pixel);
                    color.xyz = 0.2126*color.x + 0.7152*color.y + 0.0722*color.z;
                    pixel = convert_uint4_rte(color);
                    write_imageui(output, gid, pixel);
                }
            ";

        readonly ComputeAccelerator computeAccelerator;
        readonly Kernel kernel;


        public MonochromeConvertImageBuffer(ComputeAccelerator computeAccelerator) : base(computeAccelerator)
        {
            this.computeAccelerator = computeAccelerator;
            kernel = computeAccelerator.GetKernel(functionName, functionSource);
        }

        public void Create(ImageBufferBGRA32 input)
        {
            CreateIfNeed(input.ImageDesc.Width, input.ImageDesc.Height);
            kernel.SetArg(0, input);
            kernel.SetArg(1, this);
            computeAccelerator.Enqueue.Execute(kernel, new SizeT[] { input.ImageDesc.Width, input.ImageDesc.Height });
        }
    }
}
