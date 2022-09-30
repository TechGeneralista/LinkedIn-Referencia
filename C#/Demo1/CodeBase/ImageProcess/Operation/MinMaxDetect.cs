using Compute;
using Compute.CL;
using Compute.CL.Internals;
using ImageProcess.Buffer;


namespace ImageProcess.Operation
{
    public class MinMaxDetect : DataBuffer<float>
    {
        const string minMaxFunctionName = "min_max_float";
        const string minMaxFunctionSource =

           @"
             inline void atomicMax(volatile __global float *source, const float operand) 
             {
                 union 
                 {
                     unsigned int intVal;
                     float floatVal;
                 } newVal;
                    
                 union 
                 {
                     unsigned int intVal;
                     float floatVal;
                 } prevVal;

                 do 
                 {
                     prevVal.floatVal = *source;
                     newVal.floatVal = max(prevVal.floatVal, operand);
                 } while (atomic_cmpxchg((volatile __global unsigned int *)source, prevVal.intVal, newVal.intVal) != prevVal.intVal);
             }

             inline void atomicMin(volatile __global float *source, const float operand) 
             {
                union 
                {
                    unsigned int intVal;
                    float floatVal;
                } newVal;
                
                union 
                {
                    unsigned int intVal;
                    float floatVal;
                } prevVal;

                do 
                {
                    prevVal.floatVal = *source;
                    newVal.floatVal = min(prevVal.floatVal, operand);
                } while (atomic_cmpxchg((volatile __global unsigned int *)source, prevVal.intVal, newVal.intVal) != prevVal.intVal);
             }

             kernel void " + minMaxFunctionName + @"(__global uchar* input, __global float* output)
             {
                uint addr = get_global_id(0) + (get_global_id(1) * get_global_size(0));
                float fVal = (float)input[addr];
                atomicMin(&output[0], fVal);
                atomicMax(&output[1], fVal);
             }";


        readonly ComputeAccelerator computeAccelerator;
        readonly Kernel minMaxKernel;


        public MinMaxDetect(ComputeAccelerator computeAccelerator) : base(computeAccelerator)
        {
            this.computeAccelerator = computeAccelerator;
            minMaxKernel = computeAccelerator.GetKernel(minMaxFunctionName, minMaxFunctionSource);
        }

        public void Detect(ImageBufferMonochrome monochromeImageBuffer, float initialValueMinimum, float initialValueMaximum)
        {
            CreateIfNeed(2);
            Download(new float[] { initialValueMinimum, initialValueMaximum });
            minMaxKernel.SetArg(0, monochromeImageBuffer);
            minMaxKernel.SetArg(1, this);
            computeAccelerator.Enqueue.Execute(minMaxKernel, new SizeT[] { monochromeImageBuffer.Width, monochromeImageBuffer.Height });
        }
    }
}
