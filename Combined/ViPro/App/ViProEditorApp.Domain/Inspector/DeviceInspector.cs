using ViProEditorApp.Domain.Entites;

namespace ViProEditorApp.Domain.Inspector
{
    public class DeviceInspector
    {
        public DeviceEntity Entity { get; private set; }

        public DeviceInspector()
        {
            Entity = new DeviceEntity();
        }
    }
}
