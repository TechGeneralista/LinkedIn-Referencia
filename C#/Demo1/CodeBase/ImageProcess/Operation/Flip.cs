using Compute;
using Compute.CL;
using Compute.CL.Internals;
using ImageProcess.Buffer;


namespace ImageProcess.Operation
{
    public class Flip
    {
        public ImageBufferBGRA32 Output { get; }


        const string functionName = "flip_vertical";

        const string functionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                __kernel void " + functionName + @"(__read_only image2d_t input, __write_only image2d_t output, int height) 
                {
                    int2 icoord = (int2)(get_global_id(0), get_global_id(1));
                    int2 ocoord = (int2)(get_global_id(0), height - get_global_id(1));

                    uint4 pixel = read_imageui(input, sampler, icoord);
                    write_imageui(output, ocoord, pixel);
                }
                
            ";

        readonly ComputeAccelerator computeAccelerator;
        readonly Kernel kernelFlipVertical;


        public Flip(ComputeAccelerator computeAccelerator)
        {
            this.computeAccelerator = computeAccelerator;

            Output = new ImageBufferBGRA32(computeAccelerator);
            kernelFlipVertical = computeAccelerator.GetKernel(functionName, functionSource);
        }

        public void Vertical(ImageBufferBGRA32 input)
        {
            Output.CreateIfNeed(input.ImageDesc.Width, input.ImageDesc.Height);
            kernelFlipVertical.SetArg(0, input);
            kernelFlipVertical.SetArg(1, Output);
            kernelFlipVertical.SetArg(2, input.ImageDesc.Height - 1);
            computeAccelerator.Enqueue.Execute(kernelFlipVertical, new SizeT[] { input.ImageDesc.Width, input.ImageDesc.Height });
        }
    }
}
