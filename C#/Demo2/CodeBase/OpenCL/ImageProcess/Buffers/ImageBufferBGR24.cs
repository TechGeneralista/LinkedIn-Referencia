using Common;
using Common.Types;
using OpenCLWrapper;
using OpenCLWrapper.Internals;
using System;
using System.Windows.Media;


namespace ImageProcess.Buffers
{
    public class ImageBufferBGR24
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public PixelFormat Format { get; private set; }
        public int BytePerPixel { get; private set; }
        public int Stride { get; private set; }
        public Buffer<byte> Buffer { get; private set; }


        OpenCLAccelerator openCLAccelerator;


        public ImageBufferBGR24(OpenCLAccelerator openCLAccelerator)
        {
            this.openCLAccelerator = openCLAccelerator;
        }

        public void CopyToBuffer(int width, int height, PixelFormat format, IntPtr buffer, int bufferLength)
        {
            if (Buffer.IsNull() || Buffer.Size != bufferLength)
            {
                Buffer?.Dispose();
                Buffer = new Buffer<byte>(openCLAccelerator.Context, CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr, (SizeT)bufferLength);
                Width = width;
                Height = height;
                Format = format;
                BytePerPixel = (format.BitsPerPixel + 7) / 8;
                Stride = bufferLength / height;
            }

            openCLAccelerator.Enqueue.WriteBuffer(Buffer, buffer, (uint)bufferLength);
        }
    }
}
