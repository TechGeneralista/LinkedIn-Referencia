using Common;
using Common.Interfaces;
using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using Common.PopupWindow;
using Common.SaveResult;
using Common.Settings;
using Common.Tool;
using Common.Trigger;
using ImageCaptureDevice;
using ImageCaptureDevice.Interfaces;
using ImageProcess.Source;
using LogicalEvaluator;
using OpenCLWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UCVisionApp.Inspection;
using UCVisionApp.LevelBase;


namespace UCVisionApp.ImageSourceDevice
{
    public class ImageSourceDeviceDC : LevelBaseDC, ICanResetSelectedTabItemIndex, IHasImageCaptureDevice, ICanDisconnect, ICanBeforeProcess, ICanBeforeProcessCycle, ICanProcessCycle, ICanAfterProcessCycle, ICanAfterProcess, ICanRemote, ICanStopContinousTrigger, ICanSaveLoadSettings, ICanSaveResult
    {
        public IImageCaptureDevice ImageCaptureDevice => ImageProcessSourceDC.ImageCaptureDevice;
        public ImageProcessSourceDC ImageProcessSourceDC { get; }
        public TriggerDC TriggerDC { get; }
        public IProperty<int> SelectedTabItemIndex { get; } = new Property<int>();
        public IProperty<int> InternalSelectedTabItemIndex { get; } = new Property<int>();


        readonly OpenCLAccelerator openCLAccelerator;
        readonly PopupWindowDC popupWindowDC;
        readonly object lockObject = new object();


        public ImageSourceDeviceDC(LanguageDC languageDC, LogDC logDC, OpenCLAccelerator openCLAccelerator, IImageCaptureDevice imageCaptureDevice, PopupWindowDC popupWindowDC, Common.SaveResult.SaveResultDC saveResultDC) : base(languageDC, logDC)
        {
            this.openCLAccelerator = openCLAccelerator;
            Name = languageDC.ImageSourceDevice;
            this.popupWindowDC = popupWindowDC;

            ImageProcessSourceDC = new ImageProcessSourceDC(languageDC, openCLAccelerator, imageCaptureDevice);
            ImageProcessSourceDC.Start();
            ImageProcessSourceDC.Capture();

            TriggerDC = new TriggerDC(languageDC, LogicalEvaluatorDC.Id, null, this, saveResultDC);

            ContinousStarter continousStarter = new ContinousStarter(ImageProcessSourceDC.Capture);
            SelectedTabItemIndex.OnValueChanged += (o, n) => InternalSelectedTabItemIndex.Value = 1;
            InternalSelectedTabItemIndex.OnValueChanged += (o, n) => continousStarter.IsEnabled = n == 0 && TriggerDC.IsEnabled.Value && !TriggerDC.ContinousModeIsEnabled.Value;

            ResetSelectedTabItemIndex();
        }

        public void ResetSelectedTabItemIndex()
        {
            SelectedTabItemIndex.Value = 0;
            InternalSelectedTabItemIndex.Value = 1;
        }

        public void StopContinousTrigger()
        {
            TriggerDC.ContinousModeIsEnabled.Value = false;
        }

        #region Process
        public void BeforeProcess()
        {
            TriggerDC.IsEnabled.Value = false;
        }

        public void BeforeProcessCycle()
        {
            lock (lockObject)
            {
                Children.ForEach(x => x.CastTo<ICanBeforeProcessCycle>().BeforeProcessCycle());
                ImageProcessSourceDC.Capture();
            }
        }

        public void ProcessCycle()
        {
            lock (lockObject)
            {
                Children.ForEach(x => x.CastTo<ICanProcessCycle>().ProcessCycle());
            }
        }

        public void AfterProcessCycle()
        {
            lock (lockObject)
            {
                Children.ForEach(x => x.CastTo<ICanAfterProcessCycle>().AfterProcessCycle());
                LogicalEvaluatorDC.Evulate();
            }
        }

        public void AfterProcess()
        {
            TriggerDC.IsEnabled.Value = true;
        }
        #endregion

        #region Disconnect
        public async void DisconnectButtonClick()
        {
            popupWindowDC.Show();
            await DisconnectAsync();
            popupWindowDC.Close();
        }

        public Task DisconnectAsync() => Task.Run(()=>Disconnect());

        public void Disconnect()
        {
            lock(lockObject)
            {
                TriggerDC.IsEnabled.Value = false;
                ImageProcessSourceDC.Stop();
            }
        }
        #endregion

        #region Inspection
        public void AddNewInspectionButtonClick()
        {
            lock(lockObject)
            {
                Children.ToSettable().Add(new InspectionDC(LanguageDC, logDC, openCLAccelerator, ImageProcessSourceDC));
            }
        }

        internal void RemoveSelectedInspectionButtonClick()
        {
            if (SelectedChild.Value.IsNull())
                return;

            lock(lockObject)
            {
                Children.ToSettable().Remove(SelectedChild.Value);
                SelectedChild.Value = null;
            }
        }

        public void RemoveAllInspectionsButtonClick()
        {
            if (Children.Value.Length == 0)
                return;

            if (Utils.ShowAreYouSureYouWantToDeleteThemAllQuestion(LanguageDC) == MessageBoxResult.No)
                return;

            lock(lockObject)
            {
                Children.ToSettable().Clear();
            }
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
                        o.CastTo<ICanRemote>().Remote(command, idsFirstRemoved);

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
            ImageProcessSourceDC.SaveSettings(settingsCollection);
            TriggerDC.SaveSettings(settingsCollection);

            settingsCollection.AddKey(nameof(Children));
            int length = Children.Value.Length;
            settingsCollection.SetProperty(length, nameof(Children.Value.Length));

            for (int index = 0; index < length; index++)
            {
                settingsCollection.AddKey(index.ToString());
                settingsCollection.SetProperty(Children.Value[index].GetType().Name, nameof(Type));
                Children.Value[index].CastTo<ICanSaveLoadSettings>().SaveSettings(settingsCollection);
                settingsCollection.RemoveLastKey(); // index.ToString()
            }

            settingsCollection.RemoveLastKey(); // nameof(Children)
            LogicalEvaluatorDC.SaveSettings(settingsCollection);
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            ImageProcessSourceDC.LoadSettings(settingsCollection);
            TriggerDC.LoadSettings(settingsCollection);

            settingsCollection.AddKey(nameof(Children));
            int length = settingsCollection.GetProperty<int>(nameof(Children.Value.Length));

            for (int index = 0; index < length; index++)
            {
                settingsCollection.AddKey(index.ToString());
                string typeName = settingsCollection.GetProperty<string>(nameof(Type));

                if(typeName == nameof(InspectionDC))
                {
                    Children.ToSettable().Add(new InspectionDC(LanguageDC, logDC, openCLAccelerator, ImageProcessSourceDC));
                    Children.Value[index].CastTo<ICanSaveLoadSettings>().LoadSettings(settingsCollection);
                }

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

            list.Add(new SaveResultDTO(keyCreator, Result.Value, ImageProcessSourceDC.ColorImage.Value));
            Children.ForEach(x => x.CastTo<ICanSaveResult>()?.SaveResult(keyCreator, list));

            keyCreator.RemoveLast();
        }
    }
}
