using Common.NotifyProperty;
using Common;
using Common.Settings;
using System.Timers;
using System;

namespace BreakAlarmApp
{
    public class TimedAlarms
    {
        public INonSettableObservableProperty<TimedAlarm[]> List { get; } = new ObservableProperty<TimedAlarm[]>(new TimedAlarm[0]);


        FileSettingsStore fileSettingsStore = new FileSettingsStore(Utils.GetPath("TimedAlarms.bin"));
        Timer triggerTimer;


        public TimedAlarms()
        {
            SettingsCollection settingsCollection = new SettingsCollection(fileSettingsStore);
            int length = settingsCollection.GetValue<int>(nameof(List.CurrentValue.Length));

            for (int i = 0; i < length; i++)
            {
                bool isEnable = settingsCollection.GetValue<bool>(string.Format("{0}/{1}", i, "IsEnable"));
                bool isTemponary = settingsCollection.GetValue<bool>(string.Format("{0}/{1}", i, "IsTemponary"));
                uint selectedHour = settingsCollection.GetValue<uint>(string.Format("{0}/{1}", i, "SelectedHour"));
                uint selectedMinute = settingsCollection.GetValue<uint>(string.Format("{0}/{1}", i, "SelectedMinute"));
                uint playLengthSec = settingsCollection.GetValue<uint>(string.Format("{0}/{1}", i, "PlayLengthSec"));
                string soundFilePath = settingsCollection.GetValue<string>(string.Format("{0}/{1}", i, "SoundFilePath"));

                TimedAlarm timedAlarm = new TimedAlarm(isEnable, isTemponary, selectedHour, selectedMinute, playLengthSec, soundFilePath);
                timedAlarm.RemoveButtonPressed += TimedAlarm_RemoveButtonPressed;
                List.ForceSet(List.CurrentValue.Add(timedAlarm));
            }

            triggerTimer = new Timer(1000);
            triggerTimer.Elapsed += Trigger;
            triggerTimer.AutoReset = true;
            triggerTimer.Enabled = true;
            triggerTimer.Start();
        }

        private void Trigger(object sender, ElapsedEventArgs e)
        {
            DateTime currentDateTime = DateTime.Now;
            foreach (TimedAlarm timedAlarm in List.CurrentValue)
                timedAlarm.Trigger(currentDateTime);
        }

        private void TimedAlarm_RemoveButtonPressed(TimedAlarm obj) => List.ForceSet(List.CurrentValue.Remove(obj));

        internal void AddNew()
        {
            TimedAlarm timedAlarm = new TimedAlarm(true, false, 6, 0, 3, null);
            timedAlarm.RemoveButtonPressed += TimedAlarm_RemoveButtonPressed;
            List.ForceSet(List.CurrentValue.Add(timedAlarm));
        }

        public void WriteList()
        {
            SettingsCollection settingsCollection = new SettingsCollection(fileSettingsStore);

            int length = List.CurrentValue.Length;
            settingsCollection.SetValue(nameof(List.CurrentValue.Length), length);

            for (int i = 0; i < length; i++)
            {
                TimedAlarm timedAlarm = List.CurrentValue[i];
                settingsCollection.SetValue(string.Format("{0}/{1}", i, nameof(timedAlarm.IsEnable)), timedAlarm.IsEnable.CurrentValue);
                settingsCollection.SetValue(string.Format("{0}/{1}", i, nameof(timedAlarm.IsTemponary)), timedAlarm.IsTemponary.CurrentValue);
                settingsCollection.SetValue(string.Format("{0}/{1}", i, nameof(timedAlarm.SelectedHour)), timedAlarm.SelectedHour.CurrentValue);
                settingsCollection.SetValue(string.Format("{0}/{1}", i, nameof(timedAlarm.SelectedMinute)), timedAlarm.SelectedMinute.CurrentValue);
                settingsCollection.SetValue(string.Format("{0}/{1}", i, nameof(timedAlarm.AlarmSoundPlayer.PlayLengthSec)), timedAlarm.AlarmSoundPlayer.PlayLengthSec.CurrentValue);
                settingsCollection.SetValue(string.Format("{0}/{1}", i, nameof(timedAlarm.AlarmSoundPlayer.SoundFilePath)), timedAlarm.AlarmSoundPlayer.SoundFilePath.CurrentValue);
            }

            settingsCollection.Write();
        }

        internal void EnableDisableTemponaries()
        {
            foreach (TimedAlarm timedAlarm in List.CurrentValue)
                timedAlarm.EnableDisableIfTemponary();
        }
    }
}