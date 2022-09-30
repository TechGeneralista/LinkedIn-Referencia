using AppLog;
using Common;
using Common.Interface;
using Common.NotifyProperty;
using Common.Settings;
using Communication.TCP.Server.MultiClient;
using CustomControl.ImageViewControl;
using CustomControl.PopupWindow;
using CustomControl.Trigger;
using ImageSourceDevice;
using ImageSourceDevice.UVC;
using Language;
using LogicalEvaluator;
using OpenCLWrapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UniCamApp.Instance;
using UniCamApp.Remote;
using UniCamApp.Settings;


namespace UniCamApp.Source
{
    public class SourceDC : ICanHandleImageViewControlMouseEvents, ICanSaveLoadSettings, ICanRemote
    {
        public LanguageDC LanguageDC { get; }
        public ISettableObservableProperty<int> SelectedTabItemIndex { get; } = new ObservableProperty<int>();
        public TriggerDC TriggerDC { get; }
        public INonSettableObservablePropertyArray<IImageSourceDevice> AvailableUVCDevices { get; } = new ObservablePropertyArray<IImageSourceDevice>();
        public ISettableObservableProperty<IImageSourceDevice> SelectedUVCDevice { get; } = new ObservableProperty<IImageSourceDevice>();
        public INonSettableObservablePropertyArray<object> Instances { get; } = new ObservablePropertyArray<object>();
        public ISettableObservableProperty<object> SelectedInstance { get; } = new ObservableProperty<object>();
        public ISettableObservableProperty<bool> TabItemsIsEnabled { get; } = new ObservableProperty<bool>();
        public LogicalEvaluatorDC LogicalEvaluatorDC { get; }
        public TCPServerMultiClientDC TCPServerMultiClientDC { get; }
        public SettingsDC SettingsDC { get; }


        readonly PopupWindowDC popupWindowDC;
        readonly OpenCLAccelerator openCLAccelerator;
        readonly ILog log;
        readonly string appTitle;
        readonly CommunicationParser communicationParser;


        public SourceDC(LanguageDC languageDC, PopupWindowDC popupWindowDC, OpenCLAccelerator openCLAccelerator, ILog log, ISettingsCollection settingsCollection, string appTitle)
        {
            LanguageDC = languageDC;
            this.popupWindowDC = popupWindowDC;
            this.openCLAccelerator = openCLAccelerator;
            this.log = log;
            this.appTitle = appTitle;

            Instances.CurrentValueChanged += (o, n) =>
            {
                if (n.Length != 0)
                    TriggerDC.IsEnabled.CurrentValue = true;
                else
                    TriggerDC.IsEnabled.CurrentValue = false;
            };

            SelectedInstance.CurrentValueChanged += (o, n) =>
            {
                if (n.IsNotNull())
                    TabItemsIsEnabled.CurrentValue = true;
                else
                    TabItemsIsEnabled.CurrentValue = false;
            };

            SelectedTabItemIndex.CurrentValueChanged += (o, n) =>
            {
                foreach (ICanResetContent canResetContent in Instances.CurrentValue)
                    canResetContent.ResetContent();
            };

            LogicalEvaluatorDC = new LogicalEvaluatorDC(languageDC, log, Instances);

            TriggerDC = new TriggerDC(languageDC, LogicalEvaluatorDC.IdDC.Id);
            TriggerDC.IsInternalTrigger.CurrentValueChanged += IsInternalTrigger_CurrentValueChanged;
            TriggerDC.BeforeShoot += TriggerDC_BeforeShoot;
            TriggerDC.Shoot += TriggerDC_Shoot;

            TCPServerMultiClientDC = new TCPServerMultiClientDC(languageDC, settingsCollection, log);
            SettingsDC = new SettingsDC(languageDC, settingsCollection, log, popupWindowDC, this, StopTriggers, TriggerDC.CycleAsync);

            communicationParser = new CommunicationParser(TCPServerMultiClientDC, this);
        }

        private void IsInternalTrigger_CurrentValueChanged(bool o, bool n)
        {
            foreach (IHasTriggerDC hasTriggerDC in Instances.CurrentValue)
                hasTriggerDC.TriggerDC.IsEnabled.CurrentValue = !n;
        }

        private void TriggerDC_BeforeShoot()
        {
            foreach (InstanceDC instanceDC in Instances.CurrentValue)
            {
                instanceDC.TriggerDC.Stop();
                instanceDC.CapturePrepare();
            }
        }

        private void TriggerDC_Shoot()
        {
            foreach (InstanceDC instanceDC in Instances.CurrentValue)
                (instanceDC as ICanProcess)?.Process();

            if (SelectedInstance.CurrentValue.IsNotNull())
                (SelectedInstance.CurrentValue as ICanShowResultImage)?.ShowResultImage();

            LogicalEvaluatorDC.Evulate();
        }

        internal async void ScanUVCDevicesButtonClick()
        {
            popupWindowDC.Show();
            await ScanUVCDevicesAsync();
            popupWindowDC.Close();

            if (AvailableUVCDevices.CurrentValue.Length == 1)
                SelectedUVCDevice.CurrentValue = AvailableUVCDevices.CurrentValue[0];
        }

        public Task ScanUVCDevicesAsync() => Task.Run(() => ScanUVCDevices());

        public void ScanUVCDevices()
        {
            SelectedUVCDevice.CurrentValue = null;
            AvailableUVCDevices.ForceClear();

            IImageSourceDeviceScanner deviceScanner = new UVCDeviceScanner(LanguageDC);
            deviceScanner.Scan();

            // Már elindított eszközök eltávolítása
            List<IImageSourceDevice> availableDevices = new List<IImageSourceDevice>();
            List<IImageSourceDevice> availableDevicesToRemove = new List<IImageSourceDevice>();
            availableDevices.AddRange(deviceScanner.AvailableDevices);

            foreach (InstanceDC instanceDC in Instances.CurrentValue)
            {
                IImageSourceDevice startedDevice = instanceDC.ImageSourceDevice;

                foreach (IImageSourceDevice availableDevice in availableDevices)
                {
                    if (availableDevice.Type == startedDevice.Type &&
                        availableDevice.Resolution == startedDevice.Resolution &&
                        availableDevice.FrameRate == startedDevice.FrameRate &&
                        availableDevice.Location == startedDevice.Location &&
                        availableDevice.Address == startedDevice.Address)
                    {
                        availableDevicesToRemove.Add(availableDevice);
                    }
                }
            }

            foreach (IImageSourceDevice imageSourceDevice in availableDevicesToRemove)
                availableDevices.Remove(imageSourceDevice);

            AvailableUVCDevices.ForceAddRange(availableDevices.ToArray());
        }

        internal async void CreateInstanceWithSelectedUVCDeviceButtonClick()
        {
            popupWindowDC.Show();
            await CreateInstanceWithSelectedUVCDeviceAsync();
            popupWindowDC.Close();
        }

        private Task CreateInstanceWithSelectedUVCDeviceAsync() => Task.Run(() => CreateInstanceWithSelectedUVCDevice());

        private void CreateInstanceWithSelectedUVCDevice()
        {
            if (SelectedUVCDevice.CurrentValue.IsNull())
                return;

            Instances.ForceAdd(new InstanceDC(LanguageDC, SelectedUVCDevice.CurrentValue, openCLAccelerator, log));
            AvailableUVCDevices.ForceRemove(SelectedUVCDevice.CurrentValue);
            SelectedUVCDevice.CurrentValue = null;
            SelectedInstance.CurrentValue = Instances.CurrentValue.Last();
            (SelectedInstance.CurrentValue as ICanStart)?.Start();

            if (TriggerDC.IsInternalTrigger.CurrentValue)
                (SelectedInstance.CurrentValue as IHasTriggerDC).TriggerDC.IsEnabled.CurrentValue = false;
            else
            {
                (SelectedInstance.CurrentValue as IHasTriggerDC).TriggerDC.IsEnabled.CurrentValue = true;
                (SelectedInstance.CurrentValue as IHasTriggerDC).TriggerDC.Cycle();
            }
        }

        internal async void RemoveSelectedInstanceButtonClick()
        {
            popupWindowDC.Show();
            await RemoveSelectedInstanceAsync();
            popupWindowDC.Close();
        }

        private Task RemoveSelectedInstanceAsync() => Task.Run(() => RemoveSelectedInstance());

        private void RemoveSelectedInstance()
        {
            if (SelectedInstance.CurrentValue.IsNull())
                return;

            (SelectedInstance.CurrentValue as ICanStop)?.Stop();
            Instances.ForceRemove(SelectedInstance.CurrentValue);

            IImageSourceDevice imageSourceDevice = (SelectedInstance.CurrentValue as IHasImageSourceDevice)?.ImageSourceDevice;

            if (imageSourceDevice.Type == ImageSourceDeviceTypes.UVC)
                AvailableUVCDevices.ForceAdd(imageSourceDevice);

            SelectedInstance.CurrentValue = null;
        }

        public void ImageViewControlMouseDown(Point downPosition, MouseButtonEventArgs e) => (SelectedInstance.CurrentValue as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseDown(downPosition, e);
        public void ImageViewControlMouseMove(Vector moveVector, Point movePosition, MouseEventArgs e) => (SelectedInstance.CurrentValue as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseMove(moveVector, movePosition, e);
        public void ImageViewControlMouseUp(Point upPosition, MouseButtonEventArgs e) => (SelectedInstance.CurrentValue as ICanHandleImageViewControlMouseEvents)?.ImageViewControlMouseUp(upPosition, e);


        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(Application));
            settingsCollection.KeyCreator.AddNew(nameof(PredefiniedKeys.Title));
            settingsCollection.SetValue(appTitle);

            settingsCollection.KeyCreator.ReplaceLast(nameof(SourceDC));
            TriggerDC.SaveSettings(settingsCollection);
            settingsCollection.KeyCreator.AddNew(nameof(Instances));
            settingsCollection.KeyCreator.AddNew(nameof(Instances.CurrentValue.Length));

            int length = Instances.CurrentValue.Length;
            settingsCollection.SetValue(length);

            for (int i = 0; i < length; i++)
            {
                settingsCollection.KeyCreator.ReplaceLast(i.ToString());

                IImageSourceDevice imageSourceDevice = ((IHasImageSourceDevice)Instances.CurrentValue[i]).ImageSourceDevice;
                ImageSourceDeviceTypes imageSourceDeviceType = imageSourceDevice.Type;

                settingsCollection.KeyCreator.AddNew(nameof(IImageSourceDevice));
                settingsCollection.KeyCreator.AddNew(nameof(IImageSourceDevice.Type));
                settingsCollection.SetValue(imageSourceDeviceType);

                if (imageSourceDeviceType == ImageSourceDeviceTypes.UVC)
                {
                    settingsCollection.KeyCreator.ReplaceLast(nameof(IImageSourceDevice.Resolution));
                    settingsCollection.SetValue(imageSourceDevice.Resolution);
                    settingsCollection.KeyCreator.ReplaceLast(nameof(IImageSourceDevice.Location));
                    settingsCollection.SetValue(imageSourceDevice.Location);
                }

                settingsCollection.KeyCreator.RemoveLast(2);
                (Instances.CurrentValue[i] as ICanSaveLoadSettings)?.SaveSettings(settingsCollection);
            }

            settingsCollection.KeyCreator.RemoveLast(2);
            LogicalEvaluatorDC.SaveSettings(settingsCollection);
            settingsCollection.KeyCreator.RemoveLast();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(Application));
            settingsCollection.KeyCreator.AddNew(nameof(PredefiniedKeys.Title));
            string appTitle = settingsCollection.GetValue<string>();
            settingsCollection.KeyCreator.RemoveLast();

            if (appTitle != this.appTitle)
            {
                settingsCollection.ErrorOccurred();
                log.NewMessage(LogTypes.Error, LanguageDC.SettingFileColon.CurrentValue, appTitle);
                return;
            }

            foreach (object o in Instances.CurrentValue)
            {
                SelectedInstance.CurrentValue = o;
                RemoveSelectedInstance();
            }

            ScanUVCDevices();

            settingsCollection.KeyCreator.AddNew(nameof(SourceDC));
            TriggerDC.LoadSettings(settingsCollection);
            settingsCollection.KeyCreator.AddNew(nameof(Instances));
            settingsCollection.KeyCreator.AddNew(nameof(Instances.CurrentValue.Length));

            int length = settingsCollection.GetValue<int>();

            for (int i = 0; i < length; i++)
            {
                settingsCollection.KeyCreator.ReplaceLast(i.ToString());
                settingsCollection.KeyCreator.AddNew(nameof(IImageSourceDevice));
                settingsCollection.KeyCreator.AddNew(nameof(IImageSourceDevice.Type));
                ImageSourceDeviceTypes type = settingsCollection.GetValue<ImageSourceDeviceTypes>();
                settingsCollection.KeyCreator.ReplaceLast(nameof(IImageSourceDevice.Resolution));
                string resolution = settingsCollection.GetValue<string>();
                settingsCollection.KeyCreator.ReplaceLast(nameof(IImageSourceDevice.Location));
                string location = settingsCollection.GetValue<string>();
                settingsCollection.KeyCreator.ReplaceLast(nameof(IImageSourceDevice.Address));
                string address = settingsCollection.GetValue<string>();
                settingsCollection.KeyCreator.RemoveLast(2);

                if (type == ImageSourceDeviceTypes.UVC)
                {
                    IImageSourceDevice imageSourceDevice = AvailableUVCDevices.CurrentValue.FirstOrDefault(isd => isd.Type == type && isd.Resolution == resolution && isd.Location == location);

                    if (imageSourceDevice.IsNull())
                    {
                        settingsCollection.ErrorOccurred();
                        return;
                    }

                    SelectedUVCDevice.CurrentValue = imageSourceDevice;
                    CreateInstanceWithSelectedUVCDevice();
                }

                (Instances.CurrentValue[i] as ICanSaveLoadSettings)?.LoadSettings(settingsCollection);
            }

            settingsCollection.KeyCreator.RemoveLast(2);
            LogicalEvaluatorDC.LoadSettings(settingsCollection);
            settingsCollection.KeyCreator.RemoveLast();

            SelectedTabItemIndex.CurrentValue = 0;
        }

        private void StopTriggers()
        {
            TriggerDC.Stop();

            foreach (object o in Instances.CurrentValue)
                (o as IHasTriggerDC)?.TriggerDC.Stop();
        }

        public void Remote(ref Response response, Command command, string[] ids)
        {
            switch(command)
            {
                case Command.TriggerOnce:

                    switch (ids.Length)
                    {
                        case 1:
                            TriggerDC.Remote(ref response, command, ids);
                            break;
                    }

                    break;
            }
        }
    }
}
