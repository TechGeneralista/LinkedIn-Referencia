using Compute;
using Compute.CL;
using Compute.CL.Internals;
using ImageProcess.Buffer;


namespace ImageProcess.Operation
{
    public class PatternDetectMonochrome : ImageBufferMonochrome
    {
        const string patternDetectorFunctionName = "pattern_detect";
        const string patternDetectorFunctionSource =

           @"

             kernel void " + patternDetectorFunctionName + @"(__global uchar* input, uint iwidth, __global float* output, uint offsetX, uint offsetY,
                                                              __global float* pattern, uint pWidth, uint pHeight)
             {
                uint oAddr = get_global_id(0) + (get_global_id(1) * get_global_size(0));
                float sum = 0;

                for(int y = 0; y < pHeight; y += 1)
                {
                    for(int x = 0; x < pWidth; x += 1)
                    {
                        float pValue = pattern[x + (y * pWidth)];
                        uint iAddr = (get_global_id(0) - (x - offsetX)) + ((get_global_id(1) - (y - offsetY)) * iwidth);
                        uchar iValue = input[iAddr];
                        sum += pValue * (float)iValue;
                    }   
                }

                output[oAddr] = sum;
             }";

        

        const string byteRangeConvertFunctionName = "byte_range_convert";
        const string byteRangeConvertFunctionSource =

           @"

             kernel void " + byteRangeConvertFunctionName + @"(__global float* input, __global float* min_max, __global uchar* output)
             {
                uint addr = get_global_id(0) + (get_global_id(1) * get_global_size(0));
                output[addr] = convert_uchar_rte((input[addr] - min_max[0]) * ((float)255 / (min_max[1] - min_max[0])));
             }";


        readonly ComputeAccelerator computeAccelerator;
        readonly DataBuffer<float> resultBuffer;
        readonly Kernel detectorKernel;
        readonly DataBuffer<float> minMaxBuffer;
        
        readonly Kernel byteRangeConvertKernel;


        public PatternDetectMonochrome(ComputeAccelerator computeAccelerator) : base(computeAccelerator)
        {
            this.computeAccelerator = computeAccelerator;
            resultBuffer = new DataBuffer<float>(computeAccelerator);
            detectorKernel = computeAccelerator.GetKernel(patternDetectorFunctionName, patternDetectorFunctionSource);
            minMaxBuffer = new DataBuffer<float>(computeAccelerator);
            
            byteRangeConvertKernel = computeAccelerator.GetKernel(byteRangeConvertFunctionName, byteRangeConvertFunctionSource);
        }

        public void Find(ImageBufferMonochrome monochromeImageBuffer, PatternMonochrome patternBuffer)
        {
            int offsetX = (patternBuffer.Width - 1) / 2;
            int offsetY = (patternBuffer.Height - 1) / 2;
            int outputWidth = monochromeImageBuffer.Width - (patternBuffer.Width - 1);
            int outputHeight = monochromeImageBuffer.Height - (patternBuffer.Height - 1);
            resultBuffer.CreateIfNeed(monochromeImageBuffer.Size);
            detectorKernel.SetArg(0, monochromeImageBuffer);
            detectorKernel.SetArg(1, (uint)monochromeImageBuffer.Width);
            detectorKernel.SetArg(2, resultBuffer);
            detectorKernel.SetArg(3, (uint)offsetX);
            detectorKernel.SetArg(4, (uint)offsetY);
            detectorKernel.SetArg(5, patternBuffer);
            detectorKernel.SetArg(6, (uint)patternBuffer.Width);
            detectorKernel.SetArg(7, (uint)patternBuffer.Height);
            computeAccelerator.Enqueue.Execute(detectorKernel, new SizeT[] { offsetX, offsetY }, new SizeT[] { outputWidth, outputHeight });

            

            CreateIfNeed(outputWidth, outputHeight);
            byteRangeConvertKernel.SetArg(0, resultBuffer);
            byteRangeConvertKernel.SetArg(1, minMaxBuffer);
            byteRangeConvertKernel.SetArg(2, this);
            computeAccelerator.Enqueue.Execute(byteRangeConvertKernel, new SizeT[] { outputWidth, outputHeight });
        }
    }
}
