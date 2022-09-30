using Common;
using Common.NotifyProperty;
using Common.Types;
using ImageProcess.Buffers;
using ImageProcess.Operations;
using OpenCLWrapper;
using OpenCLWrapper.Internals;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace OpenCLWrapperDevImage
{
    public class MainViewModel
    {
        public ObservableProperty<WriteableBitmap> OriginalImage { get; } = new ObservableProperty<WriteableBitmap>();
        public ObservableProperty<WriteableBitmap> MonochromeImage0 { get; } = new ObservableProperty<WriteableBitmap>();
        public ObservableProperty<WriteableBitmap> MonochromeImage1 { get; } = new ObservableProperty<WriteableBitmap>();
        public ObservableProperty<WriteableBitmap> MonochromeImage { get; } = new ObservableProperty<WriteableBitmap>();
        
        public ObservableProperty<WriteableBitmap> BlurredImage { get; } = new ObservableProperty<WriteableBitmap>();
        public ISettableObservableProperty<uint> BlurSize { get; } = new ObservableProperty<uint>();

        public ObservableProperty<WriteableBitmap> EdgeDetectedImage { get; } = new ObservableProperty<WriteableBitmap>();
        public ISettableObservableProperty<uint> EdgeDetectorBlurSize { get; } = new ObservableProperty<uint>();


        readonly OpenCLAccelerator accelerator;
        readonly WriteableBitmapBuffer source0;
        readonly WriteableBitmapBuffer source1;
        readonly Monochrome monochrome;
        readonly Blur blur;
        readonly EdgeDetect edgeDetect;


        public MainViewModel()
        {
            accelerator = new OpenCLAccelerator(DeviceTypePriority.GPUCPU);
            source0 = new WriteableBitmapBuffer(accelerator);
            source1 = new WriteableBitmapBuffer(accelerator);
            monochrome = new Monochrome(accelerator);
            
            blur = new Blur(accelerator);
            BlurSize.Value = 3;
            BlurSize.ValueChanged += (o) => CreateBlurredImage();

            edgeDetect = new EdgeDetect(accelerator);
            EdgeDetectorBlurSize.Value = 1;
            EdgeDetectorBlurSize.ValueChanged += (o) => CreateEdgeDetectedImage();

            Task.Run(() => RunTest());
        }

        private void RunTest()
        {
            Load();
            ConvertToMonochrome0();
            ConvertToMonochrome1();
            ConvertToMonochrome();
            CreateBlurredImage();
            CreateEdgeDetectedImage();
        }

        internal void Load()
        {
            source0.Download(Utils.LoadImageFromFile("OriginalImage.jpg", true));
            source1.Download(Utils.LoadImageFromFile("szg0.jpg", true));
            OriginalImage.Value = source0.Upload();
        }

        /// <summary>
        /// Convert image to monochrome with Buffer object
        /// </summary>
        internal void ConvertToMonochrome0()
        {
            WriteableBitmap inputImage = OriginalImage.Value;
            WriteableBitmap outputImage = new WriteableBitmap(inputImage.PixelWidth, inputImage.PixelHeight, 96, 96, inputImage.Format, null);
            int bpp = inputImage.Format.BitsPerPixel / 8;
            int size = inputImage.PixelWidth * inputImage.PixelHeight * bpp;

            Buffer<byte> inputBuffer = new Buffer<byte>(accelerator.Context, CLMemFlags.ReadOnly, size);
            Buffer<byte> outputBuffer = new Buffer<byte>(accelerator.Context, CLMemFlags.WriteOnly, size);

            accelerator.Enqueue.WriteBuffer(inputBuffer, inputImage.BackBuffer, size);

            Kernel kernel = accelerator.GetKernel(KernelSourceTest0.Name, KernelSourceTest0.Source);
            kernel.SetArg(0, inputBuffer);
            kernel.SetArg(1, outputBuffer);
            kernel.SetArg(2, inputImage.BackBufferStride);
            kernel.SetArg(3, bpp);

            accelerator.Enqueue.Execute(kernel, new SizeT[] { inputImage.PixelWidth, inputImage.PixelHeight });
            accelerator.Enqueue.ReadBuffer(outputBuffer, outputImage.BackBuffer, size);

            outputImage.Freeze();
            MonochromeImage0.Value = outputImage;
        }

        /// <summary>
        /// Convert image to monochrome with Image2D (buffer) object, use when system OpenCL support version under V1.2
        /// </summary>
        internal void ConvertToMonochrome1()
        {
            WriteableBitmap inputImage = OriginalImage.Value;
            WriteableBitmap outputImage = new WriteableBitmap(inputImage.PixelWidth, inputImage.PixelHeight, 96, 96, inputImage.Format, null);
            int size = inputImage.PixelWidth * inputImage.PixelHeight * (inputImage.Format.BitsPerPixel / 8);

            CLImageFormat imageFormat = new CLImageFormat(CLChannelOrder.RGBA, CLChannelType.UnSignedInt8);
            BufferImage2D inputBuffer = new BufferImage2D(accelerator.Context, CLMemFlags.ReadOnly, imageFormat, inputImage.PixelWidth, inputImage.PixelHeight, inputImage.BackBufferStride);
            BufferImage2D outputBuffer = new BufferImage2D(accelerator.Context, CLMemFlags.WriteOnly, imageFormat, inputImage.PixelWidth, inputImage.PixelHeight, inputImage.BackBufferStride);

            accelerator.Enqueue.WriteBuffer(inputBuffer, inputImage.BackBuffer);

            Kernel kernel = accelerator.GetKernel(KernelSourceTest1.Name, KernelSourceTest1.Source);
            kernel.SetArg(0, inputBuffer);
            kernel.SetArg(1, outputBuffer);

            accelerator.Enqueue.Execute(kernel, null, new SizeT[] { inputImage.PixelWidth, inputImage.PixelHeight }, null);
            accelerator.Enqueue.ReadBuffer(outputBuffer, outputImage.BackBuffer);

            outputImage.Freeze();
            MonochromeImage1.Value = outputImage;
        }

        /// <summary>
        /// Convert image to monochrome with Image (buffer) object, use when system OpenCL support version upper or equal V1.2
        /// </summary>
        internal void ConvertToMonochrome()
        {
            monochrome.Convert(source0);
            MonochromeImage.Value = monochrome.Output.Upload();
        }

        private void CreateBlurredImage()
        {
            blur.Create(source0, BlurSize.Value);
            BlurredImage.Value = blur.Output.Upload();
        }

        private void CreateEdgeDetectedImage()
        {
            edgeDetect.Create(source1, EdgeDetectorBlurSize.Value);
            EdgeDetectedImage.Value = edgeDetect.Upload();
        }
    }
}