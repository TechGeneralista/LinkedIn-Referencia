using System.Threading.Tasks;


namespace Common.Interfaces
{
    public interface ICanShootAsync
    {
        Task ShootAsync(bool disableNextSaveResult = false);
    }
}
