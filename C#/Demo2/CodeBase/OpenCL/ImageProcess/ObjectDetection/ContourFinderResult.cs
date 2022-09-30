using Common;
using Common.Types;
using ImageProcess.Buffers;
using ImageProcess.Operations.Kernels;
using OpenCLWrapper;
using OpenCLWrapper.Internals;
using System;
using System.Collections.Generic;
using System.Windows;


namespace ImageProcess.ObjectDetection
{
    public class ContourFinderResult : IDisposable
    {
        public int Angle => transformedPointPairs.Angle;
        public ContourFinderCenterPoint[] Centers { get; private set; }


        readonly TransformedPointPairs transformedPointPairs;
        readonly OpenCLAccelerator openCLAccelerator;
        readonly int samplePiontsLength;
        readonly Buffer<int> brighterXBuffer;
        readonly Buffer<int> brighterYBuffer;
        readonly Buffer<int> darkerXBuffer;
        readonly Buffer<int> darkerYBuffer;
        readonly Buffer<byte> resultBuffer;
        readonly byte[] result;
        Point[] centers;


        public ContourFinderResult(OpenCLAccelerator openCLAccelerator, TransformedPointPairs transformedPointPairs, Size scanningAreaSize)
        {
            this.openCLAccelerator = openCLAccelerator;
            this.transformedPointPairs = transformedPointPairs;

            samplePiontsLength = transformedPointPairs.PointPairs.Length;
            int[] brighterX = new int[samplePiontsLength];
            int[] brighterY = new int[samplePiontsLength];
            int[] darkerX = new int[samplePiontsLength];
            int[] darkerY = new int[samplePiontsLength];
            result = new byte[(int)scanningAreaSize.Width * (int)scanningAreaSize.Height];

            for (int i = 0; i < samplePiontsLength; i++)
            {
                brighterX[i] = (int)Math.Round(transformedPointPairs.PointPairs[i].Brighter.X,0);
                brighterY[i] = (int)Math.Round(transformedPointPairs.PointPairs[i].Brighter.Y, 0);
                darkerX[i] = (int)Math.Round(transformedPointPairs.PointPairs[i].Darker.X, 0);
                darkerY[i] = (int)Math.Round(transformedPointPairs.PointPairs[i].Darker.Y, 0);
            }

            brighterXBuffer = new Buffer<int>(openCLAccelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, samplePiontsLength);
            brighterYBuffer = new Buffer<int>(openCLAccelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, samplePiontsLength);
            darkerXBuffer = new Buffer<int>(openCLAccelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, samplePiontsLength);
            darkerYBuffer = new Buffer<int>(openCLAccelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, samplePiontsLength);
            resultBuffer = new Buffer<byte>(openCLAccelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, result.Length);

            openCLAccelerator.Enqueue.WriteBuffer(brighterXBuffer, brighterX);
            openCLAccelerator.Enqueue.WriteBuffer(brighterYBuffer, brighterY);
            openCLAccelerator.Enqueue.WriteBuffer(darkerXBuffer, darkerX);
            openCLAccelerator.Enqueue.WriteBuffer(darkerYBuffer, darkerY);
        }

        internal void Find(WriteableBitmapBuffer writeableBitmapBuffer)
        {
            Detect(writeableBitmapBuffer);
            RemoveZeros(writeableBitmapBuffer);
            DetectCenterPoints(10);
        }

        private void DetectCenterPoints(int maxDistance)
        {
            List<ContourFinderCenterPoint> contourFinderCenterPoints = new List<ContourFinderCenterPoint>();
            
            foreach(Point cp in centers)
            {
                bool added = false;

                foreach(ContourFinderCenterPoint cfcp in contourFinderCenterPoints)
                {
                    if(Utils.GetPointsDistance(cfcp.DetectedCenterPoint, cp) < maxDistance)
                    {
                        cfcp.Add(cp);
                        added = true;
                    }
                }

                if (!added)
                    contourFinderCenterPoints.Add(new ContourFinderCenterPoint(cp));
            }

            Centers = contourFinderCenterPoints.ToArray();
        }

        private void RemoveZeros(WriteableBitmapBuffer writeableBitmapBuffer)
        {
            int width = writeableBitmapBuffer.Data.Descriptor.Width;
            int heigt = writeableBitmapBuffer.Data.Descriptor.Height;

            List<Point> points = new List<Point>();
            for(int y = 0; y < heigt; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = x + (y * width);

                    if (result[i] == 1)
                        points.Add(new Point(x, y));
                }
            }

            centers = points.ToArray();
        }

        private void Detect(WriteableBitmapBuffer writeableBitmapBuffer)
        {
            Kernel kernel = openCLAccelerator.GetKernel(KernelSourceContourFinder.FunctionName, KernelSourceContourFinder.FunctionSource);
            kernel.SetArg(0, writeableBitmapBuffer.Data);
            kernel.SetArg(1, brighterXBuffer);
            kernel.SetArg(2, brighterYBuffer);
            kernel.SetArg(3, darkerXBuffer);
            kernel.SetArg(4, darkerYBuffer);
            kernel.SetArg(5, samplePiontsLength);
            kernel.SetArg(6, resultBuffer);

            openCLAccelerator.Enqueue.Execute(kernel, new SizeT[] { writeableBitmapBuffer.Data.Descriptor.Width, writeableBitmapBuffer.Data.Descriptor.Height });
            openCLAccelerator.Enqueue.ReadBuffer(resultBuffer, result);
        }

        public void Dispose()
        {
            brighterXBuffer.Dispose();
            brighterYBuffer.Dispose();
            darkerXBuffer.Dispose();
            darkerYBuffer.Dispose();
            resultBuffer.Dispose();
        }
    }
}