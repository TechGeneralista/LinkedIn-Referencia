using Common.NotifyProperty;
using ImageProcess;
using ImageProcess.Buffers;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace UniCamApp.Tasks
{
    public interface ITask
    {
        string TypeName { get; }
        ISettableObservableProperty<string> Id { get; }
        WriteableBitmapBuffer LastInputImageBuffer { get; }
        SelectionRectangle SelectionRectangle { get; }
        INonSettableObservableProperty<WriteableBitmap> ResultImage { get; }
        INonSettableObservableProperty<bool> Result { get; }
        UserControl PropertiesView { get; }


        void Start(WriteableBitmapBuffer source);
        void DrawSelecionRectangle(WriteableBitmapBuffer destination, bool isSelected);
    }
}
