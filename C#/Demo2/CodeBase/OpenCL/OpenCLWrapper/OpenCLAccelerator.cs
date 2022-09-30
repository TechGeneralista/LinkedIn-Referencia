using OpenCLWrapper.Internals;
using System;
using System.Collections.Generic;


namespace OpenCLWrapper
{
    public class OpenCLAccelerator
    {
        public Context Context { get; }
        public Enqueue Enqueue { get; }


        Dictionary<string, Kernel> kernels = new Dictionary<string, Kernel>();


        public OpenCLAccelerator(DeviceTypePriority priority)
        {
            Platforms platforms = new Platforms();
            platforms.Scan();

            Devices devices = new Devices(platforms.List);
            devices.Scan();

            Device device = devices.GetDevice(priority);
            Context = new Context(device);
            CommandQueue commandQueue = new CommandQueue(Context);
            Enqueue = new Enqueue(commandQueue);
        }

        public Kernel GetKernel(string functionName, string functionSource)
        {
            if (!kernels.ContainsKey(functionName))
            {
                Program program = new Program(Context, functionName, functionSource);
                program.Build();
                kernels[functionName] = new Kernel(program);
            }

            return kernels[functionName];
        }

        public Kernel GetKernel(object name, string source)
        {
            throw new NotImplementedException();
        }
    }
}
