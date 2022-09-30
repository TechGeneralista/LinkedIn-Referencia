using Compute;
using Compute.CL;
using ImageCaptureDevice.UniversalVideoClass;


namespace ImageProcess.Buffer
{
    public class ImageBufferBGR24 : DataBuffer<byte>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }


        protected readonly Enqueue enqueue;


        public ImageBufferBGR24(ComputeAccelerator computeAccelerator) : base(computeAccelerator)
        {
            enqueue = computeAccelerator.Enqueue;
        }

        public void Download(UniversalVideoClassOutput universalVideoClassOutput)
        {
            Width = universalVideoClassOutput.Width;
            Height = universalVideoClassOutput.Height;
            Download(universalVideoClassOutput.Buffer, universalVideoClassOutput.BufferLength);
        }
    }
}
