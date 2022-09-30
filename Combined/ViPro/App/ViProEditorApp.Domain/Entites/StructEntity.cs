using System.Collections.Generic;

namespace ViProEditorApp.Domain.Entites
{
    public class StructEntity
    {
        public string Name { get; set; }
        public List<StructMemberEntity> Members { get; set; }
    }
}