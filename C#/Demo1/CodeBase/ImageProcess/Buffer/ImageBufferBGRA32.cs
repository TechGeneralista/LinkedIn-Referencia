using Common;
using Compute;
using Compute.CL;
using Compute.CL.Internals;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageProcess.Buffer
{
    public class ImageBufferBGRA32 : BufferImage
    {
        const string fillFunctionName = "fill_buffer";

        const string fillFunctionSource =
            @"
                __constant sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE |
                                CLK_ADDRESS_CLAMP_TO_EDGE |
                                CLK_FILTER_NEAREST;            

                __kernel void " + fillFunctionName + @"(__write_only image2d_t output, uchar4 c) 
                {
                    int2 coord = (int2)(get_global_id(0), get_global_id(1));
                    write_imageui(output, coord, (uint4)(c.s0,c.s1,c.s2,c.s3));
                }
            ";

        private readonly CLMemFlags defaultMemFlags = CLMemFlags.ReadWrite | CLMemFlags.AllocHostPtr;
        private readonly CLImageFormat defaultImageFormat = new CLImageFormat(CLChannelOrder.RGBA, CLChannelType.UnSignedInt8);


        public ImageBufferBGRA32(ComputeAccelerator computeAccelerator) : base(computeAccelerator) { }

        public void CreateIfNeed(int width, int height)
        {
            CLImageDesc imageDesc = new CLImageDesc(CLMemObjectType.Image2D, width, height);
            CreateIfNeed(defaultMemFlags, defaultImageFormat, imageDesc);
        }

        public void CreateCopyFrom(ImageBufferBGRA32 source)
        {
            CreateIfNeed(source.MemFlags, source.imageFormat, source.imageDesc);
            computeAccelerator.Enqueue.Copy(source, this);
        }

        public void Download(WriteableBitmap source)
        {
            if (source.Format != PixelFormats.Bgra32)
                throw new NotSupportedException();

            CreateIfNeed(source.PixelWidth, source.PixelHeight);

            source.Lock();
            computeAccelerator.Enqueue.WriteBuffer(this, source.BackBuffer);
            source.Unlock();
        }

        public WriteableBitmap Upload()
        {
            WriteableBitmap uploadImage = Utils.GetNewWriteableBitmapBgra32(imageDesc.Width, imageDesc.Height);
            uploadImage.Lock();
            computeAccelerator.Enqueue.ReadBuffer(this, uploadImage.BackBuffer);
            uploadImage.Unlock();
            uploadImage.Freeze();
            return uploadImage;
        }

        public void Clear(Color color)
        {
            Kernel kernel = computeAccelerator.GetKernel(fillFunctionName, fillFunctionSource);
            kernel.SetArg(0, this);
            kernel.SetArg(1, new byte[] { color.B , color.G , color.R , color.A });
            computeAccelerator.Enqueue.Execute(kernel, new SizeT[] { imageDesc.Width, imageDesc.Height });
        }
    }
}
