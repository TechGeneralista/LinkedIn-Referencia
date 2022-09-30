using System.Collections.Generic;

namespace ViProEditorApp.Domain.Entites
{
    public class ProjectEntity
    {
        public string Name { get; set; }
        public List<DeviceEntity> Devices { get; set; }
    }
}
