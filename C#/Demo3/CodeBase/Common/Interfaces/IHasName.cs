using Common.NotifyProperty;

namespace Common.Interfaces
{
    public interface IHasName
    {
        IReadOnlyProperty<string> Name { get; }
    }
}
