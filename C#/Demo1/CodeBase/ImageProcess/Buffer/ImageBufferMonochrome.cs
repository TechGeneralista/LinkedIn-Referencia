using Common;
using Compute;
using Compute.CL;
using Compute.CL.Internals;
using System.Windows.Media.Imaging;


namespace ImageProcess.Buffer
{
    public class ImageBufferMonochrome : DataBuffer<byte>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }


        const string uploadFunctionName = "upload_image_monochrome";

        const string uploadFunctionSource =
            @"
                kernel void " + uploadFunctionName + @"(__global uchar* input, int istride, __global uchar* output, int ostride)
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));
                    int iaddr = (istride * coord.y) + coord.x;
                    int oaddr = (ostride * coord.y) + (4 * coord.x);
                    output[oaddr+2] = output[oaddr+1] = output[oaddr] = input[iaddr]; // bgr
                    output[oaddr+3] = 255; // a
                }
            ";

        protected readonly Enqueue enqueue;
        readonly Kernel uploadKernel;
        readonly DataBuffer<byte> uploadHelpDataBuffer;


        public ImageBufferMonochrome(ComputeAccelerator computeAccelerator) : base(computeAccelerator)
        {
            enqueue = computeAccelerator.Enqueue;
            uploadKernel = computeAccelerator.GetKernel(uploadFunctionName, uploadFunctionSource);
            uploadHelpDataBuffer = new DataBuffer<byte>(computeAccelerator);
        }

        public void CreateIfNeed(ImageBufferMonochrome monochromeImageBuffer)
        {
            Width = monochromeImageBuffer.Width;
            Height = monochromeImageBuffer.Height;
            CreateIfNeed(Width * Height);
        }

        public void CreateIfNeed(int width, int height)
        {
            Width = width;
            Height = height;
            CreateIfNeed(width * height);
        }

        public void Upload(out WriteableBitmap writeableBitmap)
        {
            int stride = (Width * 4) + (Width % 4);
            int size = stride * Height;

            uploadHelpDataBuffer.CreateIfNeed(size);
            uploadKernel.SetArg(0, this);
            uploadKernel.SetArg(1, Width);
            uploadKernel.SetArg(2, uploadHelpDataBuffer);
            uploadKernel.SetArg(3, stride);
            enqueue.Execute(uploadKernel, new SizeT[] { Width, Height });

            writeableBitmap = Utils.GetNewWriteableBitmapBgra32(Width, Height);
            writeableBitmap.Lock();

            enqueue.ReadBuffer(uploadHelpDataBuffer, writeableBitmap.BackBuffer, size);

            writeableBitmap.Unlock();
            writeableBitmap.Freeze();
        }
    }
}
