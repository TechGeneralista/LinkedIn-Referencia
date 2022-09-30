using Common.NotifyProperty;


namespace Common.Interfaces
{
    public interface IHasResult
    {
        IReadOnlyProperty<bool?> Result { get; }
    }
}
