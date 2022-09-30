using AppLog;
using Common;
using Common.Interface;
using Common.NotifyProperty;
using Common.Settings;
using CustomControl.Id;
using ImageProcess.ContourFinder.Detector;
using Language;
using System;
using System.Windows.Media;


namespace ImageProcess.Modules.Counter
{
    public class CounterDC : ICanEvulateDetectorResult, IHasId, IHasResult, ICanShowResultImage, ICanSaveLoadSettings
    {
        public LanguageDC LanguageDC { get; }
        public IdDC IdDC { get; }
        public ISettableObservableProperty<uint> MinimumQuantity { get; } = new ObservableProperty<uint>(1);
        public ISettableObservableProperty<uint> MaximumQuantity { get; } = new ObservableProperty<uint>(1);
        public ISettableObservableProperty<bool> MinimumIsEnable { get; } = new ObservableProperty<bool>(true);
        public ISettableObservableProperty<bool> MaximumIsEnable { get; } = new ObservableProperty<bool>(true);
        public ISettableObservableProperty<bool> InvertResult { get; } = new ObservableProperty<bool>();
        public INonSettableObservableProperty<bool?> Result { get; } = new ObservableProperty<bool?>();


        IDetectorResult lastDetectorResult;
        readonly CounterResultDrawer counterResultDrawer;


        public CounterDC(LanguageDC languageDC, IDetectorResult detectorResult, ISettableObservableProperty<ImageSource> mainDisplaySource, ILog log)
        {
            LanguageDC = languageDC;
            lastDetectorResult = detectorResult;
            IdDC = new IdDC(languageDC, log);

            MinimumQuantity.CurrentValueChanged += MinimumQuantity_CurrentValueChanged;
            MaximumQuantity.CurrentValueChanged += MaximumQuantity_CurrentValueChanged;
            InvertResult.CurrentValueChanged += InvertResult_CurrentValueChanged;
            MinimumIsEnable.CurrentValueChanged += MinimumIsEnable_CurrentValueChanged;
            MaximumIsEnable.CurrentValueChanged += MaximumIsEnable_CurrentValueChanged;

            counterResultDrawer = new CounterResultDrawer(mainDisplaySource);
        }

        private void MaximumIsEnable_CurrentValueChanged(bool ov, bool nv)
        {
            if (!nv && !MinimumIsEnable.CurrentValue)
                MinimumIsEnable.CurrentValue = true;
        }

        private void MinimumIsEnable_CurrentValueChanged(bool ov, bool nv)
        {
            if (!nv && !MaximumIsEnable.CurrentValue)
                MaximumIsEnable.CurrentValue = true;
        }

        private void InvertResult_CurrentValueChanged(bool ov, bool nv)
        {
            if (Result.CurrentValue.IsNull())
                return;

            Result.ForceSet(!Result.CurrentValue);
        }

        private void MaximumQuantity_CurrentValueChanged(uint ov, uint nv)
        {
            if (MinimumQuantity.CurrentValue > MaximumQuantity.CurrentValue)
                MaximumQuantity.CurrentValue = ov;
        }

        private void MinimumQuantity_CurrentValueChanged(uint ov, uint nv)
        {
            if (MinimumQuantity.CurrentValue > MaximumQuantity.CurrentValue)
                MinimumQuantity.CurrentValue = ov;
        }

        public void Evulate(IDetectorResult detectorResult)
        {
            lastDetectorResult = detectorResult;

            bool? result = null;
            uint currentQuantity;

            if (detectorResult.PositionResults.CurrentValue.IsNull() || detectorResult.PositionResults.CurrentValue.Length == 0)
                currentQuantity = 0;
            else
                currentQuantity = (uint)detectorResult.PositionResults.CurrentValue.Length;

            if (MinimumIsEnable.CurrentValue && !MaximumIsEnable.CurrentValue)
            {
                if (currentQuantity >= MinimumQuantity.CurrentValue)
                    result = true;
                else
                    result = false;
            }

            else if (!MinimumIsEnable.CurrentValue && MaximumIsEnable.CurrentValue)
            {
                if (currentQuantity <= MaximumQuantity.CurrentValue)
                    result = true;
                else
                    result = false;
            }

            else if (MinimumIsEnable.CurrentValue && MaximumIsEnable.CurrentValue)
            {
                if (currentQuantity >= MinimumQuantity.CurrentValue && currentQuantity <= MaximumQuantity.CurrentValue)
                    result = true;
                else
                    result = false;
            }

            if (InvertResult.CurrentValue)
                Result.ForceSet(!result);
            else
                Result.ForceSet(result);
        }

        public void ShowResultImage() => counterResultDrawer.Draw(lastDetectorResult, Result.CurrentValue);

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(Type));
            settingsCollection.SetValue(nameof(CounterDC));
            settingsCollection.KeyCreator.RemoveLast();

            IdDC.SaveSettings(settingsCollection);

            settingsCollection.KeyCreator.AddNew(nameof(MinimumQuantity));
            settingsCollection.SetValue(MinimumQuantity.CurrentValue);
            settingsCollection.KeyCreator.ReplaceLast(nameof(MaximumQuantity));
            settingsCollection.SetValue(MaximumQuantity.CurrentValue);
            settingsCollection.KeyCreator.ReplaceLast(nameof(MinimumIsEnable));
            settingsCollection.SetValue(MinimumIsEnable.CurrentValue);
            settingsCollection.KeyCreator.ReplaceLast(nameof(MaximumIsEnable));
            settingsCollection.SetValue(MaximumIsEnable.CurrentValue);
            settingsCollection.KeyCreator.ReplaceLast(nameof(InvertResult));
            settingsCollection.SetValue(InvertResult.CurrentValue);
            settingsCollection.KeyCreator.RemoveLast();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            IdDC.LoadSettings(settingsCollection);

            settingsCollection.KeyCreator.AddNew(nameof(MinimumQuantity));
            MinimumQuantity.CurrentValue = settingsCollection.GetValue<uint>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(MaximumQuantity));
            MaximumQuantity.CurrentValue = settingsCollection.GetValue<uint>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(MinimumIsEnable));
            MinimumIsEnable.CurrentValue = settingsCollection.GetValue<bool>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(MaximumIsEnable));
            MaximumIsEnable.CurrentValue = settingsCollection.GetValue<bool>();
            settingsCollection.KeyCreator.ReplaceLast(nameof(InvertResult));
            InvertResult.CurrentValue = settingsCollection.GetValue<bool>();
            settingsCollection.KeyCreator.RemoveLast();
        }
    }
}
