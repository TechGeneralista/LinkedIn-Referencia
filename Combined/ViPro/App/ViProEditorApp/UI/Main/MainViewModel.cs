using System.Collections.ObjectModel;
using ViProEditorApp.UI.Project;
using ViProEditorApp.UI.Home;
using System.Text.Json;
using System.Text;
using System.IO;
using ViProEditorApp.Domain;
using ViProEditorApp.Domain.Entites;
using ViProEditorApp.Domain.Inspector;

namespace ViProEditorApp.UI.Main
{
    public class MainViewModel : NotifyBase
    {
        public string Title { get; private set; }
        public ObservableCollection<ProjectViewModel> ProjectViewModels { get; private set; }
        public string FilePath 
        {
            get => filePath; 
            private set => SetField(ref filePath, value);
        }
        string filePath;

        public NotifyBase TreeViewSelectedItem
        {
            get => treeViewSelectedItem;
            set => SetField(ref treeViewSelectedItem, value);
        }
        NotifyBase treeViewSelectedItem;

        public MainViewModel()
        {
#if DEBUG
            Title = $"ViPro V4.0.{Build.Number}.beta";
#else
            title = $"ViPro V4.0.{Build.Number}";
#endif

            ProjectViewModels = new ObservableCollection<ProjectViewModel>();
            ShowHome();
        }

        private void ShowHome()
            => TreeViewSelectedItem = new HomeViewModel(this);

        internal void CreateNewProject()
        {
            var projectViewModel = new ProjectViewModel();
            projectViewModel.IsSelected = true;
            ProjectViewModels.Add(projectViewModel);
        }

        internal void Close()
        {
            ProjectViewModels.Clear();
            ShowHome();
        }

        internal void OpenExistProject(string filePath)
        {
            var fileContent = File.ReadAllText(filePath, Encoding.UTF8);
            var projectEntity = JsonSerializer.Deserialize<ProjectEntity>(fileContent);
            FilePath = filePath;

            var projectInspector = new ProjectInspector(projectEntity);
            var projectViewModel = new ProjectViewModel(projectInspector);
            projectViewModel.IsSelected = true;

            ProjectViewModels.Add(projectViewModel);
        }

        internal void Save(string filePath)
        {
            var serializerOptons = new JsonSerializerOptions();
            serializerOptons.WriteIndented = true;
            var fileContent = JsonSerializer.Serialize(ProjectViewModels[0].Inspector.Entity, serializerOptons);
            File.WriteAllText(filePath, fileContent, Encoding.UTF8);
            FilePath = filePath;
        }
    }
}
