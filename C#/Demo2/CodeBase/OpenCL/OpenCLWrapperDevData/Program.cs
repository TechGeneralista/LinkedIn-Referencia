using Common.Types;
using OpenCLWrapper;
using OpenCLWrapper.Internals;
using System;
using System.Linq;


namespace OpenCLWrapperDevData
{
    class Program
    {
        static void Main(string[] args)
        {
            OpenCLAccelerator accelerator = new OpenCLAccelerator(DeviceTypePriority.GPUCPU);

            Test0(accelerator);
            Test1(accelerator);

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }

        private static void Test0(OpenCLAccelerator accelerator)
        {
            int dataSize = 1000;
            int[] inputData = Enumerable.Range(0, (int)dataSize).ToArray();
            int[] outputData = new int[dataSize];
            
            Buffer<int> inputBuffer = new Buffer<int>(accelerator.Context, CLMemFlags.ReadOnly, dataSize);
            Buffer<int> outputBuffer = new Buffer<int>(accelerator.Context, CLMemFlags.WriteOnly, dataSize);
            
            accelerator.Enqueue.WriteBuffer(inputBuffer, inputData);

            Kernel kernel = accelerator.GetKernel(KernelSourceTest0.Name, KernelSourceTest0.Source);
            kernel.SetArg(0,inputBuffer);
            kernel.SetArg(1,outputBuffer);

            accelerator.Enqueue.Execute(kernel, null, new SizeT[] { dataSize }, null);

            accelerator.Enqueue.ReadBuffer(outputBuffer, outputData);

            //check
            uint validValuesCount = 0;
            for(int i=0;i<dataSize;i++)
            {
                if (outputData[i] == (inputData[i] * inputData[i]))
                    validValuesCount += 1;
            }

            if (validValuesCount == dataSize)
                Console.WriteLine("Test0 success");
            else
                Console.WriteLine("Test0 error");
        }

        private static void Test1(OpenCLAccelerator accelerator)
        {
            int dataSize = 10;
            int[] inputData = Enumerable.Range(0, (int)dataSize).ToArray();
            int[] outputData = new int[dataSize];
            int[] resultSum = new int[1];

            Buffer<int> inputBuffer = new Buffer<int>(accelerator.Context, CLMemFlags.ReadOnly, dataSize);
            Buffer<int> outputBuffer0 = new Buffer<int>(accelerator.Context, CLMemFlags.WriteOnly, dataSize);
            Buffer<int> outputBuffer1 = new Buffer<int>(accelerator.Context, CLMemFlags.ReadWrite, resultSum.Length);

            accelerator.Enqueue.WriteBuffer(inputBuffer, inputData);

            Kernel kernel = accelerator.GetKernel(KernelSourceTest1.Name, KernelSourceTest1.Source);
            kernel.SetArg(0,inputBuffer);
            kernel.SetArg(1,outputBuffer0);
            kernel.SetArg(2,outputBuffer1);

            accelerator.Enqueue.Execute(kernel, null, new SizeT[] { dataSize }, null);

            accelerator.Enqueue.ReadBuffer(outputBuffer0, outputData);
            accelerator.Enqueue.ReadBuffer(outputBuffer1, resultSum);

            //check
            uint validValuesCount = 0;
            int sum = 0;
            for (int i = 0; i < dataSize; i++)
            {
                int value = inputData[i] * inputData[i];
                sum += value;

                if (outputData[i] == value)
                    validValuesCount += 1;
            }

            if (validValuesCount == dataSize && resultSum[0] == sum)
                Console.WriteLine("Test1 success");
            else
                Console.WriteLine("Test1 error");
        }
    }
}
