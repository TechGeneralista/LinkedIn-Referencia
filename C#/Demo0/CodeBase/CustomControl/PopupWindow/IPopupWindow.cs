using Common.NotifyProperty;


namespace CustomControl.PopupWindow
{
    public interface IPopupWindow
    {
        ISettableObservableProperty<string> Message { get; }


        void Show(string message = null);
        void Close();
    }
}
