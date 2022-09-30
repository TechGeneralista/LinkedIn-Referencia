using ViProEditorApp.Domain.Inspector;

namespace ViProEditorApp.UI.Device
{
    public class DeviceViewModel : TreeViewModelBase
    {
        public DeviceInspector Inspector { get; private set; }

        public DeviceViewModel()
        {
            Inspector = new DeviceInspector();
        }
    }
}
