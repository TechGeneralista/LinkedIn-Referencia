using Compute.CL;
using System.Collections.Generic;
using System.Linq;


namespace Compute
{
    public enum ComputeDeviceTypePriority { GPUCPU, GPU, CPU }

    public class ComputeAccelerator
    {
        public Context Context { get; private set; }
        public Enqueue Enqueue { get; private set; }
        public bool InitialisationError { get; private set; }


        readonly Dictionary<string, Kernel> kernels = new Dictionary<string, Kernel>();


        public ComputeAccelerator(ComputeDeviceTypePriority priority)
        {
            Platforms platforms = new Platforms();
            platforms.Scan();

            if (platforms.List.Count() == 0)
            {
                InitialisationError = true;
                return;
            }

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
    }
}
