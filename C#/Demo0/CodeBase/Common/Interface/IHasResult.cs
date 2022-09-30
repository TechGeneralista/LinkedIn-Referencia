using Common.NotifyProperty;


namespace Common.Interface
{
    public interface IHasResult
    {
        INonSettableObservableProperty<bool?> Result { get; }
    }
}
