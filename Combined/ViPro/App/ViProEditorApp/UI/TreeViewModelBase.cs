namespace ViProEditorApp.UI
{
    public class TreeViewModelBase : NotifyBase
    {
        public bool IsSelected
        {
            get => isSelected;
            set => SetField(ref isSelected, value);
        }
        bool isSelected = true;

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetField(ref isExpanded, value);
        }
        bool isExpanded = true;
    }
}
