using Common;
using Common.Interface;
using Common.NotifyProperty;
using Common.Settings;
using Common.Tool;
using Language;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace CustomControl.Trigger
{
    public class TriggerDC : ICanSaveLoadSettings, ICanRemote
    {
        public event Action BeforeShoot, Shoot;

        public ISettableObservableProperty<bool> IsEnabled { get; } = new ObservableProperty<bool>();
        public LanguageDC LanguageDC { get; }
        public ISettableObservableProperty<bool> IsInternalTrigger { get; } = new ObservableProperty<bool>();
        public int MaximumStartDelay { get; } = 3000;
        public ISettableObservableProperty<int> StartDelay { get; } = new ObservableProperty<int>(0);
        public int MinimumStartDelay { get; } = 0;
        public INonSettableObservableProperty<bool> SingleStartButtonIsEnable { get; } = new ObservableProperty<bool>(true);
        public INonSettableObservableProperty<long> CycleTime { get; } = new ObservableProperty<long>();


        readonly TimeMeasure timeMeasure = new TimeMeasure();
        readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        readonly ISettableObservableProperty<string> id;


        public TriggerDC(LanguageDC languageDC, ISettableObservableProperty<string> id)
        {
            LanguageDC = languageDC;
            this.id = id;

            IsEnabled.CurrentValueChanged += (o, n) =>
            {
                if (!n)
                    Stop();
            };

            IsInternalTrigger.CurrentValueChanged += (o,n) => 
            {
                if (n)
                    CycleAsync();
            };
        }

        public void Stop()
        {
            if(IsInternalTrigger.CurrentValue)
            {
                IsInternalTrigger.CurrentValue = false;
                autoResetEvent.WaitOne();
            }
        }

        public Task CycleAsync() => Task.Run(() => Cycle());

        public void Cycle()
        {
            SingleStartButtonIsEnable.ForceSet(false);
            autoResetEvent.Reset();
            
            do
            {
                Thread.Sleep(StartDelay.CurrentValue);
                BeforeShoot?.Invoke();
                timeMeasure.Restart();
                Shoot?.Invoke();
                CycleTime.ForceSet(timeMeasure.Stop());
            } while (IsInternalTrigger.CurrentValue);

            autoResetEvent.Set();
            SingleStartButtonIsEnable.ForceSet(true);
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(TriggerDC));
            settingsCollection.KeyCreator.AddNew(nameof(StartDelay));
            settingsCollection.SetValue(StartDelay.CurrentValue);
            settingsCollection.KeyCreator.RemoveLast(2);
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.KeyCreator.AddNew(nameof(TriggerDC));
            settingsCollection.KeyCreator.AddNew(nameof(StartDelay));
            StartDelay.CurrentValue = settingsCollection.GetValue<int>();
            settingsCollection.KeyCreator.RemoveLast(2);
        }

        public void Remote(ref Response response, Command command, string[] ids)
        {
            if (!IsEnabled.CurrentValue || ids.Length != 1 || ids[0] != id.CurrentValue)
                return;

            response = Response.Ok;
            Cycle();
        }
    }
}
