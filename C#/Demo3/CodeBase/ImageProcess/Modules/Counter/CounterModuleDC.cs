using Common;
using Common.Communication;
using Common.Id;
using Common.Language;
using Common.Log;
using Common.NotifyProperty;
using Common.Settings;
using ImageProcess.ContourScanner.ContourDetector;
using System.Linq;


namespace ImageProcess.Modules.Counter
{
    public class CounterModuleDC : ICanSaveLoadSettings
    {
        public IReadOnlyProperty<bool?> Result { get; } = new Property<bool?>();
        public LanguageDC LanguageDC { get; }
        public IProperty<uint> MinimumQuantity { get; } = new Property<uint>(1);
        public IProperty<uint> MaximumQuantity { get; } = new Property<uint>(1);
        public IProperty<bool> MinimumIsEnable { get; } = new Property<bool>(true);
        public IProperty<bool> MaximumIsEnable { get; } = new Property<bool>(false);
        public IProperty<bool> InvertResult { get; } = new Property<bool>();
        public IdDC IdDC { get; }


        readonly DetectorResultDC detectorResultDC;


        public CounterModuleDC(LanguageDC languageDC, LogDC logDC, DetectorResultDC detectorResultDC)
        {
            LanguageDC = languageDC;
            IdDC = new IdDC(languageDC, logDC);
            this.detectorResultDC = detectorResultDC;

            MinimumQuantity.OnValueChanged += MinimumQuantity_ValueChanged;
            MaximumQuantity.OnValueChanged += MaximumQuantity_ValueChanged;
            InvertResult.OnValueChanged += InvertResult_ValueChanged;
            MinimumIsEnable.OnValueChanged += MinimumIsEnable_ValueChanged;
            MaximumIsEnable.OnValueChanged += MaximumIsEnable_ValueChanged;
        }

        private void MaximumIsEnable_ValueChanged(bool ov, bool nv)
        {
            if (!nv && !MinimumIsEnable.Value)
                MinimumIsEnable.Value = true;
        }

        public string Remote(string command, string[] ids)
        {
            string response = null;

            if (ids.Length == 1)
            {
                if (ids.First() == IdDC.Id.Value)
                {
                    if (command == Commands.GetResult.ToString())
                    {
                        if (Result.Value.IsNull())
                            response = Responses.NotAvailable.ToString();

                        else if (Result.Value == true)
                            response = Responses.Ok.ToString();

                        else if (Result.Value == false)
                            response = Responses.Nok.ToString();
                    }
                }
            }

            return response;
        }

        private void MinimumIsEnable_ValueChanged(bool ov, bool nv)
        {
            if (!nv && !MaximumIsEnable.Value)
                MaximumIsEnable.Value = true;
        }

        private void InvertResult_ValueChanged(bool ov, bool nv)
        {
            if (Result.Value.IsNull())
                return;

            Result.ToSettable().Value =!Result.Value;
        }

        private void MaximumQuantity_ValueChanged(uint ov, uint nv)
        {
            if (MinimumQuantity.Value > MaximumQuantity.Value)
                MaximumQuantity.Value = ov;
        }

        private void MinimumQuantity_ValueChanged(uint ov, uint nv)
        {
            if (MinimumQuantity.Value > MaximumQuantity.Value)
                MinimumQuantity.Value = ov;
        }

        public void ProcessCycle()
        {
            bool? result = null;
            uint currentQuantity;

            if (detectorResultDC.PositionResults.Value.IsNull() || detectorResultDC.PositionResults.Value.Length == 0)
                currentQuantity = 0;
            else
                currentQuantity = (uint)detectorResultDC.PositionResults.Value.Length;

            if (MinimumIsEnable.Value && !MaximumIsEnable.Value)
            {
                if (currentQuantity >= MinimumQuantity.Value)
                    result = true;
                else
                    result = false;
            }

            else if (!MinimumIsEnable.Value && MaximumIsEnable.Value)
            {
                if (currentQuantity <= MaximumQuantity.Value)
                    result = true;
                else
                    result = false;
            }

            else if (MinimumIsEnable.Value && MaximumIsEnable.Value)
            {
                if (currentQuantity >= MinimumQuantity.Value && currentQuantity <= MaximumQuantity.Value)
                    result = true;
                else
                    result = false;
            }

            if (InvertResult.Value)
                Result.ToSettable().Value = !result;
            else
                Result.ToSettable().Value = result;
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            IdDC.SaveSettings(settingsCollection);

            settingsCollection.SetProperty(MinimumQuantity.Value, nameof(MinimumQuantity));
            settingsCollection.SetProperty(MaximumQuantity.Value, nameof(MaximumQuantity));
            settingsCollection.SetProperty(MinimumIsEnable.Value, nameof(MinimumIsEnable));
            settingsCollection.SetProperty(MaximumIsEnable.Value, nameof(MaximumIsEnable));
            settingsCollection.SetProperty(InvertResult.Value, nameof(InvertResult));

            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            IdDC.LoadSettings(settingsCollection);

            MinimumQuantity.DisableNextOnValueChangeEvents();
            MinimumQuantity.Value = settingsCollection.GetProperty<uint>(nameof(MinimumQuantity));
            MaximumQuantity.DisableNextOnValueChangeEvents();
            MaximumQuantity.Value = settingsCollection.GetProperty<uint>(nameof(MaximumQuantity));
            MinimumIsEnable.DisableNextOnValueChangeEvents();
            MinimumIsEnable.Value = settingsCollection.GetProperty<bool>(nameof(MinimumIsEnable));
            MaximumIsEnable.DisableNextOnValueChangeEvents();
            MaximumIsEnable.Value = settingsCollection.GetProperty<bool>(nameof(MaximumIsEnable));
            InvertResult.DisableNextOnValueChangeEvents();
            InvertResult.Value = settingsCollection.GetProperty<bool>(nameof(InvertResult));

            settingsCollection.ExitPoint();
        }
    }
}
