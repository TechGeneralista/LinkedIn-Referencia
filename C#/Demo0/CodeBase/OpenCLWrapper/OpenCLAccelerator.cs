using Language;
using OpenCLWrapper.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace OpenCLWrapper
{
    public class OpenCLAccelerator
    {
        public Context Context { get; private set; }
        public Enqueue Enqueue { get; private set; }


        readonly LanguageDC languageDC;
        readonly Dictionary<string, Kernel> kernels = new Dictionary<string, Kernel>();


        public OpenCLAccelerator(LanguageDC languageDC)
        {
            this.languageDC = languageDC;
        }

        public bool ScanDevice(DeviceTypePriority priority)
        {
            Platforms platforms = new Platforms();
            platforms.Scan();

            if (platforms.List.Count() == 0)
            {
                MessageBox.Show(languageDC.TheApplicationCannotBeStartedOpenCLSupportedDeviceNotFound.CurrentValue, languageDC.ErrorColon.CurrentValue , MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            Devices devices = new Devices(platforms.List);
            devices.Scan();

            Device device = devices.GetDevice(priority);
            Context = new Context(device);
            CommandQueue commandQueue = new CommandQueue(Context);
            Enqueue = new Enqueue(commandQueue);

            return true;
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
