using Compute;
using Compute.CL;
using Compute.CL.Internals;
using ImageProcess.Buffer;


namespace ImageProcess.Operation
{
    public class BlurMonochrome : ImageBufferMonochrome
    {
        const string functionName = "blur_monochrome";

        const string functionSource =
            @"
                int getIndex(int x, int y, int width)
                {
                    return ( y * width ) + x;
                }

                kernel void " + functionName + @"(__global uchar* input, __global uchar* output, int blur_size, int width, int height) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));

                    int fromX   = coord.x - blur_size;
                    int toX     = coord.x + blur_size;
                    int fromY   = coord.y - blur_size;
                    int toY     = coord.y + blur_size;
                    int sum   = 0;
                    int divider = 0;

                    for (int x = fromX; x <= toX; x++)
                    {
                        for (int y = fromY; y <= toY; y++)
                        {
                            if(x != 0 && y != 0)
                            {
                                sum += input[getIndex(x, y, width)];
                                divider++;
                            }
                        }
                    }

                    sum /= divider;
                    output[getIndex(coord.x, coord.y, width)] = sum;
                }
            ";

        readonly ComputeAccelerator computeAccelerator;
        readonly Kernel kernel;


        public BlurMonochrome(ComputeAccelerator computeAccelerator) : base(computeAccelerator)
        {
            this.computeAccelerator = computeAccelerator;
            kernel = computeAccelerator.GetKernel(functionName, functionSource);
        }

        public void Create(ImageBufferMonochrome input, int blurSize)
        {
            CreateIfNeed(input);
            kernel.SetArg(0, input);
            kernel.SetArg(1, this);
            kernel.SetArg(2, blurSize);
            kernel.SetArg(3, Width);
            kernel.SetArg(4, Height);
            computeAccelerator.Enqueue.Execute(kernel, new SizeT[] { input.Width, input.Height });
        }
    }
}
