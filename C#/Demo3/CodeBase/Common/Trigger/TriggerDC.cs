using Common.Communication;
using Common.Interfaces;
using Common.Language;
using Common.NotifyProperty;
using Common.SaveResult;
using Common.Settings;
using Common.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;


namespace Common.Trigger
{
    public class TriggerDC : ICanSaveLoadSettings, ICanRemote
    {
        public LanguageDC LanguageDC { get; }
        public IProperty<bool> IsEnabled { get; } = new Property<bool>();
        public IProperty<bool> ContinousModeIsEnabled { get; } = new Property<bool>();
        public int MaximumStartDelay { get; } = 3000;
        public IProperty<int> StartDelay { get; } = new Property<int>(0);
        public int MinimumStartDelay { get; } = 0;
        public IReadOnlyProperty<bool> SingleStartButtonIsEnable { get; } = new Property<bool>(true);
        public IReadOnlyProperty<long> CycleTime { get; } = new Property<long>();
        public IReadOnlyProperty<string> StatusColor { get; } = new Property<string>();


        readonly TimeMeasure timeMeasure = new TimeMeasure();
        readonly AutoResetEvent finished = new AutoResetEvent(false);
        readonly IProperty<string> id;
        readonly AutoResetEvent oneShoot = new AutoResetEvent(false);
        readonly object canProcess;
        readonly object canProcessCycle;
        readonly SaveResultDC saveResultDC;
        bool disableNextSaveResult;


        public TriggerDC(LanguageDC languageDC, IProperty<string> id, object canProcess, object canProcessCycle, SaveResultDC saveResultDC)
        {
            LanguageDC = languageDC;
            this.id = id;
            this.canProcess = canProcess;
            this.canProcessCycle = canProcessCycle;
            this.saveResultDC = saveResultDC;

            IsEnabled.OnValueChanged += (o, n) =>
            {
                if (n)
                    StatusColor.ToSettable().Value = Colors.Green.ToString();
                else
                {
                    StatusColor.ToSettable().Value = Colors.Red.ToString();

                    if (ContinousModeIsEnabled.Value)
                    {
                        ContinousModeIsEnabled.Value = false;
                        finished.WaitOne();
                    }
                }
            };

            ContinousModeIsEnabled.OnValueChanged += (o, n) =>
            {
                if (!o && n)
                    oneShoot.Set();
            };

            Task.Run(()=>Method());
        }

        public void SingleStartButtonClick()
        {
            if (SingleStartButtonIsEnable.Value)
                ShootAsync();
        }

        public Task ShootAsync(bool disableNextSaveResult = false)
        {
            this.disableNextSaveResult = disableNextSaveResult;
            return Task.Run(() => Shoot());
        }

        public void Shoot()
        {
            oneShoot.Set();
            finished.WaitOne();
        }

        private void Method()
        {
            bool saveOnFinished;

            while (true)
            {
                oneShoot.WaitOne();
                finished.Reset();
                SingleStartButtonIsEnable.ToSettable().Value = false;
                StatusColor.ToSettable().Value = Colors.Red.ToString();
                canProcess.CastTo<ICanBeforeProcess>()?.BeforeProcess();
                saveOnFinished = !ContinousModeIsEnabled.Value && !disableNextSaveResult;
                disableNextSaveResult = false;

                do
                {
                    if (StartDelay.Value != 0)
                        Thread.Sleep(StartDelay.Value);

                    canProcessCycle.CastTo<ICanBeforeProcessCycle>()?.BeforeProcessCycle();
                    timeMeasure.Restart();
                    canProcessCycle.CastTo<ICanProcessCycle>()?.ProcessCycle();
                    CycleTime.ToSettable().Value = timeMeasure.Stop();
                    canProcessCycle.CastTo<ICanAfterProcessCycle>()?.AfterProcessCycle();

                } while (ContinousModeIsEnabled.Value);

                canProcess.CastTo<ICanAfterProcess>()?.AfterProcess();

                if (saveOnFinished)
                {
                    if(saveResultDC.IsEnable.Value)
                    {
                        KeyCreator keyCreator = new KeyCreator();
                        List<SaveResultDTO> list = new List<SaveResultDTO>();
                        canProcessCycle.CastTo<ICanSaveResult>()?.SaveResult(keyCreator, list);
                        saveResultDC.WriteDatas(list);
                    }
                    
                    saveOnFinished = false;
                }

                SingleStartButtonIsEnable.ToSettable().Value = true;
                StatusColor.ToSettable().Value = Colors.Green.ToString();
                finished.Set();
            }
        }

        public string Remote(string command, string[] ids)
        {
            string response = null;

            if(ids.Length == 1)
            {
                if(ids.First() == id.Value)
                {
                    if (command == Commands.TriggerOnce.ToString())
                    {
                        if (IsEnabled.Value)
                        {
                            if (ContinousModeIsEnabled.Value)
                                ContinousModeIsEnabled.Value = false;

                            Shoot();
                            response = Responses.Ok.ToString();
                        }
                        else
                            response = Responses.ErrorTriggerDisabled.ToString();
                    }
                }
            }

            return response;
        }

        public void SaveSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            settingsCollection.SetProperty(StartDelay.Value, nameof(StartDelay));
            settingsCollection.ExitPoint();
        }

        public void LoadSettings(ISettingsCollection settingsCollection)
        {
            settingsCollection.EntryPoint(GetType().Name);
            StartDelay.Value = settingsCollection.GetProperty<int>(nameof(StartDelay));
            settingsCollection.ExitPoint();
        }
    }
}
