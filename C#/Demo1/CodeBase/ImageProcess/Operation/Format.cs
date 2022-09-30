using Compute;
using Compute.CL;
using Compute.CL.Internals;
using ImageProcess.Buffer;
using System;

namespace ImageProcess.Operation
{
    public class Format : ImageBufferBGRA32
    {
        const string functionName = "bgr24tobgra32";

        const string functionSource =
            @"

                __kernel void " + functionName + @"(__global uchar* input, __write_only image2d_t output, int istride) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));
                    int iaddr = (istride * coord.y) + (3 * coord.x);
                    write_imageui(output, coord, (uint4)(input[iaddr], input[iaddr+1], input[iaddr+2], 255));
                }
                
            ";

        readonly Kernel kernel;


        public Format(ComputeAccelerator computeAccelerator) : base(computeAccelerator)
        {
            kernel = computeAccelerator.GetKernel(functionName, functionSource);
        }

        public void ConvertToBGRA32(ImageBufferBGR24 imageBufferBGR24)
        {
            CreateIfNeed(imageBufferBGR24.Width, imageBufferBGR24.Height);
            kernel.SetArg(0, imageBufferBGR24);
            kernel.SetArg(1, this);
            kernel.SetArg(2, imageBufferBGR24.Width * 3 + (imageBufferBGR24.Width % 4));
            computeAccelerator.Enqueue.Execute(kernel, new SizeT[] { imageBufferBGR24.Width, imageBufferBGR24.Height });
        }
    }
}
