using ViProEditorApp.UI.Main;

namespace ViProEditorApp.UI.Home
{
    internal class HomeViewModel : NotifyBase
    {
        readonly MainViewModel mainViewModel;

        public HomeViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        internal void CreateNewProject()
            => mainViewModel.CreateNewProject();

        internal void OpenExistProject(string fileName)
            => mainViewModel.OpenExistProject(fileName);
    }
}
