using System.Threading.Tasks;


namespace Common.Interfaces
{
    public interface ICanDisconnect
    {
        void DisconnectButtonClick();
        Task DisconnectAsync();
        void Disconnect();
    }
}
