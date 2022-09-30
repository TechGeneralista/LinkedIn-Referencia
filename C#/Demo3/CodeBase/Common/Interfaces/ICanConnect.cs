using System.Threading.Tasks;


namespace Common.Interfaces
{
    public interface ICanConnect
    {
        void ConnectButtonClick();
        Task ConnectAsync();
        void Connect();
    }
}
