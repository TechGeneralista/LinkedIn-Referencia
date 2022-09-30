using Common;
using ImageProcess.Buffer;
using ImageProcess.ContourFinder.Detector;
using ImageProcess.ContourFinder.UserContourPath;
using ImageProcess.KernelSource;
using OpenCLWrapper;
using OpenCLWrapper.Internals;
using System;
using System.Collections.Generic;
using System.Windows;
 

namespace ImageProcess.Templates
{
    public class PointPairsCenterFinder : IDisposable
    {
        public int Angle { get; private set; }
        public Point[] Centers { get; private set; }
        public AverageCenterCalculator[] CenterPointGroups { get; private set; }


        readonly OpenCLAccelerator openCLAccelerator;
        readonly Kernel kernelCenterFinder;
        readonly Kernel kernelCenterCounter;
        readonly Kernel kernelCenterPositionGetter;
        readonly int pointPairsLength;
        readonly Buffer<int> brighterXBuffer;
        readonly Buffer<int> brighterYBuffer;
        readonly Buffer<int> darkerXBuffer;
        readonly Buffer<int> darkerYBuffer;
        readonly DataBuffer<byte> centerFinderResultBuffer;
        readonly DataBuffer<uint> counterBuffer;
        uint[] counterArray;
        readonly DataBuffer<uint> centerXPositionBuffer;
        uint[] centerXPositionArray;
        readonly DataBuffer<uint> centerYPositionBuffer;
        uint[] centerYPositionArray;


        public PointPairsCenterFinder(OpenCLAccelerator openCLAccelerator, int angle, PointPair[] pointPairs)
        {
            this.openCLAccelerator = openCLAccelerator;
            Angle = angle;

            kernelCenterFinder = openCLAccelerator.GetKernel(KernelSourceCenterFinder.FunctionName, KernelSourceCenterFinder.FunctionSource);
            kernelCenterCounter = openCLAccelerator.GetKernel(KernelSourceCenterCounter.FunctionName, KernelSourceCenterCounter.FunctionSource);
            kernelCenterPositionGetter = openCLAccelerator.GetKernel(KernelSourceCenterPositionGetter.FunctionName, KernelSourceCenterPositionGetter.FunctionSource);

            centerFinderResultBuffer = new DataBuffer<byte>(openCLAccelerator);
            counterBuffer = new DataBuffer<uint>(openCLAccelerator);
            centerXPositionBuffer = new DataBuffer<uint>(openCLAccelerator);
            centerYPositionBuffer = new DataBuffer<uint>(openCLAccelerator);

            pointPairsLength = pointPairs.Length;
            if (pointPairsLength == 0)
                return;

            int[] brighterX = new int[pointPairsLength];
            int[] brighterY = new int[pointPairsLength];
            int[] darkerX = new int[pointPairsLength];
            int[] darkerY = new int[pointPairsLength];

            for (int i = 0; i < pointPairsLength; i++)
            {
                brighterX[i] = (int)Math.Round(pointPairs[i].Brighter.X, 0);
                brighterY[i] = (int)Math.Round(pointPairs[i].Brighter.Y, 0);
                darkerX[i] = (int)Math.Round(pointPairs[i].Darker.X, 0);
                darkerY[i] = (int)Math.Round(pointPairs[i].Darker.Y, 0);
            }

            brighterXBuffer = new Buffer<int>(openCLAccelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, pointPairsLength);
            brighterYBuffer = new Buffer<int>(openCLAccelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, pointPairsLength);
            darkerXBuffer = new Buffer<int>(openCLAccelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, pointPairsLength);
            darkerYBuffer = new Buffer<int>(openCLAccelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, pointPairsLength);

            openCLAccelerator.Enqueue.WriteBuffer(brighterXBuffer, brighterX);
            openCLAccelerator.Enqueue.WriteBuffer(brighterYBuffer, brighterY);
            openCLAccelerator.Enqueue.WriteBuffer(darkerXBuffer, darkerX);
            openCLAccelerator.Enqueue.WriteBuffer(darkerYBuffer, darkerY);
        }

        public void FindCenters(Buffer.Image2DBuffer imageBuffer, byte sensitivity, byte match)
        {
            int width = imageBuffer.BufferImage.Descriptor.Width;
            int height = imageBuffer.BufferImage.Descriptor.Height;

            // find
            centerFinderResultBuffer.Create(imageBuffer.BufferImage.Descriptor.Width * imageBuffer.BufferImage.Descriptor.Height);

            if (pointPairsLength != 0)
            {
                kernelCenterFinder.SetArg(0, imageBuffer.BufferImage);
                kernelCenterFinder.SetArg(1, brighterXBuffer);
                kernelCenterFinder.SetArg(2, brighterYBuffer);
                kernelCenterFinder.SetArg(3, darkerXBuffer);
                kernelCenterFinder.SetArg(4, darkerYBuffer);
                kernelCenterFinder.SetArg(5, pointPairsLength);
                kernelCenterFinder.SetArg(6, sensitivity);
                kernelCenterFinder.SetArg(7, match);
                kernelCenterFinder.SetArg(8, centerFinderResultBuffer.Buffer);

                openCLAccelerator.Enqueue.Execute(kernelCenterFinder, new SizeT[] { width, height });
                openCLAccelerator.Enqueue.CommandQueue.Flush();
                openCLAccelerator.Enqueue.CommandQueue.Finish();
            }

            // count
            counterBuffer.Create(1);
            counterArray = new uint[1];
            counterBuffer.Download(counterArray);

            kernelCenterCounter.SetArg(0, centerFinderResultBuffer.Buffer);
            kernelCenterCounter.SetArg(1, counterBuffer.Buffer);
            openCLAccelerator.Enqueue.Execute(kernelCenterCounter, new SizeT[] { width, height });
            openCLAccelerator.Enqueue.ReadBuffer(counterBuffer.Buffer, counterArray);

            // get positions
            int count = (int)counterArray[0];

            if (count == 0)
            {
                this.Centers = new Point[0];
                return;
            }

            counterArray[0] = 0;
            counterBuffer.Download(counterArray);

            centerXPositionBuffer.Create(count);
            centerXPositionArray = new uint[count];
            centerYPositionBuffer.Create(count);
            centerYPositionArray = new uint[count];

            kernelCenterPositionGetter.SetArg(0, centerFinderResultBuffer.Buffer);
            kernelCenterPositionGetter.SetArg(1, counterBuffer.Buffer);
            kernelCenterPositionGetter.SetArg(2, centerXPositionBuffer.Buffer);
            kernelCenterPositionGetter.SetArg(3, centerYPositionBuffer.Buffer);
            openCLAccelerator.Enqueue.Execute(kernelCenterPositionGetter, new SizeT[] { width, height });
            openCLAccelerator.Enqueue.ReadBuffer(centerXPositionBuffer.Buffer, centerXPositionArray);
            openCLAccelerator.Enqueue.ReadBuffer(centerYPositionBuffer.Buffer, centerYPositionArray);

            List<Point> centers = new List<Point>();
            for (uint i = 0; i < count; i++)
                centers.Add(new Point(centerXPositionArray[i], centerYPositionArray[i]));

            Centers = centers.ToArray();
        }

        public void GroupCenterPoints(double groupTolerance)
        {
            List<AverageCenterCalculator> centerPointGroups = new List<AverageCenterCalculator>();
            foreach (Point centerPoint in Centers)
            {
                bool isAdded = false;
                foreach (AverageCenterCalculator centerPointGroup in centerPointGroups)
                {
                    if (centerPointGroup.AbsoluteCenter.Distance(centerPoint) < groupTolerance)
                    {
                        centerPointGroup.Add(centerPoint);
                        isAdded = true;
                        break;
                    }
                }

                if (!isAdded)
                {
                    AverageCenterCalculator centerPointGroup = new AverageCenterCalculator();
                    centerPointGroup.Add(centerPoint);
                    centerPointGroups.Add(centerPointGroup);
                }
            }

            CenterPointGroups = centerPointGroups.ToArray();
        }

        public void Dispose()
        {
            brighterXBuffer.Dispose();
            brighterYBuffer.Dispose();
            darkerXBuffer.Dispose();
            darkerYBuffer.Dispose();
            centerFinderResultBuffer.Dispose();
            counterBuffer.Dispose();
            centerXPositionBuffer.Dispose();
            centerYPositionBuffer.Dispose();
        }
    }
}
