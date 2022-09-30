using Common.NotifyProperty;
using System.Windows.Media;


namespace Common.Interfaces
{
    public interface ICanDrawResultImage
    {
        IReadOnlyProperty<ImageSource> ResultImage { get; }

        void DrawResultImage();
    }
}
