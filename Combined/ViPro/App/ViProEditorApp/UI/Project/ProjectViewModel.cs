using System.Collections.ObjectModel;
using ViProEditorApp.Domain.Inspector;
using ViProEditorApp.UI.Device;

namespace ViProEditorApp.UI.Project
{
    public class ProjectViewModel : TreeViewModelBase
    {
        public ProjectInspector Inspector { get; private set; }

        public ObservableCollection<DeviceViewModel> Devices { get; private set; } = new ObservableCollection<DeviceViewModel>();

        public string Name
        {
            get => Inspector.Entity.Name;
            set
            {
                Inspector.Name = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NameState));
            }
        }

        public ProjectNameInspectorState NameState => Inspector.NameState;

        public ProjectViewModel()
        {
            Inspector = new ProjectInspector();
        }

        public ProjectViewModel(ProjectInspector inspector)
        {
            Inspector = inspector;
        }

        internal void AddNewDevice()
        {
            var viewModel = new DeviceViewModel();

            Devices.Add(viewModel);
        }
    }
}
