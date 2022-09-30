using Common;
using Compute;
using ImageCaptureDevice;
using ImageCaptureDevice.UniversalVideoClass;
using ImageProcess.Buffer;
using ImageProcess.Operation;
using System;
using System.Windows.Media.Imaging;


namespace ImageProcess.ImageCaptureDevice.UniversalVideoClass
{
    public class UVCPreviewDC : DCBase
    {
        public IImageCaptureDevice ImageCaptureDevice
        {
            get => imageCaptureDevice;
            set
            {
                if (imageCaptureDevice != null)
                    imageCaptureDevice.NewImageAvailable -= UploadPreviewImage;

                imageCaptureDevice = value;

                if(imageCaptureDevice != null)
                    imageCaptureDevice.NewImageAvailable += UploadPreviewImage;
            }
        }
        IImageCaptureDevice imageCaptureDevice;

        public WriteableBitmap PreviewImage
        {
            get => previewImage;
            protected set => SetField(ref previewImage, value);
        }
        WriteableBitmap previewImage;


        readonly ImageBufferBGR24 imageBufferBGR24;
        readonly Format format;
        readonly Flip flip;


        public UVCPreviewDC(ComputeAccelerator computeAccelerator)
        {
            imageBufferBGR24 = new ImageBufferBGR24(computeAccelerator);
            format = new Format(computeAccelerator);
            flip = new Flip(computeAccelerator);
        }

        private void UploadPreviewImage(object sender, NewImageAvailableArgs e)
        {
            if(e.ImageData is UniversalVideoClassOutput universalVideoClassOutput)
            {
                imageBufferBGR24.Download(universalVideoClassOutput);
                format.ConvertToBGRA32(imageBufferBGR24);
                flip.Vertical(format);
                PreviewImage = flip.Output.Upload();
            }
        }
    }
}
