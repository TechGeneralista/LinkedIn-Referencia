using Common;
using Common.NotifyProperty;
using Common.Settings;
using System.Linq;


namespace BreakAlarmApp
{
    public class ExternalAlarms
    {
        public INonSettableObservableProperty<ExternalAlarm[]> List { get; } = new ObservableProperty<ExternalAlarm[]>(new ExternalAlarm[0]);


        FileSettingsStore fileSettingsStore = new FileSettingsStore(Utils.GetPath("ExternalAlarms.bin"));


        public ExternalAlarms()
        {
            SettingsCollection settingsCollection = new SettingsCollection(fileSettingsStore);
            int length = settingsCollection.GetValue<int>(nameof(List.CurrentValue.Length));

            for (int i = 0; i < length; i++)
            {
                bool isEnable = settingsCollection.GetValue<bool>(string.Format("{0}/{1}", i, "IsEnable"));
                uint inputIndex = settingsCollection.GetValue<uint>(string.Format("{0}/{1}", i, "SelectedInputIndex"));
                uint playLengthSec = settingsCollection.GetValue<uint>(string.Format("{0}/{1}", i, "PlayLengthSec"));
                string soundFilePath = settingsCollection.GetValue<string>(string.Format("{0}/{1}", i, "SoundFilePath"));

                ExternalAlarm externalAlarm = new ExternalAlarm(isEnable, inputIndex, playLengthSec, soundFilePath);
                externalAlarm.RemoveButtonPressed += ExternalAlarm_RemoveButtonPressed;
                List.ForceSet(List.CurrentValue.Add(externalAlarm));
            }
        }

        ~ExternalAlarms()
        {
            WriteList();
        }


        internal void WriteList()
        {
            SettingsCollection settingsCollection = new SettingsCollection(fileSettingsStore);

            int length = List.CurrentValue.Length;
            settingsCollection.SetValue(nameof(List.CurrentValue.Length), length);

            for (int i = 0; i < length; i++)
            {
                ExternalAlarm externalAlarm = List.CurrentValue[i];
                settingsCollection.SetValue(string.Format("{0}/{1}", i, nameof(externalAlarm.IsEnable)), externalAlarm.IsEnable.CurrentValue);
                settingsCollection.SetValue(string.Format("{0}/{1}", i, nameof(externalAlarm.SelectedInputIndex)), externalAlarm.SelectedInputIndex.CurrentValue);
                settingsCollection.SetValue(string.Format("{0}/{1}", i, nameof(externalAlarm.AlarmSoundPlayer.PlayLengthSec)), externalAlarm.AlarmSoundPlayer.PlayLengthSec.CurrentValue);
                settingsCollection.SetValue(string.Format("{0}/{1}", i, nameof(externalAlarm.AlarmSoundPlayer.SoundFilePath)), externalAlarm.AlarmSoundPlayer.SoundFilePath.CurrentValue);
            }

            settingsCollection.Write();
        }

        internal void Start(int inputIndex)
        {
            ExternalAlarm externalAlarm = List.CurrentValue.FirstOrDefault(x => x.SelectedInputIndex.CurrentValue == inputIndex);

            if (externalAlarm.IsNull())
                return;

            externalAlarm.Play();
        }

        internal void AddNew()
        {
            ExternalAlarm externalAlarm = new ExternalAlarm(true, 0, 3, null);
            externalAlarm.RemoveButtonPressed += ExternalAlarm_RemoveButtonPressed;
            List.ForceSet(List.CurrentValue.Add(externalAlarm));
        }

        private void ExternalAlarm_RemoveButtonPressed(ExternalAlarm obj) => List.ForceSet(List.CurrentValue.Remove(obj));
    }
}