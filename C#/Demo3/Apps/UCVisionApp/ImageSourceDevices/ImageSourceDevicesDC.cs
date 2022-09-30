using Common;
using Common.Interfaces;
using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using Common.PopupWindow;
using Common.SaveResult;
using Common.Settings;
using Common.Trigger;
using ImageCaptureDevice;
using ImageCaptureDevice.Interfaces;
using ImageCaptureDevice.UVC;
using OpenCLWrapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCVisionApp.ImageSourceDevice;
using UCVisionApp.LevelBase;


namespace UCVisionApp.ImageSourceDevices
{
    public class ImageSourceDevicesDC : LevelBaseDC, ICanResetSelectedTabItemIndex, ICanBeforeProcess, ICanBeforeProcessCycle, ICanProcessCycle, ICanAfterProcessCycle, ICanAfterProcess, ICanRemote, ICanStopContinousTrigger, ICanShootAsync, ICanSaveLoadSettings, ICanSaveResult
    {
        public IProperty<int> SelectedTabItemIndex { get; } = new Property<int>();
        public IProperty<int> SelectedScannerTabIndex { get; } = new Property<int>();
        public TriggerDC TriggerDC { get; }
        public IReadOnlyPropertyArray<object> ImageCaptureDeviceScanners { get; } = new PropertyArray<object>();


        readonly OpenCLAccelerator openCLAccelerator;
        readonly PopupWindowDC popupWindowDC;
        readonly List<IImageCaptureDevice> startedImageCaptureDevices = new List<IImageCaptureDevice>();
        readonly SaveResultDC saveResultDC;


        public ImageSourceDevicesDC(LanguageDC languageDC, LogDC logDC, OpenCLAccelerator openCLAccelerator, PopupWindowDC popupWindowDC, SaveResultDC saveResultDC) : base(languageDC, logDC)
        {
            Name = languageDC.ImageSourceDevices;
            this.openCLAccelerator = openCLAccelerator;
            this.popupWindowDC = popupWindowDC;
            SelectedScannerTabIndex.Value = 0;
            this.saveResultDC = saveResultDC;

            TriggerDC = new TriggerDC(languageDC, LogicalEvaluatorDC.Id, this, this, saveResultDC);
            TriggerDC.IsEnabled.Value = true;

            // local scanners
            UVCDeviceScannerDC uvcDeviceScannerDC = new UVCDeviceScannerDC(languageDC, popupWindowDC);
            uvcDeviceScannerDC.StartDevice += UvcDeviceScannerDC_StartDevice;

            ImageCaptureDeviceScanners.ToSettable().Add(uvcDeviceScannerDC);
        }

        public void ResetSelectedTabItemIndex()
            => SelectedTabItemIndex.Value = 0;

        public void StopContinousTrigger()
        {
            TriggerDC.ContinousModeIsEnabled.Value = false;
            Children.ForEach(x => x.CastTo<ICanStopContinousTrigger>().StopContinousTrigger());
        }

        public Task ShootAsync(bool disableNextSaveResult = false)
            => TriggerDC.ShootAsync(disableNextSaveResult);

        #region Process
        public void BeforeProcess()
            => Children.ForEach(x => x.CastTo<ICanBeforeProcess>().BeforeProcess());

        public void BeforeProcessCycle()
            => Children.ForEach(x => x.CastTo<ICanBeforeProcessCycle>().BeforeProcessCycle());

        public void ProcessCycle()
            => Children.ForEach(x => x.CastTo<ICanProcessCycle>().ProcessCycle());

        public void AfterProcessCycle()
        {
            Children.ForEach(x => x.CastTo<ICanAfterProcessCycle>().AfterProcessCycle());
            LogicalEvaluatorDC.Evulate();
        }

        public void AfterProcess()
            => Children.ForEach(x => x.CastTo<ICanAfterProcess>().AfterProcess());
        #endregion

        #region Start Device
        private void UvcDeviceScannerDC_StartDevice(IImageCaptureDevice imageCaptureDevice)
        {
            if (TriggerDC.ContinousModeIsEnabled.Value)
                TriggerDC.ContinousModeIsEnabled.Value = false;

            startedImageCaptureDevices.Add(imageCaptureDevice);
            ImageSourceDeviceDC imageSourceDeviceDC = new ImageSourceDeviceDC(LanguageDC, logDC, openCLAccelerator, imageCaptureDevice, popupWindowDC, saveResultDC);
            imageSourceDeviceDC.TriggerDC.IsEnabled.Value = !TriggerDC.ContinousModeIsEnabled.Value;
            Children.ToSettable().Add(imageSourceDeviceDC);
        }
        #endregion

        #region Disconnect
        public async void DisconnectButtonClick()
        {
            if (SelectedChild.Value.IsNull())
                return;

            popupWindowDC.Show();
            await StopDeviceAsync();
            popupWindowDC.Close();
        }

        private Task StopDeviceAsync()
        {
            return Task.Run(() =>
            {
                if (TriggerDC.ContinousModeIsEnabled.Value)
                    TriggerDC.ContinousModeIsEnabled.Value = false;

                IHasImageCaptureDevice hasImageCaptureDevice = (IHasImageCaptureDevice)SelectedChild.Value;
                hasImageCaptureDevice.CastTo<ICanDisconnect>().Disconnect();
                SelectedChild.Value = null;
                Children.ToSettable().Remove(hasImageCaptureDevice);

                IImageCaptureDevice imageCaptureDevice = hasImageCaptureDevice.ImageCaptureDevice;
                startedImageCaptureDevices.Remove(hasImageCaptureDevice.ImageCaptureDevice);
                ImageCaptureDeviceScanners.ForEach(x => x.CastTo<ICanAddNewImageCaptureDevice>().AddNewImageCaptureDevice(imageCaptureDevice));
            });
        }
        #endregion

        #region Remote
        public string Remote(string command, string[] ids)
        {
            string response = TriggerDC.Remote(command, ids);

            if (response.IsNull())
                response = LogicalEvaluatorDC.Remote(command, ids);

            if (response.IsNull())
            {
                if (ids.Length > 1 && Id.Value == ids.First())
                {
                    string[] idsFirstRemoved = ids.RemoveFirst();

                    foreach (object o in Children.Value)
                    {
                        response = o.CastTo<ICanRemote>().Remote(command, idsFirstRemoved);

                        if (response.IsNotNull())
                            break;
                    }
                }
            }

            return response;
        }
        #endregion

        #region SaveLoad
        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            TriggerDC.SaveSettings(settingsCollection);

            settingsCollection.AddKey(nameof(Children));
            int length = Children.Value.Length;
            settingsCollection.SetProperty(length, nameof(Children.Value.Length));

            for (int index = 0; index < length; index++)
            {
                settingsCollection.AddKey(index.ToString());

                settingsCollection.SetProperty(startedImageCaptureDevices[index].Type, nameof(IImageCaptureDevice), nameof(IImageCaptureDevice.Type));
                settingsCollection.SetProperty(startedImageCaptureDevices[index].Id, nameof(IImageCaptureDevice), nameof(IImageCaptureDevice.Id));
                settingsCollection.SetProperty(startedImageCaptureDevices[index].SelectedResolution.Value.Text, nameof(IImageCaptureDevice), nameof(IImageCaptureDevice.SelectedResolution));

                Children.Value[index].CastTo<ICanSaveLoadSettings>().SaveSettings(settingsCollection);
                settingsCollection.RemoveLastKey(); // index.ToString()
            }

            settingsCollection.RemoveLastKey(); // nameof(Children)
            LogicalEvaluatorDC.SaveSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            Children.ForEach(x =>
            {
                IHasImageCaptureDevice hasImageCaptureDevice = (IHasImageCaptureDevice)x;
                Children.ToSettable().Remove(hasImageCaptureDevice);
                ((ICanDisconnect)hasImageCaptureDevice).Disconnect();
                IImageCaptureDevice imageCaptureDevice = hasImageCaptureDevice.ImageCaptureDevice;
                ImageCaptureDeviceScanners.ForEach(y => ((ICanAddNewImageCaptureDevice)y).AddNewImageCaptureDevice(imageCaptureDevice));
            });

            settingsCollection.EntryPoint(GetType().Name);
            TriggerDC.LoadSettings(settingsCollection);

            settingsCollection.AddKey(nameof(Children));
            int length = settingsCollection.GetProperty<int>(nameof(Children.Value.Length));

            for (int index = 0; index < length; index++)
            {
                settingsCollection.AddKey(index.ToString());

                ImageSourceDeviceTypes type = settingsCollection.GetProperty<ImageSourceDeviceTypes>(nameof(IImageCaptureDevice), nameof(IImageCaptureDevice.Type));
                string id = settingsCollection.GetProperty<string>(nameof(IImageCaptureDevice), nameof(IImageCaptureDevice.Id));
                string selectedResolution = settingsCollection.GetProperty<string>(nameof(IImageCaptureDevice), nameof(IImageCaptureDevice.SelectedResolution));

                IHasHandleType hasHandleType = (IHasHandleType)ImageCaptureDeviceScanners.Value.FirstOrDefault(x => ((IHasHandleType)x).HandleType == type);
                ((ICanStartImageCaptureDevice)hasHandleType).StartImageCaptureDevice(id, selectedResolution);

                IImageCaptureDevice imageCaptureDevice = ((IHasImageCaptureDevice)Children.Value[index]).ImageCaptureDevice;

                if (type != imageCaptureDevice.Type || id != imageCaptureDevice.Id || selectedResolution != imageCaptureDevice.SelectedResolution.Value.Text)
                {
                    logDC.NewMessage(LogTypes.Error, LanguageDC.ImageSourceDeviceNotFoundColon, string.Format("{0}/{1}/{2}", type, id, selectedResolution));
                    break;
                }

                Children.Value[index].CastTo<ICanSaveLoadSettings>().LoadSettings(settingsCollection);

                settingsCollection.RemoveLastKey(); // index.ToString()
            }

            settingsCollection.RemoveLastKey(); // nameof(Children)
            LogicalEvaluatorDC.LoadSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }
        #endregion

        public void SaveResult(KeyCreator keyCreator, List<SaveResultDTO> list)
        {
            keyCreator.AddNew(Id.Value);
            list.Add(new SaveResultDTO(keyCreator, Result.Value, null));
            Children.ForEach(x => x.CastTo<ICanSaveResult>()?.SaveResult(keyCreator, list));
            keyCreator.RemoveLast();
        }
    }
}
