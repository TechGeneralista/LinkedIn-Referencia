using Common.Language;
using Common.Log;
using OpenCLWrapper.CL;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace OpenCLWrapper
{
    public enum DeviceTypePriority { GPUCPU, GPU, CPU }

    public class OpenCLAccelerator
    {
        public Context Context { get; private set; }
        public Enqueue Enqueue { get; private set; }


        readonly LanguageDC languageDC;
        readonly LogDC logDC;
        readonly Dictionary<string, Kernel> kernels = new Dictionary<string, Kernel>();


        public OpenCLAccelerator(LanguageDC languageDC, LogDC logDC)
        {
            this.languageDC = languageDC;
            this.logDC = logDC;
        }

        public bool ScanDevice(DeviceTypePriority priority)
        {
            Platforms platforms = new Platforms();
            platforms.Scan();

            if (platforms.List.Count() == 0)
            {
                MessageBox.Show(languageDC.TheApplicationCannotBeStartedOpenCLSupportedDeviceNotFound.Value, languageDC.ErrorColon.Value, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            Devices devices = new Devices(platforms.List);
            devices.Scan();

            Device device = devices.GetDevice(priority);
            Context = new Context(device);
            CommandQueue commandQueue = new CommandQueue(Context);
            Enqueue = new Enqueue(commandQueue);

            logDC.NewMessage(LogTypes.Information, languageDC.HardwareAccelerationColon, device.Name);

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
