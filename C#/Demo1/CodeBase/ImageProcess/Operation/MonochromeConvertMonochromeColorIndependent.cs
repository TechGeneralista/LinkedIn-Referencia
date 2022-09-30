using Compute;
using Compute.CL;
using Compute.CL.Internals;
using ImageProcess.Buffer;


namespace ImageProcess.Operation
{
    public class MonochromeConvertMonochromeColorIndependent : ImageBufferMonochrome
    {
        const string functionName = "monochrome_color_independent";

        const string functionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                kernel void " + functionName + @"(__read_only image2d_t input, __global uchar* output, int ostride)
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));
                    int oaddr = (ostride * coord.y) + coord.x;
                    uint4 color = read_imageui(input, sampler, coord);
                    output[oaddr] = convert_uint_rte(((float)color.x + (float)color.y + (float)color.z) / (float)3);
                }
            ";

        readonly Kernel kernel;


        public MonochromeConvertMonochromeColorIndependent(ComputeAccelerator computeAccelerator) : base(computeAccelerator)
        {
            kernel = computeAccelerator.GetKernel(functionName, functionSource);
        }

        public void Convert(ImageBufferBGRA32 imageBuffer)
        {
            CreateIfNeed(imageBuffer.ImageDesc.Width, imageBuffer.ImageDesc.Height);
            kernel.SetArg(0, imageBuffer);
            kernel.SetArg(1, this);
            kernel.SetArg(2, Width);
            enqueue.Execute(kernel, new SizeT[] { Width, Height });
        }

        public byte GetAveragePixelBrightness()
        {
            return 0;
        }
    }
}
