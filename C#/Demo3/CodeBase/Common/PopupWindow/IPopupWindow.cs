using Common.NotifyProperty;


namespace Common.PopupWindow
{
    public interface IPopupWindow
    {
        IProperty<string> Message { get; }


        void Show(string message = null);
        void Close();
    }
}
