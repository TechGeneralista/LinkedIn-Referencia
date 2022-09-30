using System.Threading.Tasks;
using UVC.Internals;


namespace UVC
{
    public interface IImageSource
    {
        BitmapDataDTO Frame { get; }
        string Name { get; }
        string Resolution { get; }
        string FrameRate { get; }
        bool IsRunning { get; }
        DevicePropertiesDC Properties { get; }

        Task StartAsync();
        void Start();
        void Capture();
        Task StopAsync();
        void Stop();
    }
}
