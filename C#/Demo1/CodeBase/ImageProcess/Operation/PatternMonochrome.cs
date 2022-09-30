using Compute;
using ImageProcess.Buffer;


namespace ImageProcess.Operation
{
    public class PatternMonochrome : DataBuffer<float>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }



        public PatternMonochrome(ComputeAccelerator computeAccelerator) : base(computeAccelerator) { }


        public void Download(float[,] pattern)
        {
            Width = pattern.GetLength(1);
            Height = pattern.GetLength(0);
            Download(ConvertTo1D(pattern));
        }

        private float[] ConvertTo1D(float[,] pattern)
        {
            float[] retVal = new float[Width * Height];

            for (int x = 0; x < Width; x += 1)
            {
                for (int y = 0; y < Height; y += 1)
                {
                    retVal[(y * Width) + x] = pattern[y, x];
                }
            }

            return retVal;
        }
    }
}
