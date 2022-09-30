using Common;
using ImageProcess.Buffers;
using OpenCLWrapper;
using System.Collections.Generic;
using System.Windows;


namespace ImageProcess.ObjectDetection
{
    public class ContourFinder
    {
        public Size RectangleSize { get; private set; }
        public ContourFinderResult[] Results { get; private set; }


        readonly OpenCLAccelerator openCLAccelerator;


        public ContourFinder(OpenCLAccelerator openCLAccelerator)
        {
            this.openCLAccelerator = openCLAccelerator;
        }

        public void Prepare(WriteableBitmapBuffer writeableBitmapBuffer, UserContour userContour)
        {
            FreeUnusedResources();
            RectangleSize = userContour.Size;
            int imageWidth = writeableBitmapBuffer.Data.Descriptor.Width;
            int imageHeight = writeableBitmapBuffer.Data.Descriptor.Height;

            Size scanningAreaSize = new Size(imageWidth, imageHeight);

            List<ContourFinderResult> contourFinderResults = new List<ContourFinderResult>();
            foreach (TransformedPointPairs tpp in userContour.TransformedPointPairs)
                contourFinderResults.Add(new ContourFinderResult(openCLAccelerator, tpp, scanningAreaSize));

            this.Results = contourFinderResults.ToArray();
            Find(writeableBitmapBuffer);
        }

        private void FreeUnusedResources()
        {
            if(Results.IsNotNull() && Results.Length != 0)
            {
                foreach (ContourFinderResult cfr in Results)
                    cfr.Dispose();
            }
        }

        private void Find(WriteableBitmapBuffer writeableBitmapBuffer)
        {
            foreach (ContourFinderResult cfr in Results)
                cfr.Find(writeableBitmapBuffer);
        }
    }
}
