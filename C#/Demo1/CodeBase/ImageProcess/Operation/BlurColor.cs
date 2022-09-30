using Compute;
using Compute.CL;
using Compute.CL.Internals;
using ImageProcess.Buffer;


namespace ImageProcess.Operation
{
    public class BlurColor : ImageBufferBGRA32
    {
        const string functionName = "blur_color";

        const string functionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                kernel void " + functionName + @"(__read_only image2d_t input, __write_only image2d_t output, int size) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));

                    int fromX   = coord.x - size;
                    int toX     = coord.x + size;
                    int fromY   = coord.y - size;
                    int toY     = coord.y + size;
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

        readonly Kernel kernel;


        public BlurColor(ComputeAccelerator computeAccelerator) : base(computeAccelerator)
        {
            kernel = computeAccelerator.GetKernel(functionName, functionSource);
        }

        public void Create(ImageBufferBGRA32 input, int size)
        {
            CreateIfNeed(input.ImageDesc.Width, input.ImageDesc.Height);
            kernel.SetArg(0, input);
            kernel.SetArg(1, this);
            kernel.SetArg(2, size);
            computeAccelerator.Enqueue.Execute(kernel, new SizeT[] { input.ImageDesc.Width, input.ImageDesc.Height });
        }
    }
}
