using Common.SettingBackupAndRestore;
using ImageCaptureDevice;
using ImageCaptureDevice.UniversalVideoClass;
using ImageProcess.Buffer;
using ImageProcess.ImageCaptureDevice.UniversalVideoClass;
using ImageProcess.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.ImageSource.UniversalVideoClass
{
    internal class UniversalVideoClassDC : BlockItemDC, IHasImageCaptureDevice, IShutdownable, ICanBackupAndRestore, ICanShowDeviceSelector
    {
        public IImageCaptureDevice ImageCaptureDevice 
        {
            get => imageCaptureDevice;
            private set
            {
                if(value != null)
                {
                    value.NewImageAvailable += ImageCaptureDevice_NewImageAvailable;
                    value.SelectedResolutionChanged += ImageCaptureDevice_SelectedResolutionChanged;
                }

                SetField(ref imageCaptureDevice, value);
            }
        }
        IImageCaptureDevice imageCaptureDevice;

        public BlockItemDataOutputDC<ImageBufferBGRA32> BlockItemDataOutputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }


        readonly AutoResetEvent startCaptureEvent, captureFinishedEvent;
        readonly ImageBufferBGR24 imageBufferBGR24;
        readonly Format format;
        readonly Flip flip;
        bool restoreError;
        string guid, frameSizeString;
        Task task;


        public UniversalVideoClassDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataOutputDC = new BlockItemDataOutputDC<ImageBufferBGRA32>();
            BlockItemDataOutputDC.PullAction += BlockItemDataOutputDC_PullAction;
            BlockItemTriggerOutputDC = new BlockItemTriggerOutputDC(dependencyParams);

            startCaptureEvent = new AutoResetEvent(false);
            captureFinishedEvent = new AutoResetEvent(false);
            imageBufferBGR24 = new ImageBufferBGR24(dependencyParams.ComputeAccelerator);
            format = new Format(dependencyParams.ComputeAccelerator);
            flip = new Flip(dependencyParams.ComputeAccelerator);
        }

        public bool ShowDeviceSelector()
        {
            UVCScannerDC imageCaptureDeviceScannerDC = new UVCScannerDC(dependencyParams.ComputeAccelerator, GetInUseImageCaptureDevices());
            ImageCaptureDevice = imageCaptureDeviceScannerDC.ShowDialog(dependencyParams.MainWindow);
            return imageCaptureDevice != null;
        }

        private IEnumerable<IImageCaptureDevice> GetInUseImageCaptureDevices()
        {
            lock(dependencyParams.Items.Lock)
            {
                object[] items = dependencyParams.Items.Where(b => b is IHasImageCaptureDevice).ToArray();

                List<IImageCaptureDevice> imageCaptureDevices = new List<IImageCaptureDevice>();
                foreach (object item in dependencyParams.Items)
                {
                    if (item is IHasImageCaptureDevice hasImageCaptureDevice)
                        imageCaptureDevices.Add(hasImageCaptureDevice.ImageCaptureDevice);
                }

                return imageCaptureDevices;
            }
        }

        private void ImageCaptureDevice_SelectedResolutionChanged(object sender, EventArgs e)
            => dependencyParams.NotifyProjectChanged();

        private async void ImageCaptureDevice_NewImageAvailable(object sender, NewImageAvailableArgs e)
        {
            if(startCaptureEvent.WaitOne(0) && e.ImageData is UniversalVideoClassOutput universalVideoClassOutput)
            {
                imageBufferBGR24.Download(universalVideoClassOutput);
                format.ConvertToBGRA32(imageBufferBGR24);
                flip.Vertical(format);
                captureFinishedEvent.Set();
            }

            if(task == null || task.Status == TaskStatus.RanToCompletion)
                await (task = Task.Run(() => BlockItemTriggerOutputDC.Trigger()));
        }

        private void BlockItemDataOutputDC_PullAction(object sender, EventArgs e)
        {
            startCaptureEvent.Set();

            if (captureFinishedEvent.WaitOne(1000))
                BlockItemDataOutputDC.Value = flip.Output;
            else
                BlockItemDataOutputDC.Value = null;
        }

        public void Shutdown()
        {
            if(ImageCaptureDevice != null)
            {
                ImageCaptureDevice.NewImageAvailable -= ImageCaptureDevice_NewImageAvailable;
                ImageCaptureDevice.SelectedResolutionChanged -= ImageCaptureDevice_SelectedResolutionChanged;
                ImageCaptureDevice.Stop();
            }
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(Left), Left);
            bar.SetData(nameof(Top), Top);

            if(!restoreError)
            {
                bar.SetData(nameof(ImageCaptureDevice) + nameof(ImageCaptureDevice.Guid), ImageCaptureDevice.Guid);
                bar.SetData(nameof(ImageCaptureDevice) + nameof(ImageCaptureDevice.SelectedResolution) + nameof(ImageCaptureDevice.SelectedResolution.FrameSizeString), ImageCaptureDevice.SelectedResolution.FrameSizeString);
            }
            else
            {
                bar.SetData(nameof(ImageCaptureDevice) + nameof(ImageCaptureDevice.Guid), guid);
                bar.SetData(nameof(ImageCaptureDevice) + nameof(ImageCaptureDevice.SelectedResolution) + nameof(ImageCaptureDevice.SelectedResolution.FrameSizeString), frameSizeString);
            }

            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);
            Left = bar.GetData<double>(nameof(Left));
            Top = bar.GetData<double>(nameof(Top));
            string guid = bar.GetData<string>(nameof(ImageCaptureDevice) + nameof(ImageCaptureDevice.Guid));
            string frameSizeString = bar.GetData<string>(nameof(ImageCaptureDevice) + nameof(ImageCaptureDevice.SelectedResolution) + nameof(ImageCaptureDevice.SelectedResolution.FrameSizeString));
            UVCScannerDC imageCaptureDeviceScannerDC = new UVCScannerDC(dependencyParams.ComputeAccelerator);
            ImageCaptureDevice = imageCaptureDeviceScannerDC.SelectDevice(guid, frameSizeString);

            if(ImageCaptureDevice == null)
            {
                restoreError = true;
                this.guid = guid;
                this.frameSizeString = frameSizeString;
                SetStatus(Status.Error, $"A modult nem sikerült visszaállítani:\nGuid: {guid}\nFelbontás: {frameSizeString}");
            }
        }
    }
}
