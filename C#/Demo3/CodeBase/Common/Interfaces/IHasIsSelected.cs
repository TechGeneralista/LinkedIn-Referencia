using Common.NotifyProperty;


namespace Common.Interfaces
{
    public interface IHasIsSelected
    {
        IProperty<bool> IsSelected { get; }
    }
}
