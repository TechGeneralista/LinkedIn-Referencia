using Compute;
using Compute.CL;
using Compute.CL.Internals;
using ImageProcess.Buffer;
using System;
using System.Windows.Media;


namespace ImageProcess.Operation
{
    public class BarrelDistortionRemoveColor : ImageBufferBGRA32
    {
        const string functionName = "barrel_distortion";

        const string functionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP |
                                CLK_FILTER_NEAREST;            

                __kernel void " + functionName + @"(__read_only image2d_t input, __write_only image2d_t output, float correctionRadius, float zoom) 
                {
                    float halfWidth = (float)get_image_width(output) / (float)2;
                    float halfHeight = (float)get_image_height(output) / (float)2;
                    int dX = get_global_id(0);
                    int dY = get_global_id(1);

                    float newX = dX - halfWidth;
                    float newY = dY - halfHeight;
                    
                    float distance = sqrt(pow(newX, 2) + pow(newY, 2));
                    float r = distance / correctionRadius;
                    float theta;

                    if (r == 0)
                        theta = 1;
                    else
                        theta = atan(r) / r;

                    float sX = halfWidth + (theta * newX * zoom);
                    float sY = halfHeight + (theta * newY * zoom);
                    
                    if(sX > 0 && sY > 0 && sX < (float)get_image_width(output) && sY < (float)get_image_height(output))
                        write_imageui(output, (int2)(dX, dY), read_imageui(input, sampler, (float2)(sX, sY)));
                }
                
            ";

        readonly Kernel kernel;


        public BarrelDistortionRemoveColor(ComputeAccelerator computeAccelerator) : base(computeAccelerator) 
        {
            kernel = computeAccelerator.GetKernel(functionName, functionSource);
        }

        public void Remove(ImageBufferBGRA32 source, float strength, float zoom = 1)
        {
            CreateIfNeed(source);
            Clear(Colors.Blue);
            float correctionRadius = (float)Math.Sqrt(Math.Pow((int)ImageDesc.Width, 2) + Math.Pow((int)ImageDesc.Height, 2)) / strength;

            kernel.SetArg(0, source);
            kernel.SetArg(1, this);
            kernel.SetArg(2, correctionRadius);
            kernel.SetArg(3, zoom);
            computeAccelerator.Enqueue.Execute(kernel, new SizeT[] { ImageDesc.Width, ImageDesc.Height });
        }
    }
}
