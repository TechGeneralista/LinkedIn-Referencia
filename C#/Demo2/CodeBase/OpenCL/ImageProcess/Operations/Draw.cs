using Common.Types;
using ImageProcess.Buffers;
using ImageProcess.Operations.Kernels;
using OpenCLWrapper;
using System;
using System.Windows.Media;


namespace ImageProcess.Operations
{
    public class Draw
    {
        OpenCLAccelerator openCLAccelerator;

        public Draw(OpenCLAccelerator openCLAccelerator)
        {
            this.openCLAccelerator = openCLAccelerator;
        }

        public void Rectangle(WriteableBitmapBuffer destination, System.Drawing.Rectangle rectangle, Color fill, Color outline, int thickness)
        {
            FillRectangle(destination, rectangle, fill);

            for(int i=0;i<thickness;i++)
            {
                Line(destination, rectangle.X, rectangle.Y+i, rectangle.X + rectangle.Width, rectangle.Y+i, outline);
                Line(destination, rectangle.X + rectangle.Width-i, rectangle.Y, rectangle.X + rectangle.Width-i, rectangle.Y + rectangle.Height, outline);
                Line(destination, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height-i, rectangle.X, rectangle.Y + rectangle.Height-i, outline);
                Line(destination, rectangle.X+i, rectangle.Y + rectangle.Height, rectangle.X+i, rectangle.Y, outline);
            }
        }

        public void Line(WriteableBitmapBuffer destination, int sx, int sy, int ex, int ey, Color color)
        {
            int dx = ex - sx;
            int dy = ey - sy;

            int steps = Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy);

            float xinc = (float)dx / (float)steps;
            float yinc = (float)dy / (float)steps;

            Kernel kernel = openCLAccelerator.GetKernel(KernelSourceDrawLine.FunctionName, KernelSourceDrawLine.FunctionSource);
            kernel.SetArg(0, destination.Data);
            kernel.SetArg(1, destination.Data);
            kernel.SetArg(2, sx);
            kernel.SetArg(3, sy);
            kernel.SetArg(4, xinc);
            kernel.SetArg(5, yinc);
            kernel.SetArg(6, (uint)color.B);
            kernel.SetArg(7, (uint)color.G);
            kernel.SetArg(8, (uint)color.R);
            kernel.SetArg(9, (uint)color.A);
            openCLAccelerator.Enqueue.Execute(kernel, new SizeT[] { steps });
        }

        public void FillRectangle(WriteableBitmapBuffer destination, System.Drawing.Rectangle rectangle, Color fill)
        {
            Kernel kernel = openCLAccelerator.GetKernel(KernelSourceDrawFillRectangle.FunctionName, KernelSourceDrawFillRectangle.FunctionSource);
            kernel.SetArg(0, destination.Data);
            kernel.SetArg(1, destination.Data);
            kernel.SetArg(2, (int)fill.B);
            kernel.SetArg(3, (int)fill.G);
            kernel.SetArg(4, (int)fill.R);
            kernel.SetArg(5, (int)fill.A);
            openCLAccelerator.Enqueue.Execute(kernel, new SizeT[] { rectangle.X, rectangle.Y }, new SizeT[] { rectangle.Width, rectangle.Height });
        }

        public void Mask(WriteableBitmapBuffer input, Color color, WriteableBitmapBuffer output)
        {
            Kernel kernel = openCLAccelerator.GetKernel(KernelSourceDrawMask.FunctionName, KernelSourceDrawMask.FunctionSource);
            kernel.SetArg(0, input.Data);
            kernel.SetArg(1, output.Data);
            kernel.SetArg(2, color.B);
            kernel.SetArg(3, color.G);
            kernel.SetArg(4, color.R);
            kernel.SetArg(5, color.A);
            openCLAccelerator.Enqueue.Execute(kernel, new SizeT[] { input.Data.Descriptor.Width, input.Data.Descriptor.Height });
        }
    }
}
