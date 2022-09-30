using Common;
using Common.NotifyProperty;
using System;


namespace BreakAlarmApp
{
    public class TimedAlarm
    {
        public event Action<TimedAlarm> RemoveButtonPressed;


        public ISettableObservableProperty<bool> IsEnable { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<bool> IsTemponary { get; } = new ObservableProperty<bool>();
        public uint[] Hours { get; private set; }
        public ISettableObservableProperty<uint> SelectedHour { get; } = new ObservableProperty<uint>();
        public uint[] Minutes { get; private set; }
        public ISettableObservableProperty<uint> SelectedMinute { get; } = new ObservableProperty<uint>();
        public AlarmSoundPlayer AlarmSoundPlayer { get; }


        public TimedAlarm(bool isEnable, bool isTemponary, uint hour, uint minute, uint playLengthSec, string soundFilePath)
        {
            IsEnable.CurrentValue = isEnable;
            IsTemponary.CurrentValue = isTemponary;

            Hours = Utils.GenerateRange(0, 24);
            Minutes = Utils.GenerateRange(0, 60);

            SelectedHour.CurrentValue = hour;
            SelectedMinute.CurrentValue = minute;
            AlarmSoundPlayer = new AlarmSoundPlayer(playLengthSec, soundFilePath);
        }

        internal void Remove() => RemoveButtonPressed?.Invoke(this);

        internal void Trigger(DateTime currentDateTime)
        {
            if (currentDateTime.Hour == SelectedHour.CurrentValue && currentDateTime.Minute == SelectedMinute.CurrentValue && currentDateTime.Second == 0 && IsEnable.CurrentValue)
                AlarmSoundPlayer.Start();
        }

        internal void EnableDisableIfTemponary()
        {
            if (IsTemponary.CurrentValue)
                IsEnable.CurrentValue = !IsEnable.CurrentValue;
        }
    }
}