using Common.NotifyProperty;


namespace Common.Interfaces
{
    public interface ICanResetSelectedTabItemIndex
    {
        IProperty<int> SelectedTabItemIndex { get; }

        void ResetSelectedTabItemIndex();
    }
}
