using Common;
using Common.NotifyProperty;
using Microsoft.Win32;
using System;
using System.IO;
using System.Timers;
using System.Windows.Media;


namespace BreakAlarmApp
{
    public class AlarmSoundPlayer
    {
        public INonSettableObservableProperty<bool> IsPlaying { get; } = new ObservableProperty<bool>();
        public ISettableObservableProperty<uint> PlayLengthSec { get; } = new ObservableProperty<uint>(3);
        public INonSettableObservableProperty<string> SoundFilePath { get; } = new ObservableProperty<string>();


        readonly MediaPlayer mediaPlayer;
        readonly Timer stopTimer;


        public AlarmSoundPlayer(uint playLengthSec, string soundFilePath)
        {
            PlayLengthSec.CurrentValue = playLengthSec;
            SoundFilePath.ForceSet(soundFilePath);

            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            stopTimer = new Timer();
            stopTimer.AutoReset = false;
            stopTimer.Elapsed += StopTimer_Elapsed;
        }

        internal void Start()
        {
            if (IsPlaying.CurrentValue || string.IsNullOrEmpty(SoundFilePath.CurrentValue) || !File.Exists(SoundFilePath.CurrentValue))
                return;

            IsPlaying.ForceSet(true);

            try
            {
                Utils.InvokeIfNecessary(() =>
                {
                    mediaPlayer.Open(new Uri(SoundFilePath.CurrentValue));
                });
            }
            catch { }
        }

        private void StopTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Utils.InvokeIfNecessary(() =>
                {
                    mediaPlayer.Stop();
                    mediaPlayer.Close();
                });
            }
            catch { }

            IsPlaying.ForceSet(false);
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            MediaPlayer mediaPlayer = (MediaPlayer)sender;
            mediaPlayer.Play();

            stopTimer.Interval = PlayLengthSec.CurrentValue * 1000;
            stopTimer.Start();
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            MediaPlayer mediaPlayer = (MediaPlayer)sender;
            mediaPlayer.Position = TimeSpan.Zero;
            mediaPlayer.Play();
        }

        internal void BrowseSoundFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "hang fájlok | *.*";
            bool? b = openFileDialog.ShowDialog();

            if (b.IsNotNull() && b == true)
                SoundFilePath.ForceSet(openFileDialog.FileName);
        }
    }
}