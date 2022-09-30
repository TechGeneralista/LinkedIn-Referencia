using Common;
using Common.NotifyProperty;
using System;


namespace BreakAlarmApp
{
    public class ExternalAlarm
    {
        public event Action<ExternalAlarm> RemoveButtonPressed;

        public ISettableObservableProperty<bool> IsEnable { get; } = new ObservableProperty<bool>();
        public uint[] InputIdexes { get; private set; }
        public ISettableObservableProperty<uint> SelectedInputIndex { get; } = new ObservableProperty<uint>();
        public AlarmSoundPlayer AlarmSoundPlayer { get; }


        public ExternalAlarm(bool isEnable, uint inputIndex, uint playLengthSec, string soundFilePath)
        {
            IsEnable.CurrentValue = isEnable;
            InputIdexes = Utils.GenerateRange(0, 10);

            SelectedInputIndex.CurrentValue = inputIndex;

            AlarmSoundPlayer = new AlarmSoundPlayer(playLengthSec, soundFilePath);
        }

        internal void Play()
        {
            if (IsEnable.CurrentValue)
                AlarmSoundPlayer.Start();
        }

        internal void Remove() => RemoveButtonPressed?.Invoke(this);
    }
}