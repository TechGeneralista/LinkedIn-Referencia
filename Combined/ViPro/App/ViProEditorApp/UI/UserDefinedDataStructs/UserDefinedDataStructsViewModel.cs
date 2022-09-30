using System.Collections.ObjectModel;
using ViProEditorApp.UI.UserDefinedDataStruct;

namespace ViProEditorApp.UI.UserDefinedDataStructs
{
    internal class UserDefinedDataStructsViewModel : TreeViewModelBase
    {
        public ObservableCollection<UserDefinedDataStructViewModel> UserDefinedDataStructViewModels { get; private set; }

        public UserDefinedDataStructsViewModel()
        {
            UserDefinedDataStructViewModels = new ObservableCollection<UserDefinedDataStructViewModel>();
        }

        internal void AddNewUserDefinedDataStruct()
        {

        }
    }
}
