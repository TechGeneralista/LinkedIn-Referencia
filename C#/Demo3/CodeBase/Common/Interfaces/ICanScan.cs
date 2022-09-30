using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ICanScan
    {
        void ScanButtonClick();
        Task ScanAsync();
        void Scan();
    }
}
