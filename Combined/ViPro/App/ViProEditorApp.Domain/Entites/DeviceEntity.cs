using System.Collections.Generic;

namespace ViProEditorApp.Domain.Entites
{
    public class DeviceEntity
    {
        public string Id { get; set; }
        public string IPAddress { get; set; }
        public int? Port { get; set; }
        public List<StructEntity> Structs { get; set; }
    }
}