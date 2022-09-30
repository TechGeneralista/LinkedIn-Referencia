using Common.NotifyProperty;
using System.Windows.Media.Imaging;
using UniCamV2.Tools;


namespace UniCamV2.Content.Model
{
    public class ReferenceImages
    {
        public INonSettableObservableProperty<WriteableBitmap> Color { get; } = new ObservableProperty<WriteableBitmap>();
        public INonSettableObservableProperty<WriteableBitmap> Monochrome { get; } = new ObservableProperty<WriteableBitmap>();


        readonly ImagePreparator imagePreparator;

        public ReferenceImages(ImagePreparator imagePreparator)
        {
            this.imagePreparator = imagePreparator;
            Refresh();
        }

        public void Refresh()
        {
            imagePreparator.Capture();
            Color.ForceSet(imagePreparator.ColorImageBuffer.Upload());
            Monochrome.ForceSet(imagePreparator.MonochromeImageBuffer.Upload());
        }
    }
}