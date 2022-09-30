using Common.NotifyProperty;

namespace Common.Interfaces
{
    public interface IHasId
    {
        IProperty<string> Id { get; }
    }
}
