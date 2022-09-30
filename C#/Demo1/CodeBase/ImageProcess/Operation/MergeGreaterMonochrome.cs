using Compute;
using Compute.CL;
using Compute.CL.Internals;
using ImageProcess.Buffer;


namespace ImageProcess.Operation
{
    public class MergeGreaterMonochrome : ImageBufferMonochrome
    {
        const string functionName = "merge_monochrome";

        const string functionSource =
            @"
                kernel void " + functionName + @"(global uchar* input0, global uchar* input1, global uchar* output)
                {
                    uint addr = get_global_id(0) + (get_global_id(1) * get_global_size(0));
                    
                    if(input0[addr] > input1[addr])
                        output[addr] = input0[addr];

                    else if(input1[addr] >= input0[addr])
                        output[addr] = input1[addr];
                }
            ";

        readonly Kernel mergeKernel;


        public MergeGreaterMonochrome(ComputeAccelerator computeAccelerator) : base(computeAccelerator)
        {
            mergeKernel = computeAccelerator.GetKernel(functionName, functionSource);
        }

        internal void Merge(PatternDetectMonochrome patternDetect0, PatternDetectMonochrome patternDetect1)
        {
            CreateIfNeed(patternDetect0);
            mergeKernel.SetArg(0, patternDetect0);
            mergeKernel.SetArg(1, patternDetect1);
            mergeKernel.SetArg(2, this);
            enqueue.Execute(mergeKernel, new SizeT[] { patternDetect0.Width, patternDetect0.Height });
        }
    }
}
