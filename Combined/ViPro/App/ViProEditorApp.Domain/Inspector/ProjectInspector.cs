using ViProEditorApp.Domain.Entites;

namespace ViProEditorApp.Domain.Inspector
{
    public enum ProjectNameInspectorState { Ok, Empty }

    public class ProjectInspector
    {
        public ProjectEntity Entity { get; private set; }

        public string Name
        {
            set
            {
                value = value.Trim();
                Entity.Name = value;
                UpdateNameState();
            }
        }

        public ProjectNameInspectorState NameState { get; private set; } = ProjectNameInspectorState.Empty;

        public ProjectInspector()
        {
            Entity = new ProjectEntity();
        }

        public ProjectInspector(ProjectEntity entity)
        {
            Entity = entity;
            UpdateNameState();
        }

        private void UpdateNameState()
            => NameState = !string.IsNullOrEmpty(Entity.Name) ? ProjectNameInspectorState.Ok : ProjectNameInspectorState.Empty;
    }
}
