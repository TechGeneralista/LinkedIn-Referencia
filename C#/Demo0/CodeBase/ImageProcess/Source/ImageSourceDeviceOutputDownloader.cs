using ImageProcess.Buffer;
using ImageProcess.Templates;
using ImageSourceDevice;
using OpenCLWrapper;


namespace ImageProcess.Source
{
    public class ImageSourceDeviceOutputDownloader
    {
        public Image2DBuffer OutputImageBuffer { get; private set; }


        readonly DataBuffer<byte> dataBuffer;
        readonly FormatConvert formatConvert;
        readonly FlipVertical flipVertical;


        public ImageSourceDeviceOutputDownloader(OpenCLAccelerator openCLAccelerator)
        {
            dataBuffer = new DataBuffer<byte>(openCLAccelerator);
            formatConvert = new FormatConvert(openCLAccelerator);
            flipVertical = new FlipVertical(openCLAccelerator);
        }

        public void Download(IImageSourceDeviceOutput deviceOutput)
        {
            switch(deviceOutput.Type)
            {
                case ImageSourceDeviceOutputTypes.IntPtr:

                    dataBuffer.Download(deviceOutput.Buffer, deviceOutput.BufferLength);
                    formatConvert.ConvertBGR24ToBGRA32(deviceOutput.Width, deviceOutput.Height, deviceOutput.Stride, dataBuffer);
                    flipVertical.Flip(formatConvert.Output);
                    OutputImageBuffer = flipVertical.Output;
                    break;
            }
        }
    }
}
