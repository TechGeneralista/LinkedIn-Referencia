using Common;
using ImageProcess.Buffers;
using ImageProcess.Operations;
using OpenCLWrapper;
using UVC;


namespace UniCamV2.Tools
{
    public class ImagePreparator
    {
        public WriteableBitmapBuffer ColorImageBuffer { get; private set; }
        public WriteableBitmapBuffer MonochromeImageBuffer { get; private set; }
        public IImageSource ImageSource { get; set; }


        readonly ImageBufferBGR24 capturedImageBuffer;
        readonly FormatConvert formatConvert;
        readonly FlipVertical flipVertical;
        readonly Monochrome monochrome;


        public ImagePreparator(OpenCLAccelerator openCLAccelerator)
        {
            capturedImageBuffer = new ImageBufferBGR24(openCLAccelerator);
            formatConvert = new FormatConvert(openCLAccelerator);
            flipVertical = new FlipVertical(openCLAccelerator);
            monochrome = new Monochrome(openCLAccelerator);
        }

        internal void Capture()
        {
            if (ImageSource.IsNull())
                return;

            ImageSource.Capture();
            Prepare(ImageSource.Frame);
        }

        private void Prepare(BitmapDataDTO frame)
        {
            capturedImageBuffer.CopyToBuffer(frame.Width,frame.Height, frame.Format, frame.Buffer, frame.BufferLength);
            formatConvert.Convert(capturedImageBuffer);
            flipVertical.Flip(formatConvert.Output);
            monochrome.Convert(flipVertical.Output);

            ColorImageBuffer = flipVertical.Output;
            MonochromeImageBuffer = monochrome.Output;
        }
    }
}
